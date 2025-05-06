using Ambev.DeveloperEvaluation.Domain.Common;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.Unit.TestHelpers;

/// <summary>
/// Extension methods for testing entities.
/// </summary>
public static class EntityTestExtensions
{
    /// <summary>
    /// Sets the Id of an entity using reflection.
    /// This is only for testing purposes.
    /// </summary>
    /// <param name="entity">The entity to set the Id for.</param>
    /// <param name="id">The Id value to set.</param>
    public static void SetId(this BaseEntity entity, Guid id)
    {
        var property = entity.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(entity, id);
    }
}