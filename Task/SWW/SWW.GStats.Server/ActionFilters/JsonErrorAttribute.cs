using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace SWW.GStats.Server.ActionFilters
{
    public class JsonErrorAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context) {

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Hack to serialize to json
            var jsonError = Encoding.UTF8.GetBytes($"{{\"error\" : \"{context.Exception.Message}\"}}"); 
            context.HttpContext.Response.Body.Write(jsonError, 0, jsonError.Length);
            context.HttpContext.Response.ContentType = "application/json";

            context.ExceptionHandled = true;

        }
    }
}
