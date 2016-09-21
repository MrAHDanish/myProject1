using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
namespace Inspinia_MVC5_SeedProject
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
    public static class UrlHelperExtensions
    {
        public static string Action(this UrlHelper helper, string actionName, string controllerName, object routeValues, string protocol, bool defaultPort)
        {
            return Action(helper, actionName, controllerName, routeValues, protocol, null, defaultPort);
        }

        //public static string Truncate(this string value, int maxLength)
        //{
        //    if (string.IsNullOrEmpty(value)) return value;
        //    return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        //}

        public static string Timeago(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
        public static string Action(this UrlHelper helper, string actionName, string controllerName, object routeValues, string protocol, string hostName, bool defaultPort)
        {
            if (!defaultPort)
            {
                return helper.Action(actionName, controllerName, new RouteValueDictionary(routeValues), protocol, hostName);
            }

            string port = "80";
            if (protocol.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                port = "443";
            }

            Uri requestUrl = helper.RequestContext.HttpContext.Request.Url;
            string defaultPortRequestUrl = Regex.Replace(requestUrl.ToString(), @"(?<=:)\d+?(?=/)", port);
            Uri url = new Uri(new Uri(defaultPortRequestUrl, UriKind.Absolute), requestUrl.PathAndQuery);

            var requestContext = GetRequestContext(url);
            var urlHelper = new UrlHelper(requestContext, helper.RouteCollection);

            var values = new RouteValueDictionary(routeValues);
            values.Add("controller", controllerName);
            values.Add("action", actionName);

            return urlHelper.RouteUrl(null, values, protocol, hostName);
        }

        private static RequestContext GetRequestContext(Uri uri)
        {
            // Create a TextWriter with null stream as a backing stream 
            // which doesn't consume resources
            using (var writer = new StreamWriter(Stream.Null))
            {
                var request = new HttpRequest(
                    filename: string.Empty,
                    url: uri.ToString(),
                    queryString: string.IsNullOrEmpty(uri.Query) ? string.Empty : uri.Query.Substring(1));
                var response = new HttpResponse(writer);
                var httpContext = new HttpContext(request, response);
                var httpContextBase = new HttpContextWrapper(httpContext);
                return new RequestContext(httpContextBase, new RouteData());
            }
        }
    }
    public static class HMTLHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
        {
            string cssClass = "active";
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }
        public static MvcHtmlString JsMinify(
            this HtmlHelper helper, Func<object, object> markup)
        {
            string notMinifiedJs =
             (markup.DynamicInvoke(helper.ViewContext) ?? "").ToString();

            var minifier = new Minifier();
            var minifiedJs = minifier.MinifyJavaScript(notMinifiedJs, new CodeSettings
            {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            });
            return new MvcHtmlString(minifiedJs);
        }
    }
}
