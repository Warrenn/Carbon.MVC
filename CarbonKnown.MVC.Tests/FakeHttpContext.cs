using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Moq;

namespace CarbonKnown.MVC.Tests
{
    public class FakeHttpContext
    {
        public readonly Mock<HttpContextBase> Context;
        public readonly Mock<HttpRequestBase> Request;
        public readonly Mock<HttpResponseBase> Response;
        public readonly Mock<HttpSessionStateBase> Session;
        public readonly Mock<HttpServerUtilityBase> Server;

        public FakeHttpContext(
            NameValueCollection form = null,
            HttpCookieCollection cookies = null,
            GenericPrincipal user = null,
            Uri url = null,
            string httpMethod = null,
            Stream outputStream = null,
            Action
                <Mock<HttpRequestBase>, Mock<HttpResponseBase>, Mock<HttpSessionStateBase>, Mock<HttpServerUtilityBase>>
                configureAction = null)
        {
            Context = new Mock<HttpContextBase>();
            Request = new Mock<HttpRequestBase>();
            Response = new Mock<HttpResponseBase>();
            Session = new Mock<HttpSessionStateBase>();
            Server = new Mock<HttpServerUtilityBase>();

            if (url != null)
            {
                Request.Setup(req => req.QueryString)
                       .Returns(GetQueryStringParameters(url.Query));
                Request.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                       .Returns(GetUrlFileName(url.PathAndQuery));
                Request.Setup(req => req.PathInfo)
                       .Returns(String.Empty);
            }
            if (!String.IsNullOrEmpty(httpMethod))
            {
                Request
                    .Setup(req => req.HttpMethod)
                    .Returns(httpMethod);
            }
            if (cookies != null)
            {
                Request.Setup(r => r.Cookies).Returns(cookies);
            }
            if (form != null)
            {
                Request.Setup(r => r.Form).Returns(form);
            }
            if (user != null)
            {
                Context.Setup(u => u.User).Returns(user);
            }
            if (configureAction != null)
            {
                configureAction(Request, Response, Session, Server);
            }
            if (outputStream != null)
            {
                Response.SetupGet(res => res.OutputStream).Returns(outputStream);
            }

            Context.Setup(ctx => ctx.Request).Returns(Request.Object);
            Context.Setup(ctx => ctx.Response).Returns(Response.Object);
            Context.Setup(ctx => ctx.Session).Returns(Session.Object);
            Context.Setup(ctx => ctx.Server).Returns(Server.Object);
        }

        public HttpContextBase Object
        {
            get { return Context.Object; }
        }

        public static string GetUrlFileName(string url)
        {
            return url.Contains("?") ? url.Substring(0, url.IndexOf("?", StringComparison.Ordinal)) : url;
        }

        public static NameValueCollection GetQueryStringParameters(string url)
        {
            if (url.Contains("?"))
            {
                var parameters = new NameValueCollection();

                var parts = url.Split("?".ToCharArray());
                var keys = parts[1].Split("&".ToCharArray());

                foreach (var part in keys.Select(key => key.Split("=".ToCharArray())))
                {
                    parameters.Add(part[0], part[1]);
                }

                return parameters;
            }
            return null;
        }
    }
}
