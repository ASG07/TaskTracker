using System.ComponentModel.DataAnnotations;

namespace webapiemp.DTOs.RequestDTO;

public class UserRegisterDTO
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
}
