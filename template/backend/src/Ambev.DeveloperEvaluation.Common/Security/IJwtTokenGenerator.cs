namespace Ambev.DeveloperEvaluation.Common.Security;

/// <summary>
/// Interface for generating JWT tokens
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="email">The user's email address</param>
    /// <returns>The generated JWT token</returns>
    string GenerateToken(Guid userId, string email);
}
