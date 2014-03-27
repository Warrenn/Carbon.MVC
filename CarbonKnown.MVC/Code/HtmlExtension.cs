using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace CarbonKnown.MVC.Code
{
    public static class HtmlExtension
    {
        public static string AbsoluteUrl(this HtmlHelper helper, string path)
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            var request = context.Request;
            var url = request.Url;
            if (url == null) return path;
            var urlBuilder =
                new UriBuilder(url.AbsoluteUri)
                    {
                        Path = UrlHelper.GenerateContentUrl(path, context)
                    };

            return urlBuilder.ToString();
        }

        public static string FormatNumber(this HtmlHelper helper, decimal amount)
        {
            return amount.ToString("#,##0.00", CultureInfo.CurrentCulture);
        }

        public static IHtmlString NgApp(this HtmlHelper helper)
        {
            var viewBag = helper.ViewBag;
            var appDirective =
                string.IsNullOrEmpty(viewBag.ngApp)
                    ? string.Empty
                    : string.Format(" xmlns:ng=\"http://angularjs.org\" ng-app=\"{0}\" id=\"ng-app\" ", viewBag.ngApp);
            return helper.Raw(appDirective);
        }

        public static string ActiveClass(
            this HtmlHelper helper,
            string key,
            string element,
            string activeClass = "active")
        {
            var viewData = helper.ViewData;
            var state = viewData[key] as string;
            return string.Equals(state, element, StringComparison.InvariantCultureIgnoreCase)
                       ? activeClass
                       : string.Empty;
        }

        public static string ActiveClass<T>(
            this HtmlHelper helper,
            string key,
            T element,
            string activeClass = "active") where T : struct
        {
            var viewData = helper.ViewData;
            var check = (T) viewData[key];
            return (element.Equals(check)) ? activeClass : string.Empty;
        }

        public static IHtmlString NgCtrl(this HtmlHelper helper)
        {
            var viewBag = helper.ViewBag;
            var appDirective =
                string.IsNullOrEmpty(viewBag.ngBodyCtrl)
                    ? string.Empty
                    : string.Format("ng-controller=\"{0}\" ", viewBag.ngBodyCtrl);
            return helper.Raw(appDirective);
        }

        public static string GetName<T>(this HtmlHelper helper, T enumValue) where T : struct
        {
            return Enum.GetName(typeof (T), enumValue);
        }
    }
}