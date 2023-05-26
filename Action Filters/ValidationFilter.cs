namespace ForexFintechAPI.Action_Filters;

using FluentValidation;
using ForexFintechAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

//public class ValidationFilter : ActionFilterAttribute
//{
//    private readonly PartnerDtoValidator _partnerDtoValidator;
//    private readonly ExchangeManipulationDtoValidator _exchangeManipulationValidator;
//    private readonly RegisterDtoValidator _registerViewModelValidator;
//    private readonly LoginDtoValidator _loginViewModelValidator;

//    public ValidationFilter(PartnerDtoValidator partnerDtoValidator,
//                                                ExchangeManipulationDtoValidator exchangeManipulationValidator,
//                                                LoginDtoValidator loginViewModelValidator,
//                                                RegisterDtoValidator registerViewModelValidator)
//    {
//        _partnerDtoValidator = partnerDtoValidator;
//        _exchangeManipulationValidator = exchangeManipulationValidator;
//        _loginViewModelValidator = loginViewModelValidator;
//        _registerViewModelValidator = registerViewModelValidator;
//    }

//    public override void OnActionExecuting(ActionExecutingContext context)
//    {
//        // Validate IEntity
//        var entityParam = context.ActionArguments.SingleOrDefault(p => p.Value != null && p.Value.GetType().Name.EndsWith("Dto"));
//        if (entityParam.Value == null)
//        {
//            context.Result = new BadRequestObjectResult("Object is null");
//            return;
//        }
//        // Perform FluentValidation
//        if (entityParam.Value is PartnerDto partnerDto)
//        {
//            var partnerValidationResult = _partnerDtoValidator.Validate(partnerDto);
//            if (!partnerValidationResult.IsValid)
//            {
//                List<string> errorMessages = partnerValidationResult.Errors
//                                                                    .Select(error => error.ErrorMessage)
//                                                                    .ToList();
//                context.Result = new BadRequestObjectResult(errorMessages);
//                return;
//            }
//        }
//        else if (entityParam.Value is ExchangeManipulationDataDto exchangeManipulationDataDto)
//        {
//            var exchangeValidationResult = _exchangeManipulationValidator.Validate(exchangeManipulationDataDto);
//            if (!exchangeValidationResult.IsValid)
//            {
//                List<string> errorMessages = exchangeValidationResult.Errors
//                                                                     .Select(error => error.ErrorMessage)
//                                                                     .ToList();
//                context.Result = new BadRequestObjectResult(errorMessages);
//                return;
//            }
//        }
//        else if (entityParam.Value is RegisterDto registerDto)
//        {
//            var registerValidationResult = _registerViewModelValidator.Validate(registerDto);
//            if (!registerValidationResult.IsValid)
//            {
//                List<string> errorMessages = registerValidationResult.Errors
//                                                                     .Select(error => error.ErrorMessage)
//                                                                     .ToList();

//                context.Result = new BadRequestObjectResult(errorMessages);
//                return;
//            }
//        }
//        else if (entityParam.Value is LoginDto loginDto)
//        {
//            var loginValidationResult = _loginViewModelValidator.Validate(loginDto);
//            if (!loginValidationResult.IsValid)
//            {
//                List<string> errorMessages = loginValidationResult.Errors
//                                                                     .Select(error => error.ErrorMessage)
//                                                                     .ToList();

//                context.Result = new BadRequestObjectResult(errorMessages);
//                return;
//            }
//        }

//        // Validate ModelState
//        if (!context.ModelState.IsValid)
//        {
//            context.Result = new BadRequestObjectResult(context.ModelState);
//        }
//    }
//}



//public class ValidationFilter<T> : IActionFilter where T : class
//{
//    private readonly IValidator<T> _validator;
//    public ValidationFilter(IValidator<T> validator)
//    {
//        _validator = validator;
//    }
//    public void OnActionExecuting(ActionExecutingContext context)
//    {
//        var endpoint = context.ActionDescriptor.EndpointMetadata
//            .OfType<ControllerActionDescriptor>()
//            .FirstOrDefault();


//        if (context.ActionArguments["entity"] is T entity)
//        {
//            var validationResult = _validator.Validate(entity);
//            if (!validationResult.IsValid)
//            {
//                List<string> errorMessages = validationResult.Errors
//                                                                    .Select(error => error.ErrorMessage)
//                                                                    .ToList();
//                context.Result = new BadRequestObjectResult(errorMessages);
//                return;
//            }
//        }
//        else
//        {
//            context.Result = new BadRequestObjectResult("Invalid request entity.");
//            return;
//        }
//    }
//    public void OnActionExecuted(ActionExecutedContext context)
//    {
//        // No action needed
//    }
//}

public class ValidationFilter : ActionFilterAttribute
{
    private readonly IEnumerable<IValidator> _validators;
    private readonly ILogger<ValidationFilter> _logger;

    public ValidationFilter(IEnumerable<IValidator> validators, ILogger<ValidationFilter> logger)
    {
        _validators = validators;
        _logger = logger;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var entry in context.ActionArguments)
        {
            var argumentType = entry.Value.GetType();
            var validator = _validators.FirstOrDefault(v => v.CanValidateInstancesOfType(argumentType));

            if (validator != null)
            {
                var validationContext = new ValidationContext<object>(entry.Value);
                var validationResult = validator.Validate(validationContext);
                if (!validationResult.IsValid)
                {
                    List<string> errorMessages = validationResult.Errors.Select(error => error.ErrorMessage)
                                                                        .ToList();
                    context.Result = new BadRequestObjectResult(errorMessages);
                    _logger.LogError("Validation errors: {Errors}", errorMessages);
                    return;
                }
            }
        }
    }
}









