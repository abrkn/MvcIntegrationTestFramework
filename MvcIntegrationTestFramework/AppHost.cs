// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppHost.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Defines the AppHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;

    /// <summary>
    ///   Hosts an ASP.NET application within an ASP.NET-enabled .NET appdomain
    ///   and provides methods for executing test code within that appdomain
    /// </summary>
    public class AppHost
    {
        /// <summary>
        /// Reflected method info to access GetApplicationInstance.
        /// </summary>
        private static readonly MethodInfo getApplicationInstanceMethod;

        /// <summary>
        /// Reflected method info to access RecycleApplicationInstance.
        /// </summary>
        private static readonly MethodInfo recycleApplicationInstanceMethod;

        /// <summary>
        /// The gateway to the ASP.NET-enabled .NET appdomain
        /// </summary>
        private readonly AppDomainProxy appDomainProxy;

        /// <summary>
        /// The application settings used to override values in the ASP.NET web site's Web.config.
        /// </summary>
        private static NameValueCollection appSettings;

        /// <summary>
        /// Initializes static members of the <see cref="AppHost"/> class.
        /// </summary>
        static AppHost()
        {
            // Get references to some MethodInfos we'll need to use later to bypass nonpublic access restrictions
            var httpApplicationFactory = typeof(HttpContext).Assembly.GetType("System.Web.HttpApplicationFactory", true);

            getApplicationInstanceMethod = httpApplicationFactory.GetMethod(
                "GetApplicationInstance", BindingFlags.Static | BindingFlags.NonPublic);
            recycleApplicationInstanceMethod = httpApplicationFactory.GetMethod(
                "RecycleApplicationInstance", BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppHost"/> class.
        /// </summary>
        /// <param name="appPhysicalDirectory">The app physical directory.</param>
        /// <param name="virtualDirectory">The virtual directory.</param>
        private AppHost(string appPhysicalDirectory, string virtualDirectory = "/")
        {
            if (virtualDirectory == null)
            {
                throw new ArgumentNullException("virtualDirectory");
            }

            this.appDomainProxy =
                (AppDomainProxy)
                ApplicationHost.CreateApplicationHost(typeof(AppDomainProxy), virtualDirectory, appPhysicalDirectory);
            this.appDomainProxy.RunCodeInAppDomain(
                (aSettings) =>
                    {
                        if (aSettings != null)
                        {
                            OverrideAppSettings(aSettings);
                        }

                        InitializeApplication();

                        FilterProviders.Providers.Add(new InterceptionFilterProvider());
                        LastRequestData.Reset();
                    }, 
                NameValueCollectionConversions.ConvertFromNameValueCollection(appSettings));
        }

        /// <summary>
        /// Creates an instance of the AppHost so it can be used to simulate a browsing session.
        /// </summary>
        /// <param name="projectDirectory">The MVC project directory.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="projectDirectory"/> is null.</exception>
        /// <returns>The simulated application host.</returns>
        public static AppHost Simulate(string projectDirectory)
        {
            var mvcProjectPath = GetMvcProjectPath(projectDirectory);
            if (mvcProjectPath == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Mvc Project {0} not found", projectDirectory));
            }

            CopyDllFiles(mvcProjectPath);
            return new AppHost(mvcProjectPath);
        }

        /// <summary>
        /// Simulates the specified MVC project directory.
        /// </summary>
        /// <param name="projectDirectory">The MVC project directory.</param>
        /// <param name="settingValues">The app setting values.</param>
        /// <returns>The simulated application host.</returns>
        public static AppHost Simulate(string projectDirectory, NameValueCollection settingValues)
        {
            if (settingValues == null)
            {
                throw new ArgumentNullException("settingValues");
            }

            appSettings = settingValues;

            var mvcProjectPath = GetMvcProjectPath(projectDirectory);

            if (mvcProjectPath == null)
            {
                throw new ArgumentException(string.Format("Mvc Project {0} not found", projectDirectory));
            }

            CopyDllFiles(mvcProjectPath);

            return new AppHost(mvcProjectPath);
        }

        /// <summary>
        /// Starts the specified test script.
        /// </summary>
        /// <param name="testScript">The test script.</param>
        public void Start(Action<BrowsingSession> testScript)
        {
            var serializableDelegate = new SerializableDelegate<Action<BrowsingSession>>(testScript);
            this.appDomainProxy.RunBrowsingSessionInAppDomain(serializableDelegate);
        }

        /// <summary>
        /// Copies DLL files from the MVC project to the current application domain's base directory.
        /// </summary>
        /// <param name="mvcProjectPath">The MVC project path.</param>
        private static void CopyDllFiles(string mvcProjectPath)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var file in Directory.GetFiles(baseDirectory, "*.dll"))
            {
                var destFile = Path.Combine(mvcProjectPath, "bin", Path.GetFileName(file));

                if (!File.Exists(destFile) || File.GetCreationTimeUtc(destFile) != File.GetCreationTimeUtc(file))
                {
                    File.Copy(file, destFile, true);
                }
            }
        }

        /// <summary>
        /// Gets the application instance.
        /// </summary>
        /// <returns>The application instance.</returns>
        private static HttpApplication GetApplicationInstance()
        {
            var writer = new StringWriter();
            var workerRequest = new SimpleWorkerRequest(string.Empty, string.Empty, writer);
            var httpContext = new HttpContext(workerRequest);
            return (HttpApplication)getApplicationInstanceMethod.Invoke(null, new object[] { httpContext });
        }

        /// <summary>
        /// Gets the MVC project path from the specified project name.
        /// </summary>
        /// <param name="mvcProjectName">Name of the MVC project.</param>
        /// <returns>The ASP.NET MVC project path.</returns>
        private static string GetMvcProjectPath(string mvcProjectName)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            while (baseDirectory.Contains("\\"))
            {
                baseDirectory = baseDirectory.Substring(0, baseDirectory.LastIndexOf("\\"));
                var mvcPath = Path.Combine(baseDirectory, mvcProjectName);
                if (Directory.Exists(mvcPath))
                {
                    return mvcPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        private static void InitializeApplication()
        {
            var appInstance = GetApplicationInstance();

            appInstance.PostRequestHandlerExecute += delegate
                {
                    // Collect references to context objects that would otherwise be lost
                    // when the request is completed
                    if (LastRequestData.HttpSessionState == null)
                    {
                        LastRequestData.HttpSessionState = HttpContext.Current.Session;
                    }

                    if (LastRequestData.Response == null)
                    {
                        LastRequestData.Response = HttpContext.Current.Response;
                    }
                };

            RefreshEventsList(appInstance);

            RecycleApplicationInstance(appInstance);
        }

        /// <summary>
        /// Overrides the app settings.
        /// </summary>
        /// <param name="settings">The app settings to override with.</param>
        private static void OverrideAppSettings(Dictionary<string, string> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            foreach (var originalKey in ConfigurationManager.AppSettings.AllKeys)
            {
                foreach (var newKey in settings.Keys.Where(newKey => newKey == originalKey))
                {
                    ConfigurationManager.AppSettings[originalKey] = settings[newKey];
                }
            }
        }

        /// <summary>
        /// Recycles the application instance.
        /// </summary>
        /// <param name="appInstance">The app instance.</param>
        private static void RecycleApplicationInstance(HttpApplication appInstance)
        {
            recycleApplicationInstanceMethod.Invoke(null, new object[] { appInstance });
        }

        /// <summary>
        /// Refreshes the events list.
        /// </summary>
        /// <param name="appInstance">The app instance.</param>
        private static void RefreshEventsList(HttpApplication appInstance)
        {
            var stepManager =
                typeof(HttpApplication).GetField("_stepManager", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(appInstance);

            var resumeStepsWaitCallback =
                typeof(HttpApplication).GetField(
                    "_resumeStepsWaitCallback", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(appInstance);

            var buildStepsMethod = stepManager.GetType().GetMethod(
                "BuildSteps", BindingFlags.NonPublic | BindingFlags.Instance);

            buildStepsMethod.Invoke(stepManager, new[] { resumeStepsWaitCallback });
        }
    }
}