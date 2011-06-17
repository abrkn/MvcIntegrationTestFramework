using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using MvcIntegrationTestFramework.Browsing;

namespace MvcIntegrationTestFramework.Hosting
{
    /// <summary>
    /// Simply provides a remoting gateway to execute code within the ASP.NET-hosting appdomain
    /// </summary>
    internal class AppDomainProxy : MarshalByRefObject
    {
        public void RunCodeInAppDomain(Action<Dictionary<string, string>> codeToRun, Dictionary<string, string> appSettings)
        {
            codeToRun(appSettings);
        }

        public void RunBrowsingSessionInAppDomain(SerializableDelegate<Action<BrowsingSession>> script)
        {
            var browsingSession = new BrowsingSession();
            script.Delegate(browsingSession);
        }

        public override object InitializeLifetimeService()
        {
            return null; // Tells .NET not to expire this remoting object
        }
    }
}