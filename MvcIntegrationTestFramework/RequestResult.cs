// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestResult.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Represents the result of a simulated request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Represents the result of a simulated request
    /// </summary>
    public class RequestResult
    {
        /// <summary>
        /// Gets or sets the action executed context.
        /// </summary>
        /// <value>
        /// The action executed context.
        /// </value>
        public ActionExecutedContext ActionExecutedContext { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public HttpResponse Response { get; set; }

        /// <summary>
        /// Gets or sets the response text.
        /// </summary>
        /// <value>
        /// The response text.
        /// </value>
        public string ResponseText { get; set; }

        /// <summary>
        /// Gets or sets the result executed context.
        /// </summary>
        /// <value>
        /// The result executed context.
        /// </value>
        public ResultExecutedContext ResultExecutedContext { get; set; }
    }
}