// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameValueCollectionConversions.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Defines the NameValueCollectionConversions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Web.Routing;

    /// <summary>
    /// Helper class to convert to and from a <see cref="System.Collections.Specialized.NameValueCollection"/>.
    /// </summary>
    public static class NameValueCollectionConversions
    {
        /// <summary>
        /// Converts from name value collection to a dictionary.
        /// </summary>
        /// <param name="values">The values to convert from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static Dictionary<string, string> ConvertFromNameValueCollection(NameValueCollection values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            return values.AllKeys.ToDictionary(key => key, key => values[key]);
        }

        /// <summary>
        /// Converts from an object to a <see cref="System.Collections.Specialized.NameValueCollection"/>.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static NameValueCollection ConvertFromObject(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var nvc = new NameValueCollection();
            var dict = new RouteValueDictionary(value);

            foreach (var kvp in dict)
            {
                if (kvp.Value == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture, "The value for the item with key {0} is null.", kvp.Key), 
                        "value");
                }

                if (kvp.Value.GetType().Name.Contains("Anonymous"))
                {
                    var prefix = kvp.Key + ".";
                    foreach (var innerkvp in new RouteValueDictionary(kvp.Value))
                    {
                        nvc.Add(prefix + innerkvp.Key, innerkvp.Value.ToString());
                    }
                }
                else
                {
                    nvc.Add(kvp.Key, kvp.Value.ToString());
                }
            }

            return nvc;
        }
    }
}