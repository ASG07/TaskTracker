namespace webapiemp.Models;

public class AuthenticationResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public bool IsSuccess { get; set; }
    public IEnumerable<string> ErrorMessages { get; set; }

}
