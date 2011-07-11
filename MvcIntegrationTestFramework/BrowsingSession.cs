// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowsingSession.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Defines the BrowsingSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;

    /// <summary>
    /// The browsing session.
    /// </summary>
    public class BrowsingSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsingSession"/> class.
        /// </summary>
        public BrowsingSession()
        {
            this.Cookies = new HttpCookieCollection();
        }

        /// <summary>
        /// Gets the cookies.
        /// </summary>
        public HttpCookieCollection Cookies { get; private set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        public HttpSessionState Session { get; private set; }

        /// <summary>
        /// GETs the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The result from of the GET request.</returns>
        public RequestResult Get(string url)
        {
            return this.ProcessRequest(url, HttpVerbs.Get, new NameValueCollection());
        }

        /// <summary>
        /// Sends a post to your url. Url should NOT start with a /
        /// </summary>
        /// <param name="url">The URL to post to.</param>
        /// <param name="formData">The form data to post, for example <code>new { A = 123, B = "Hello" }</code>.</param>
        /// <returns>The result of executing the POST request.</returns>
        /// <example>
        ///   <code>
        /// var result = Post("registration/create", new
        /// {
        ///   Form = new
        ///   {
        ///     InvoiceNumber = "10000",
        ///     AmountDue = "10.00",
        ///     Email = "chriso@innovsys.com",
        ///     Password = "welcome",
        ///     ConfirmPassword = "welcome"
        ///   }
        /// });
        /// </code>
        /// </example>
        public RequestResult Post(string url, object formData)
        {
            var formNameValueCollection = NameValueCollectionConversions.ConvertFromObject(formData);
            return this.ProcessRequest(url, HttpVerbs.Post, formNameValueCollection);
        }

        /// <summary>
        /// Adds any new cookies to cookie collection.
        /// </summary>
        private void AddAnyNewCookiesToCookieCollection()
        {
            if (LastRequestData.Response == null)
            {
                return;
            }

            var lastResponseCookies = LastRequestData.Response.Cookies;

            foreach (string cookieName in lastResponseCookies)
            {
                var cookie = lastResponseCookies[cookieName];

                if (this.Cookies[cookieName] != null)
                {
                    this.Cookies.Remove(cookieName);
                }

                if ((cookie.Expires == default(DateTime)) || (cookie.Expires > DateTime.Now))
                {
                    this.Cookies.Add(cookie);
                }
            }
        }

        /// <summary>
        /// Processes the specified request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="formValues">The form values.</param>
        /// <returns>The request of executing the simulate request.</returns>
        private RequestResult ProcessRequest(
            string url, HttpVerbs httpVerb = HttpVerbs.Get, NameValueCollection formValues = null)
        {
            return this.ProcessRequest(url, httpVerb, formValues, null);
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="httpVerb">The HTTP verb.</param>
        /// <param name="formValues">The form values.</param>
        /// <param name="headers">The headers.</param>
        /// <returns>The request of executing the simulate request.</returns>
        private RequestResult ProcessRequest(
            string url, HttpVerbs httpVerb, NameValueCollection formValues, NameValueCollection headers)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            // TODO: Doesn't the BCL contain methods to do all this URL handling already?

            // Fix up URLs that incorrectly start with / or ~/
            // TODO: Why is this incorrect? They can be handled fine and make the library easier to use by avoiding unecessary restrictions.
            if (url.StartsWith("~/", true, CultureInfo.InvariantCulture))
            {
                url = url.Substring(2);
            }
            else if (url.StartsWith("/", true, CultureInfo.InvariantCulture))
            {
                url = url.Substring(1);
            }

            // Parse out the querystring if provided
            var query = string.Empty;
            var querySeparatorIndex = url.IndexOf("?");

            if (querySeparatorIndex >= 0)
            {
                query = url.Substring(querySeparatorIndex + 1);
                url = url.Substring(0, querySeparatorIndex);
            }

            // Perform the request
            LastRequestData.Reset();
            var output = new StringWriter();
            var httpVerbName = httpVerb.ToString().ToLower();
            var workerRequest = new SimulatedWorkerRequest(
                url, query, output, this.Cookies, httpVerbName, formValues, headers);
            HttpRuntime.ProcessRequest(workerRequest);

            // Capture the output
            this.AddAnyNewCookiesToCookieCollection();
            this.Session = LastRequestData.HttpSessionState;
            return new RequestResult
                {
                    ResponseText = output.ToString(), 
                    ActionExecutedContext = LastRequestData.ActionExecutedContext, 
                    ResultExecutedContext = LastRequestData.ResultExecutedContext, 
                    Response = LastRequestData.Response, 
                };
        }
    }
}