// <copyright file="WindowsLoginHandler.cs" auther="Mohammad Younes">
// Copyright 2013 Mohammad Younes.
// 
// Released under the MIT license
// http://opensource.org/licenses/MIT
//
// </copyright>

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MixedAuth.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MixedAuth
{

  /// <summary>
  /// Managed handler for windows authentication.
  /// </summary>
  public class WindowsLoginHandler : HttpTaskAsyncHandler, System.Web.SessionState.IRequiresSessionState
  {
    public HttpContext Context { get; set; }
    public override async Task ProcessRequestAsync(HttpContext context)
    {
      this.Context = context;

      //if user is already authenticated, LogonUserIdentity will be holding the current application pool identity.
      //to overcome this:
      //1. save userId to session.
      //2. log user off.
      //3. request challenge.
      //4. log user in.

      if (context.User.Identity.IsAuthenticated)
      {
        this.SaveUserIdToSession(context.User.Identity.GetUserId());

        await WinLogoffAsync(context);

        context.RequestChallenge();
      }
      else if (!context.Request.LogonUserIdentity.IsAuthenticated)
      {
        context.RequestChallenge();
      }
      else
      {
        // true: user is trying to link windows login to an existing account
        if (this.SessionHasUserId())
        {
          var userId = this.ReadUserIdFromSession();
          this.SaveUserIdToContext(userId);
          await WinLinkLoginAsync(context);
        }
        else // normal login.
          await WinLoginAsync(context);
      }
    }

    #region helpers
    /// <summary>
    /// Executes Windows login action against account controller.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task WinLoginAsync(HttpContext context)
    {
      var routeData = this.CreateRouteData(Action.Login);

      routeData.Values.Add("returnUrl", context.Request["returnUrl"]);
      routeData.Values.Add("userName", context.Request.Form["UserName"]);

      await ExecuteController(context, routeData);
    }

    /// <summary>
    /// Execute Link Windows login action against account controller.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task WinLinkLoginAsync(HttpContext context)
    {
      var routeData = this.CreateRouteData(Action.Link);

      await ExecuteController(context, routeData);
    }

    /// <summary>
    /// Executes Windows logoff action against controller.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task WinLogoffAsync(HttpContext context)
    {
      var routeData = this.CreateRouteData(Action.Logoff);

      await ExecuteController(context, routeData);
    }

    /// <summary>
    /// Executes controller based on route data.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="routeData"></param>
    /// <returns></returns>
    private async Task ExecuteController(HttpContext context, RouteData routeData)
    {
      var wrapper = new HttpContextWrapper(context);
      MvcHandler handler = new MvcHandler(new RequestContext(wrapper, routeData));

      IHttpAsyncHandler asyncHandler = ((IHttpAsyncHandler)handler);
      await Task.Factory.FromAsync(asyncHandler.BeginProcessRequest, asyncHandler.EndProcessRequest, context, null);
    }

    #endregion
  }
}
