using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser;

/// <summary>
/// Handler for processing CreateUserCommand requests
/// </summary>
public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of CreateUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="passwordHasher">The password hasher</param>
    /// <param name="mapper">The mapper instance</param>
    public CreateUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the CreateUserCommand request
    /// </summary>
    /// <param name="command">The CreateUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user details</returns>
    public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (existingUser != null)
        {
            return new CreateUserResult
            {
                Success = false,
                Error = "User with this email already exists"
            };
        }

        var user = _mapper.Map<User>(command);
        user.PasswordHash = _passwordHasher.HashPassword(command.Password);

        var validationResult = user.Validate();
        if (!validationResult.IsValid)
        {
            return new CreateUserResult
            {
                Success = false,
                Error = string.Join(", ", validationResult.Errors.Select(e => e.Detail))
            };
        }

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);
        return _mapper.Map<CreateUserResult>(createdUser);
    }
}
