using ForexFintechAPI.Action_Filters;
using ForexFintechAPI.Models;
using IdentityAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private readonly RegisterDtoValidator _validator;
        private ILogger<AuthController> _logger;

        public AuthController(IUserService userService, RegisterDtoValidator validator, ILogger<AuthController> logger)
        {
            _userService = userService;
            this._validator = validator;
            _logger = logger;
        }
        [ServiceFilter(typeof(ValidationFilter))]
        [HttpPost("Register")]
        [SwaggerOperation(Summary ="Registration of User",Description = "Registers a new user in the system")]
        [SwaggerResponse(200,Type =typeof(UserManagerResponse),ContentTypes = new[] { "application/json" })]
        [SwaggerResponse(400,Type =typeof(string),ContentTypes = new[] { "application/json" })]
        [SwaggerResponse(500, Type = typeof(string), ContentTypes = new[] { "application/json" })]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var validatorResult = _validator.Validate(model);
            var result = await _userService.RegisterUserAsync(model);
            if (result.IsSuccess)
                return Ok(result);
            _logger.LogError("Authentication Error: {Error}", result.Errors);
            return BadRequest(result.Errors);
        }
        [HttpPost("Login")]
        [SwaggerOperation(Summary = "LogIn User", Description = "Logging into the system using registered E-mail and Password ")]
        [SwaggerResponse(200, Type = typeof(UserManagerResponse), ContentTypes = new[] { "application/json" })]
        [SwaggerResponse(400, Type = typeof(string), ContentTypes = new[] { "application/json" })]
        [SwaggerResponse(500, Type = typeof(string), ContentTypes = new[] { "application/json" })]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var result = await _userService.LoginUserAsync(model);
            if (result.IsSuccess)
                return Ok(result);
            _logger.LogError("Authentication Error: {Error}", result.Errors);
            return BadRequest(result.Errors);
        }
    }
}

