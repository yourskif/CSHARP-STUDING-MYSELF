// Path: console-online-store/StoreBLL/Exceptions/StoreException.cs
namespace StoreBLL.Exceptions;

using System;

/// <summary>
/// Base exception for all store-related errors.
/// </summary>
public class StoreException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoreException"/> class.
    /// </summary>
    public StoreException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StoreException"/> class with a message.
    /// </summary>
    /// <param name="message">Error message.</param>
    public StoreException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StoreException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="innerException">Inner exception.</param>
    public StoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : StoreException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">Validation error message.</param>
    public ValidationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="fieldName">Name of field that failed validation.</param>
    /// <param name="message">Validation error message.</param>
    public ValidationException(string fieldName, string message)
        : base($"{fieldName}: {message}")
    {
        this.FieldName = fieldName;
    }

    /// <summary>
    /// Gets the name of the field that failed validation.
    /// </summary>
    public string? FieldName { get; }
}

/// <summary>
/// Exception thrown when insufficient stock is available for an operation.
/// </summary>
public class InsufficientStockException : StoreException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InsufficientStockException"/> class.
    /// </summary>
    /// <param name="productId">Product identifier.</param>
    /// <param name="requested">Requested quantity.</param>
    /// <param name="available">Available quantity.</param>
    public InsufficientStockException(int productId, int requested, int available)
        : base($"Insufficient stock for product #{productId}. Requested: {requested}, Available: {available}")
    {
        this.ProductId = productId;
        this.Requested = requested;
        this.Available = available;
    }

    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public int ProductId { get; }

    /// <summary>
    /// Gets the requested quantity.
    /// </summary>
    public int Requested { get; }

    /// <summary>
    /// Gets the available quantity.
    /// </summary>
    public int Available { get; }
}

/// <summary>
/// Exception thrown when an invalid order state transition is attempted.
/// </summary>
public class InvalidOrderStateTransitionException : StoreException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOrderStateTransitionException"/> class.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="fromState">Current order state.</param>
    /// <param name="toState">Target order state.</param>
    public InvalidOrderStateTransitionException(int orderId, int fromState, int toState)
        : base($"Cannot transition order #{orderId} from state {fromState} to {toState}")
    {
        this.OrderId = orderId;
        this.FromState = fromState;
        this.ToState = toState;
    }

    /// <summary>
    /// Gets the order identifier.
    /// </summary>
    public int OrderId { get; }

    /// <summary>
    /// Gets the current order state.
    /// </summary>
    public int FromState { get; }

    /// <summary>
    /// Gets the target order state.
    /// </summary>
    public int ToState { get; }
}

/// <summary>
/// Exception thrown when a user is blocked and attempts to perform an action.
/// </summary>
public class UserBlockedException : StoreException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserBlockedException"/> class.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    public UserBlockedException(int userId)
        : base($"User #{userId} is blocked and cannot perform this action")
    {
        this.UserId = userId;
    }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public int UserId { get; }
}

/// <summary>
/// Exception thrown when a duplicate entity is detected.
/// </summary>
public class DuplicateEntityException : StoreException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateEntityException"/> class.
    /// </summary>
    /// <param name="entityType">Type of entity.</param>
    /// <param name="identifier">Duplicate identifier value.</param>
    public DuplicateEntityException(string entityType, string identifier)
        : base($"A {entityType} with identifier '{identifier}' already exists")
    {
        this.EntityType = entityType;
        this.Identifier = identifier;
    }

    /// <summary>
    /// Gets the entity type.
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the duplicate identifier.
    /// </summary>
    public string Identifier { get; }
}
