using System.Text.Json.Serialization;

namespace EmployeeManagement.Domain.Entities.Authentication;

public class AuthenticationModel
{
    public string UserId { get; set; }
    public string? Message { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
    public string? Token { get; set; }

    public int ExpiresOn { get; set; }
    [JsonIgnore]
    public string? RefreshToken { get; set; }
    public int RefreshTokenExpiration { get; set; }
}