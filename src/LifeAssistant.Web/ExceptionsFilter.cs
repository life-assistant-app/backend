using LifeAssistant.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LifeAssistant.Web;

public class ExceptionsFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = new ContentResult
        {
            StatusCode = GetStatusCode(context.Exception),
            Content = context.Exception.Message
        };
    }

    private int GetStatusCode(Exception contextException)
    {
        return contextException switch
        {
            EntityNotFoundException => 404,
            EntityStateException => 400,
            IllegalAccessException => 403,
            _ => 500
        };
    }
}