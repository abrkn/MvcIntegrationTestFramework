// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterceptionFilterProvider.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Defines the InterceptionFilterProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// The interception filter used to store information about the last request before it is lost.
    /// </summary>
    internal class InterceptionFilterProvider : IFilterProvider
    {
        /// <summary>
        /// Returns an enumerator that contains all the <see cref="T:System.Web.Mvc.IFilterProvider"/> instances in the service locator.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>
        /// The enumerator that contains all the <see cref="T:System.Web.Mvc.IFilterProvider"/> instances in the service locator.
        /// </returns>
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            yield return new Filter(new InterceptionFilter(), FilterScope.Action, null);
        }
    }
}