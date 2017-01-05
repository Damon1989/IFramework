﻿using IFramework.SysExceptions;
using IFramework.SysExceptions.ErrorCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Http.Results;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http.Controllers;
using IFramework.Infrastructure;

namespace IFramework.AspNet
{
    public class ApiControllerBase : ApiController
    {

        protected string GetModelErrorMessage()
        {
            return string.Join(",", ModelState.Values
                                              .SelectMany(v => v.Errors
                                                                .Select(e => $"error:{e.ErrorMessage} exception:{e.Exception?.GetBaseException()?.Message}")));
        }

        #region process wrapping
        protected ApiResult<T> Process<T>(Func<T> func, bool needRetry = true)
        {
            if (ModelState.IsValid)
            {
                var apiResult = ExceptionManager.Process<T>(func, needRetry);
                return apiResult;
            }
            else
            {
                return
                    new ApiResult<T>
                    (
                        ErrorCode.InvalidParameters,
                        GetModelErrorMessage()
                    );
            }
        }
        protected ApiResult Process(Action action, bool needRetry = true)
        {
            if (ModelState.IsValid)
            {
                var apiResult = ExceptionManager.Process(action, needRetry);
                return apiResult;
            }
            else
            {
                return
                    new ApiResult
                    (
                        ErrorCode.InvalidParameters,
                        GetModelErrorMessage()

                    );
            }
        }
        List<CookieHeaderValue> _cookies;
        protected void AddCookies(params CookieHeaderValue[] cookies)
        {
            if (_cookies == null)
            {
                _cookies = new List<CookieHeaderValue>();
            }
            _cookies.AddRange(cookies);
        }

        protected string TryGetCookie(string key, string defaultValue)
        {
            try
            {
                var value = defaultValue;
                CookieHeaderValue cookie = Request.Headers.GetCookies(key).FirstOrDefault();
                if (cookie != null)
                {
                    value = cookie[key].Value;
                }
                return value;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        protected void RemoveCookies(string key)
        {
            var cookies = Request.Headers.GetCookies(key).ToList();
            cookies.ForEach(c => c.Expires = DateTimeOffset.Now.AddDays(-1));
            AddCookies(cookies.ToArray());
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(controllerContext, cancellationToken)
                .ContinueWith(t =>
                {
                    if (_cookies != null && _cookies.Count > 0)
                    {
                        t.Result.Headers.AddCookies(_cookies);
                    }
                    return t.Result;
                }); ;
        }


        protected async Task<ApiResult> ProcessAsync(Func<Task> func, bool continueOnCapturedContext = false, bool needRetry = true)
        {
            if (ModelState.IsValid)
            {
                return await ExceptionManager.ProcessAsync(func, continueOnCapturedContext, needRetry).ConfigureAwait(continueOnCapturedContext);
            }
            else
            {
                return
                    new ApiResult
                    (
                        ErrorCode.InvalidParameters,
                        GetModelErrorMessage()

                    );
            }
        }

        protected async Task<ApiResult<T>> ProcessAsync<T>(Func<Task<T>> func, bool continueOnCapturedContext = false, bool needRetry = true)
        {
            if (ModelState.IsValid)
            {
                return await ExceptionManager.ProcessAsync(func, continueOnCapturedContext, needRetry).ConfigureAwait(continueOnCapturedContext);
            }
            else
            {
                return
                   new ApiResult<T>
                   (
                      ErrorCode.InvalidParameters,
                      GetModelErrorMessage()
                   );
            }

        }

        protected string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}