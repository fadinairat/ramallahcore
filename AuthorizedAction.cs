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

namespace Ramallah
{
    public class AuthorizedAction : ActionFilterAttribute
    {        
        public AuthorizedAction()
        {
            //_context = new DataContext();
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session.GetString("id") == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Users" }, { "action", "UserLogin" }, { "area", "Control" } });
                return;
            }


            if(filterContext.HttpContext.Session.GetString("is_super_admin") == "True")
            {
                //Do Nothing 
                //User have full permission
            }
            //else if (filterContext.HttpContext.Session.GetString("type") == "Employer")
            //{
            //    //Employer Logged in
            //}
            //else if (filterContext.HttpContext.Session.GetString("type") == "Donor")
            //{
            //    //Donor Logged in
            //}
            //else if (filterContext.HttpContext.Session.GetString("type") == "Trainer")
            //{
            //    //Trainer Logged in
            //}            
            else if (filterContext.HttpContext.Session.GetString("permissions") != null)
            {
                var perms = JsonConvert.DeserializeObject<List<Permissions>>(filterContext.HttpContext.Session.GetString("permissions"));
                string controllerName = filterContext.RouteData.Values["controller"].ToString();
                string actionName = filterContext.RouteData.Values["action"].ToString();
                string url = "/" + controllerName + "/" + actionName;

                var reservedPerm = JsonConvert.DeserializeObject<List<Permissions>>(filterContext.HttpContext.Session.GetString("reserved"));
                bool resPerm = false;
                if (reservedPerm != null)
                {
                    resPerm = reservedPerm.Where(a => a.Reserved == true && a.Action.ToLower() == actionName.ToLower() && (a.Controller.ToLower() == controllerName.ToLower() || a.Controller.ToString() == "")).Any();
                }

                if(resPerm)
                {
                    //Dashboard access
                    //Allowed for all users
                    //Nothing to do
                }
                else 
                {
                    //Loop through user permissions to make sure that action and controller exists within permissions
                    bool exists = false;
                    foreach (Permissions perm in perms)
                    {
                        if (perm.Action.ToLower() == actionName.ToLower() && perm.Controller.ToLower() == controllerName.ToLower())
                        {
                            exists = true;
                            break;
                        }
                    }
                    //int count = perms.Where(s => s.Controller.ToLower() == controllerName.ToLower() && s.Action.ToLower() == actionName.ToLower()).Count();
                    if (!exists)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Users" }, { "action", "AccessDenied" }, { "area", "Control" } });
                        return;
                    }
                    //if (count == 0)
                    //{
                    //    filterContext.Result = new RedirectToRouteResult(
                    //    new RouteValueDictionary { { "controller", "Users" }, { "action", "AccessDenied" },{ "area", "Control" } });
                    //    return;
                    //}

                }
                //User have the required permission to access current page
            }
            else
            {
                //User Group Doesn't have any permission, so kick him out of Control
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Users" }, { "action", "UserLogin" },{ "area", "Control" } });
                return;
            }
        }
    }
}
