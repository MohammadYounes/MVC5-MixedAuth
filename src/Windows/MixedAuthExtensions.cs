// <copyright file="MixedAuthExtensions.cs" author="Mohammad Younes">
// Copyright 2013 Mohammad Younes.
// 
// Released under the MIT license
// http://opensource.org/licenses/MIT
//
// </copyright>


using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Web;
using System.Web.SessionState;

namespace MixedAuth
{
    public enum Action { Login, Link, Logoff };

    public static class MixedAuthExtensions
    {
        const string userIdKey = "windows.userId";
        //http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
        const int fakeStatusCode = 418;

        const string controllerName = "Account";
        const string loginActionName = "WindowsLogin";
        const string linkActionName = "LinkWindowsLogin";
        const string logoffActionName = "WindowsLogoff";
        const string windowsLoginRouteName = "Windows/Login";


        public static void RegisterWindowsAuthentication(this MvcApplication app)
        {
            app.EndRequest += (object sender, EventArgs e) =>
            {
                HttpContext.Current.ApplyChallenge();
            };
        }

        /// <summary>
        /// Registers ignore route for the managed handler.
        /// </summary>
        /// <param name="routes"></param>
        public static void IgnoreWindowsLoginRoute(this RouteCollection routes)
        {
            routes.IgnoreRoute(windowsLoginRouteName);
        }

        /// <summary>
        /// By pass all middleware and modules, by setting a fake status code.
        /// </summary>
        /// <param name="context"></param>
        public static void RequestChallenge(this HttpContext context)
        {
            context.Response.StatusCode = fakeStatusCode;
        }

        /// <summary>
        /// Invoke on end response only. Replaces the current response status code with 401.2
        /// </summary>
        /// <param name="context"></param>
        public static void ApplyChallenge(this HttpContext context)
        {
            if (context.Response.StatusCode == fakeStatusCode)
            {
                context.Response.StatusCode = 401;
                context.Response.SubStatusCode = 2;

                //http://msdn.microsoft.com/en-us/library/system.web.httpresponse.tryskipiiscustomerrors(v=vs.110).aspx
                //context.Response.TrySkipIisCustomErrors = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static RouteData CreateRouteData(this WindowsLoginHandler handler, Action action)
        {
            RouteData routeData = new RouteData();
            routeData.RouteHandler = new MvcRouteHandler();

            switch (action)
            {
                case Action.Login:
                    routeData.Values.Add("controller", controllerName);
                    routeData.Values.Add("action", loginActionName);
                    break;
                case Action.Link:
                    routeData.Values.Add("controller", controllerName);
                    routeData.Values.Add("action", linkActionName);
                    break;
                case Action.Logoff:
                    routeData.Values.Add("controller", controllerName);
                    routeData.Values.Add("action", logoffActionName);
                    break;
                default:
                    throw new NotSupportedException(string.Format("unknonw action value '{0}'.", action));
            }
            return routeData;
        }


        /// <summary>
        /// Saves userId to the items collection inside <see cref="HttpContext"/>.
        /// </summary>
        public static void SaveUserIdToContext(this WindowsLoginHandler handler, string userId)
        {
            if (handler.Context.Items.Contains(userIdKey))
                throw new ApplicationException("Id already exists in context.");

            handler.Context.Items.Add("windows.userId", userId);
        }

        /// <summary>
        /// Reads userId from item collection inside <see cref="HttpContext"/>.
        /// </summary>
        /// <remarks>The item will removed before this method returns</remarks>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string ReadUserId(this HttpContextBase context)
        {
            if (!context.Items.Contains(userIdKey))
                throw new ApplicationException("Id not found in context.");

            string userId = context.Items[userIdKey] as string;
            context.Items.Remove(userIdKey);

            return userId;
        }

        /// <summary>
        /// Returns true if the session contains an entry for userId.
        /// </summary>
        public static bool SessionHasUserId(this WindowsLoginHandler handler)
        {
            return handler.Context.Session[userIdKey] != null;
        }

        /// <summary>
        /// Save a session-state value with the specified userId.
        /// </summary>
        public static void SaveUserIdToSession(this WindowsLoginHandler handler, string userId)
        {
            if (handler.SessionHasUserId())
                throw new ApplicationException("Id already exists in session.");

            handler.Context.Session[userIdKey] = userId;
        }

        /// <summary>
        /// Reads userId value from session-state.
        /// </summary>
        /// <remarks>The session-state value removed before this method returns.</remarks>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string ReadUserIdFromSession(this WindowsLoginHandler handler)
        {
            string userId = handler.Context.Session[userIdKey] as string;

            if (string.IsNullOrEmpty(userIdKey))
                throw new ApplicationException("Id not found in session.");

            handler.Context.Session.Remove(userIdKey);

            return userId;
        }


        /// <summary>
        /// Creates a form for windows login, simulating external login providers.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcForm BeginWindowsAuthForm(this HtmlHelper htmlHelper, object htmlAttributes)
        {
            return htmlHelper.BeginForm("Login", "Windows", FormMethod.Post, htmlAttributes);
        }

        /// <summary>
        /// Creates a form for windows login, simulating external login providers.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcForm BeginWindowsAuthForm(this HtmlHelper htmlHelper, object routeValues, object htmlAttributes)
        {
            return htmlHelper.BeginForm("Login", "Windows", FormMethod.Post, htmlAttributes);
        }



    }
}