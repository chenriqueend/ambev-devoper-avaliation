using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;

/// <summary>
/// Handler for the AuthenticateUserCommand
/// </summary>
public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    /// <summary>
    /// Initializes a new instance of AuthenticateUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="passwordHasher">The password hasher</param>
    /// <param name="jwtTokenGenerator">The JWT token generator</param>
    public AuthenticateUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Handles the authentication request
    /// </summary>
    /// <param name="request">The authentication command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result</returns>
    public async Task<AuthenticateUserResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return new AuthenticateUserResult { Success = false, Message = "User not found" };

        if (user.Status == UserStatus.Suspended)
            return new AuthenticateUserResult { Success = false, Message = "User is suspended" };

        try
        {
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return new AuthenticateUserResult { Success = false, Message = "Invalid password" };
        }
        catch
        {
            // If there's any error verifying the password (like invalid hash format),
            // return a generic error message
            return new AuthenticateUserResult { Success = false, Message = "Invalid credentials" };
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email);
        return new AuthenticateUserResult
        {
            Success = true,
            Token = token,
            Id = user.Id,
            Email = user.Email,
            Name = user.Username,
            Role = user.Role.ToString()
        };
    }
}
