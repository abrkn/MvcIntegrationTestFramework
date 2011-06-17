using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Routing;

namespace MvcIntegrationTestFramework
{
    public static class NameValueCollectionConversions
    {
        public static NameValueCollection ConvertFromObject(object anonymous)
        {
            var nvc = new NameValueCollection();
            var dict = new RouteValueDictionary(anonymous);

            foreach (var kvp in dict)
            {
                if (kvp.Value == null)
                {
                    throw new NullReferenceException(kvp.Key);
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

        public static Dictionary<string, string> ConvertFromNameValueCollection(NameValueCollection nvc)
        {
            if (nvc == null)
                return null;

            return nvc.AllKeys.ToDictionary(key => key, key => nvc[key]);
        }
    }
}
