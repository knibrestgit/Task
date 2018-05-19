using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace SWW.GStats.Server.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NullToNotFoundAttribute : Attribute, IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            var objResult = context.Result as ObjectResult;
            if (objResult.Value == null) {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            
        }
    }
}
