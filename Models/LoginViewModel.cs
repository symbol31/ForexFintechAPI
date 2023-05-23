using System.ComponentModel.DataAnnotations;

namespace ForexFintechAPI.Models;

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Role { get;set; }
}
public class UserManagerResponse
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public IEnumerable<string> Errors { get; set; }
    public DateTime? ExpireTime { get; set; }
}

