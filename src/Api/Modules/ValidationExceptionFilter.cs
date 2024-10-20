using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Modules;

public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .ToDictionary(error => error.PropertyName, error => error.ErrorMessage);

            context.Result = new BadRequestObjectResult(new { Errors = errors });
            context.ExceptionHandled = true;
        }
    }
}