namespace Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;

/// <summary>
/// Represents the result of an authentication attempt
/// </summary>
public class AuthenticateUserResult
{
    /// <summary>
    /// Gets or sets whether the authentication was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the authentication token if successful
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message if authentication failed
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the user's unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's role
    /// </summary>
    public string Role { get; set; } = string.Empty;
}
