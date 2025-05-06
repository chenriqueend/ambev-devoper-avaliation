using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of AuthController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public AuthController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Authenticates a user with their credentials
    /// </summary>
    /// <param name="request">The authentication request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication token if successful</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<AuthenticateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request, CancellationToken cancellationToken)
    {
        var validator = new AuthenticateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid request",
                Errors = validationResult.Errors.Select(e => new ValidationErrorDetail { Detail = e.ErrorMessage, Error = e.ErrorCode })
            });

        var command = _mapper.Map<AuthenticateUserCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
        {
            // Return different status codes based on the error message
            return result.Message switch
            {
                "User not found" => NotFound(new ApiResponse
                {
                    Success = false,
                    Message = result.Message ?? "User not found",
                    Errors = new[] { new ValidationErrorDetail { Detail = result.Message ?? "User not found", Error = "UserNotFound" } }
                }),
                "User is suspended" => Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = result.Message ?? "User is suspended",
                    Errors = new[] { new ValidationErrorDetail { Detail = result.Message ?? "User is suspended", Error = "UserSuspended" } }
                }),
                _ => Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = result.Message ?? "Authentication failed",
                    Errors = new[] { new ValidationErrorDetail { Detail = result.Message ?? "Authentication failed", Error = "AuthenticationFailed" } }
                })
            };
        }

        var response = _mapper.Map<AuthenticateUserResponse>(result);
        return Ok<AuthenticateUserResponse>(response);
    }
}
