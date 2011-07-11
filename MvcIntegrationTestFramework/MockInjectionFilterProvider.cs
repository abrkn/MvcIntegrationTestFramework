// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockInjectionFilterProvider.cs" company="Public">
//   Free
// </copyright>
// <summary>
//   Defines the MockInjectionFilterProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MvcIntegrationTestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    ///   A filter provider allowing the user to execute a specified action before each controller action.
    /// </summary>
    public class MockInjectionFilterProvider : IFilterProvider
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "MockInjectionFilterProvider" /> class.
        /// </summary>
        /// <param name = "action">The action to execute before each action. If the action returns false, this filter becomes disabled.</param>
        public MockInjectionFilterProvider(Func<ActionExecutingContext, bool> action)
        {
            this.Action = action;
        }

        /// <summary>
        ///   Gets or sets the action.
        /// </summary>
        /// <value>
        ///   The action.
        /// </value>
        public Func<ActionExecutingContext, bool> Action { get; set; }

        /// <summary>
        ///   Returns an enumerator that contains all the <see cref = "T:System.Web.Mvc.IFilterProvider" /> instances in the service locator.
        /// </summary>
        /// <param name = "controllerContext">The controller context.</param>
        /// <param name = "actionDescriptor">The action descriptor.</param>
        /// <returns>
        ///   The enumerator that contains all the <see cref = "T:System.Web.Mvc.IFilterProvider" /> instances in the service locator.
        /// </returns>
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            yield return new Filter(new ActionFilterAttribute { Action = this.Action }, FilterScope.Action, null);
        }

        /// <summary>
        /// The action filter used to override OnActionExecuting.
        /// </summary>
        private sealed class ActionFilterAttribute : System.Web.Mvc.ActionFilterAttribute
        {
            /// <summary>
            ///   Gets or sets whether the action filter is disabled.
            /// </summary>
            private bool disabled;

            /// <summary>
            ///   Gets or sets the action.
            /// </summary>
            /// <value>
            ///   The action.
            /// </value>
            public Func<ActionExecutingContext, bool> Action { get; set; }

            /// <summary>
            ///   Called by the ASP.NET MVC framework before the action method executes.
            /// </summary>
            /// <param name = "filterContext">The filter context.</param>
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (this.disabled)
                {
                    return;
                }

                if (!this.Action(filterContext))
                {
                    this.disabled = true;
                }
            }
        }
    }
}