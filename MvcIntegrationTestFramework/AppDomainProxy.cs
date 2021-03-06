﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomainProxy.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Simply provides a remoting gateway to execute code within the ASP.NET-hosting appdomain
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Simply provides a remoting gateway to execute code within the ASP.NET-hosting appdomain
    /// </summary>
    internal class AppDomainProxy : MarshalByRefObject
    {
        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
        /// </PermissionSet>
        public override object InitializeLifetimeService()
        {
            return null; // Tells .NET not to expire this remoting object
        }

        /// <summary>
        /// Runs the browsing session in app domain.
        /// </summary>
        /// <param name="script">The script.</param>
        public void RunBrowsingSessionInAppDomain(SerializableDelegate<Action<BrowsingSession>> script)
        {
            var browsingSession = new BrowsingSession();
            script.Delegate(browsingSession);
        }

        /// <summary>
        /// Runs the code in app domain.
        /// </summary>
        /// <param name="codeToRun">The code to run.</param>
        /// <param name="appSettings">The app settings.</param>
        public void RunCodeInAppDomain(
            Action<Dictionary<string, string>> codeToRun, Dictionary<string, string> appSettings)
        {
            codeToRun(appSettings);
        }
    }
}