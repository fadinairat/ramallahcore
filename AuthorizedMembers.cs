using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Ramallah.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PEFAuth
{
    public class AuthorizedMembers : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session.GetString("mem_id") == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" }, { "area", "" } });
                return;
            }
            else if (filterContext.HttpContext.Session.GetString("completed") !="True")
            {
                
                //Prevent accessing jobs until user complete his profile
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Members" }, { "action", "Profile" }, { "area", "" } });
            }
        }
    }
}
