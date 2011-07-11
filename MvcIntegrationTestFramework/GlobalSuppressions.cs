// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   GlobalSuppressions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member",
    Target = "MvcIntegrationTestFramework.Hosting.AppHost.#GetApplicationInstance()",
    Justification = "Passed via reflection to HttpRuntime which handles disposal.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
    Scope = "member",
    Target = "MvcIntegrationTestFramework.Browsing.BrowsingSession.#ProcessRequest(System.String,System.Web.Mvc.HttpVerbs,System.Collections.Specialized.NameValueCollection,System.Collections.Specialized.NameValueCollection)",
    Justification = "Passed via reflection to HttpRuntime which handles disposal.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "MvcIntegrationTestFramework.Hosting.AppDomainProxy.#RunCodeInAppDomain(System.Action`1<System.Collections.Generic.Dictionary`2<System.String,System.String>>,System.Collections.Generic.Dictionary`2<System.String,System.String>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "MvcIntegrationTestFramework.Hosting.AppDomainProxy.#RunBrowsingSessionInAppDomain(MvcIntegrationTestFramework.Hosting.SerializableDelegate`1<System.Action`1<MvcIntegrationTestFramework.Browsing.BrowsingSession>>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "MvcIntegrationTestFramework.Browsing.MvcUtils")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc", Scope = "namespace", Target = "MvcIntegrationTestFramework.Browsing")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc", Scope = "type", Target = "MvcIntegrationTestFramework.MvcUtils")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "MvcIntegrationTestFramework.MvcUtils")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc", Scope = "namespace", Target = "MvcIntegrationTestFramework")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mvc")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "MvcIntegrationTestFramework.AppDomainProxy.#RunBrowsingSessionInAppDomain(MvcIntegrationTestFramework.SerializableDelegate`1<System.Action`1<MvcIntegrationTestFramework.BrowsingSession>>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "MvcIntegrationTestFramework.AppDomainProxy.#RunCodeInAppDomain(System.Action`1<System.Collections.Generic.Dictionary`2<System.String,System.String>>,System.Collections.Generic.Dictionary`2<System.String,System.String>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "MvcIntegrationTestFramework.BrowsingSession.#ProcessRequest(System.String,System.Web.Mvc.HttpVerbs,System.Collections.Specialized.NameValueCollection,System.Collections.Specialized.NameValueCollection)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "MvcIntegrationTestFramework.AppHost.#GetApplicationInstance()")]
