// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastRequestData.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Defines the LastRequestData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;

    /// <summary>
    ///   A place to store information about each request as we capture it
    ///   Static because HttpRuntime.ProcessRequest() exposes no convenient hooks for intercepting the
    ///   request processing pipeline, so we're statically attaching an interceptor to all loaded controllers
    /// </summary>
    internal static class LastRequestData
    {
        /// <summary>
        /// Gets or sets the action executed context.
        /// </summary>
        /// <value>
        /// The action executed context.
        /// </value>
        public static ActionExecutedContext ActionExecutedContext { get; set; }

        /// <summary>
        /// Gets or sets the state of the HTTP session.
        /// </summary>
        /// <value>
        /// The state of the HTTP session.
        /// </value>
        public static HttpSessionState HttpSessionState { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public static HttpResponse Response { get; set; }

        /// <summary>
        /// Gets or sets the result executed context.
        /// </summary>
        /// <value>
        /// The result executed context.
        /// </value>
        public static ResultExecutedContext ResultExecutedContext { get; set; }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public static void Reset()
        {
            ActionExecutedContext = null;
            ResultExecutedContext = null;
            HttpSessionState = null;
            Response = null;
        }
    }
}