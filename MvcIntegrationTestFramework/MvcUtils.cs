// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MvcUtils.cs" company="Publc">
//   Free
// </copyright>
// <summary>
//   Defines the MvcUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper methods to test ASP.NET MVC projects.
    /// </summary>
    public static class MvcUtils
    {
        /// <summary>
        /// Extracts the anti forgery token from the specified html.
        /// </summary>
        /// <param name="html">The HTML response text.</param>
        /// <returns>The anti forgery token.</returns>
        public static string ExtractAntiForgeryToken(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }

            const string Pattern =
                @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>";

            var match = Regex.Match(html, Pattern);
            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }
    }
}