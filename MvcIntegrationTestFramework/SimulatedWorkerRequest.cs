// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimulatedWorkerRequest.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Defines the SimulatedWorkerRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Hosting;

    /// <summary>
    /// A simulated worker request.
    /// </summary>
    internal class SimulatedWorkerRequest : SimpleWorkerRequest
    {
        /// <summary>
        /// Simulated cookies.
        /// </summary>
        private readonly HttpCookieCollection cookies;

        /// <summary>
        /// Simulated form values.
        /// </summary>
        private readonly NameValueCollection formValues;

        /// <summary>
        /// Simulated headers.
        /// </summary>
        private readonly NameValueCollection headers;

        // TODO: There is already a HttpVerb class somewhere in the BCL.

        /// <summary>
        /// The HTTP verb, such as GET or POST.
        /// </summary>
        private readonly string httpVerbName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedWorkerRequest"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="query">The query.</param>
        /// <param name="output">The output.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="httpVerbName">Name of the HTTP verb.</param>
        /// <param name="formValues">The form values.</param>
        /// <param name="headers">The headers.</param>
        public SimulatedWorkerRequest(
            string page, 
            string query, 
            TextWriter output, 
            HttpCookieCollection cookies, 
            string httpVerbName, 
            NameValueCollection formValues, 
            NameValueCollection headers)
            : base(page, query, output)
        {
            this.cookies = cookies;
            this.httpVerbName = httpVerbName;
            this.formValues = formValues;
            this.headers = headers;
        }

        /// <summary>
        /// Returns the HTTP request verb.
        /// </summary>
        /// <returns>
        /// The HTTP verb for this request.
        /// </returns>
        public override string GetHttpVerbName()
        {
            return this.httpVerbName;
        }

        /// <summary>
        /// Returns the standard HTTP request header that corresponds to the specified index.
        /// </summary>
        /// <param name="index">The index of the header. For example, the <see cref="F:System.Web.HttpWorkerRequest.HeaderAllow"/> field.</param>
        /// <returns>
        /// The HTTP request header.
        /// </returns>
        public override string GetKnownRequestHeader(int index)
        {
            // Override "Content-Type" header for POST requests, otherwise ASP.NET won't read the Form collection
            if (index == 12)
            {
                if (string.Equals(this.httpVerbName, "post", StringComparison.OrdinalIgnoreCase))
                {
                    return "application/x-www-form-urlencoded";
                }
            }

            switch (index)
            {
                case 0x19:
                    return this.MakeCookieHeader();
                default:
                    return this.headers == null ? null : this.headers[GetKnownRequestHeaderName(index)];
            }
        }

        /// <summary>
        /// Returns the portion of the HTTP request body that has already been read.
        /// </summary>
        /// <returns>
        /// The portion of the HTTP request body that has been read.
        /// </returns>
        public override byte[] GetPreloadedEntityBody()
        {
            if (this.formValues == null)
            {
                return base.GetPreloadedEntityBody();
            }

            var sb = new StringBuilder();
            foreach (string key in this.formValues)
            {
                sb.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(this.formValues[key]));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        /// <summary>
        /// Returns a nonstandard HTTP request header value.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <returns>
        /// The header value.
        /// </returns>
        public override string GetUnknownRequestHeader(string name)
        {
            return this.headers == null ? null : this.headers[name];
        }

        /// <summary>
        /// Get all nonstandard HTTP header name-value pairs.
        /// </summary>
        /// <returns>
        /// An array of header name-value pairs.
        /// </returns>
        public override string[][] GetUnknownRequestHeaders()
        {
            if (this.headers == null)
            {
                return null;
            }

            var unknownHeaders = from key in this.headers.Keys.Cast<string>()
                                 let knownRequestHeaderIndex = GetKnownRequestHeaderIndex(key)
                                 where knownRequestHeaderIndex < 0
                                 select new[] { key, this.headers[key] };
            return unknownHeaders.ToArray();
        }

        /// <summary>
        /// Creates a cookie header from this instance's cookie collection.
        /// </summary>
        /// <returns>The cookie header value.</returns>
        private string MakeCookieHeader()
        {
            if ((this.cookies == null) || (this.cookies.Count == 0))
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (string cookieName in this.cookies)
            {
                sb.AppendFormat("{0}={1};", cookieName, this.cookies[cookieName].Value);
            }

            return sb.ToString();
        }
    }
}