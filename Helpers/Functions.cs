using HtmlAgilityPack;
using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Ramallah.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ramallah.Helpers
{
    public class Functions
    {
        public static bool ValidateAPIAccess(HttpContext HContext,out string Message)
        {
            const string APIKEY = "APIKEY";
            //var data = new { result = true, message = "" };
            if (!HContext.Request.Headers.TryGetValue(APIKEY, out
                   var extractedApiKey))
            {
                HContext.Response.StatusCode = 401;
                //data = new { result = false, message = "Api Key was not provided" };
                Message = "Api Key was not provided";
                return false;
            }
            var appSettings = HContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEY);
            if (apiKey == null || !apiKey.Equals(extractedApiKey))
            {
                HContext.Response.StatusCode = 401;
                //data = new { result = false, message = "Unauthorized client" };
                Message = "Unauthorized client";
                return false;
            }
            Message = "";
            return true;
        }
        public static int CalculateAge(DateTime Dob)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - Dob.Year;

            if (now.Month < Dob.Month || (now.Month == Dob.Month && now.Day < Dob.Day))
            {
                age--;
            }
            return age;
        }

        public static int CalculateMonths(DateTime date1, DateTime date2)
        {            
            TimeSpan difference = date2 - date1; // TimeSpan object
            int totalDays = difference.Days; // total number of days
            double averageDaysInMonth = 365.2425 / 12; // average number of days in a month

            int resultInMonths = (int)Math.Round(totalDays / averageDaysInMonth); // round to nearest integer

            return resultInMonths;
        }
        public static string RemoveHTMLTagsCharArray(string html)
        {
            char[] charArray = new char[html.Length];
            int index = 0;
            bool isInside = false;

            for (int i = 0; i < html.Length; i++)
            {
                char left = html[i];

                if (left == '<')
                {
                    isInside = true;
                    continue;
                }

                if (left == '>')
                {
                    isInside = false;
                    continue;
                }

                if (!isInside)
                {
                    charArray[index] = left;
                    index++;
                }
            }

            return new string(charArray, 0, index);
        }
        public static string RemoveHtml(string? htmlContent)
        {
            if (htmlContent == null) return string.Empty;

            //Remove all the html tags
            htmlContent = Regex.Replace(htmlContent, "<.*?>", string.Empty);
            htmlContent = RemoveHTMLTagsCharArray(htmlContent);

            // Decode HTML entities
            htmlContent = System.Net.WebUtility.HtmlDecode(htmlContent);
            return htmlContent;
        }
        public static string FilterHtml(string? htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent)) return string.Empty;

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var acceptableTags = new System.String[] { "p", "b", "i", "u", "ul", "ol", "li", "a", "table", "h1", "h2", "h3", "h4", "h5", "h6", "span", "div", "blockquote", "img", "video", "audio", "button", "strong", "em" };

            var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
                {
                    var childNodes = node.SelectNodes("./*|./text()");

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }
                    parentNode.RemoveChild(node);
                }
            }

            return document.DocumentNode.InnerHtml;
        }

        public static bool CheckAccess(string actionName, string controllerName, string area, HttpContext httpContext)
        {
            //return true;
            if (httpContext.Session.GetString("is_super_admin") != null && httpContext.Session.GetString("is_super_admin") == "True")
            {
                return true;
            }
            else if (httpContext.Session.GetString("permissions") != null)
            {
                var perms = JsonConvert.DeserializeObject<List<Permissions>>(httpContext.Session.GetString("permissions"));
                string url = "/" + controllerName + "/" + actionName;

                var reservedPerm = JsonConvert.DeserializeObject<List<Permissions>>(httpContext.Session.GetString("reserved"));
                bool resPerm = false;
                if (reservedPerm != null)
                {
                    resPerm = reservedPerm.Where(a => a.Reserved == true && a.Action.ToLower() == actionName.ToLower() && (a.Controller.ToLower() == controllerName.ToLower() || a.Controller.ToString() == "")).Any();
                }

                if (resPerm)
                {
                    //Dashboard access
                    //Allowed for all users
                    //Nothing to do
                    return true;
                }
                else
                {
                    //Loop through user permissions to make sure that action and controller exists within permissions
                    bool exists = false;
                    foreach(Permissions perm in perms)
                    {
                        if(perm.Action.ToLower() == actionName.ToLower() && perm.Controller.ToLower() == controllerName.ToLower())
                        {
                            exists = true;
                            break;
                        }
                    }                
                    //int count = perms.Where(s => s.Permissions.Controller.ToLower() == controllerName.ToLower() && s.Permissions.Action.ToLower() == actionName.ToLower()).Count();
                    //if(count == 0 ) return false;
                    return exists;
                }
            }
            else
            {
                return false;
            }
            //Permitted
            return true;
        }
    }
}
