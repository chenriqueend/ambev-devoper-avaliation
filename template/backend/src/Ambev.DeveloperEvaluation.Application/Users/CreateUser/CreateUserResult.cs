using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser;

/// <summary>
/// Represents the response returned after attempting to create a new user.
/// </summary>
/// <remarks>
/// This response contains the result of the user creation operation, including:
/// - The unique identifier of the newly created user (if successful)
/// - The success status of the operation
/// - Any error message that occurred during the operation (if unsuccessful)
/// </remarks>
public class CreateUserResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly created user.
    /// </summary>
    /// <value>A GUID that uniquely identifies the created user in the system. Null if the operation failed.</value>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user creation operation was successful.
    /// </summary>
    /// <value>True if the user was created successfully; otherwise, false.</value>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if the user creation operation failed.
    /// </summary>
    /// <value>A string containing the error message, or null if the operation was successful.</value>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the user's name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's role
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Gets or sets the user's status
    /// </summary>
    public UserStatus Status { get; set; }
}
