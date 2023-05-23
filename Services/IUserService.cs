using ForexFintechAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityAPI.Services;

public interface IUserService
{
    Task<UserManagerResponse> RegisterUserAsync(RegisterDto model);
    Task<UserManagerResponse> LoginUserAsync(LoginDto model);
}
public class UserService : IUserService
{
    private UserManager<IdentityUser> _userManager;
    private readonly ILogger<IdentityUser> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _config;
    public UserService(UserManager<IdentityUser> userManager, IConfiguration config, ILogger<IdentityUser> logger, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _config = config;
        _logger = logger;
        _signInManager = signInManager;
    }
    public async Task<UserManagerResponse> RegisterUserAsync(RegisterDto model)
    {
        if (model == null)
            throw new NullReferenceException("Register Model is null");
        if (model.Password != model.ConfirmPassword)
            return new UserManagerResponse
            {
                Message = "Confirm password and password doesn't match",
                IsSuccess = false,
            };
        IdentityUser identityUser = new IdentityUser
        {
            Email = model.Email,
            UserName = model.Email
        };
        var result = await _userManager.CreateAsync(identityUser, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(identityUser, model.Role);
            return new UserManagerResponse
            {
                Message = "User created succesfully",
                IsSuccess = true
            };
        }
        else
        {
            return new UserManagerResponse
            {
                Message = "User couldn't be created.",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
    public async Task<UserManagerResponse> LoginUserAsync(LoginDto model)
    {
        if (model == null)
            throw new NullReferenceException("Please fill all the required fields.");
        var user = await _userManager.FindByEmailAsync(model.Email);
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
            return new UserManagerResponse
            {
                Message = "The username or password is incorrect.",
                IsSuccess = false,
            };
        var roles = await _userManager.GetRolesAsync(user);
        var claim = new[]
        {
            new Claim(ClaimTypes.Name, model.Email),
            new Claim(ClaimTypes.Role, roles.FirstOrDefault())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claim,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
        string tokenasstring = new JwtSecurityTokenHandler().WriteToken(token);
        return new UserManagerResponse
        {
            Message = tokenasstring,
            IsSuccess = true,
            ExpireTime = token.ValidTo
        };
    }
}
