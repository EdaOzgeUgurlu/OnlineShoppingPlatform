using Microsoft.AspNetCore.Http;
using OnlineShopping.Business.Operations.Order;
using OnlineShopping.Business.Operations.Product;
using OnlineShopping.Business.Operations.Setting;
using OnlineShopping.Business.Operations.User;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Authentication;

namespace OnlineShopping.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception is InvalidCredentialsException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return context.Response.WriteAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                }.ToString());
            }
            else if (exception is OrderException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                }.ToString());
            }
            else if (exception is OrderProductException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                return context.Response.WriteAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                }.ToString());
            }
            else if (exception is ProductException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                }.ToString());
            }
            else if (exception is SettingException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                }.ToString());
            }
            else if (exception is UserException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                }.ToString());
            }
            else if (exception is OrderDeleteException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return context.Response.WriteAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                }.ToString());
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware."
            }.ToString());
        }
    }
}