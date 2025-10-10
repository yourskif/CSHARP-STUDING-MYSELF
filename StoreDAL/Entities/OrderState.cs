namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents an order state (status) in the system.
/// Examples: New Order, Confirmed, In Delivery, Delivered, etc.
/// </summary>
[Table("order_states")]
public class OrderState : BaseEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderState"/> class.
    /// Default constructor for EF Core.
    /// </summary>
    public OrderState()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderState"/> class with specified values.
    /// </summary>
    /// <param name="id">Unique identifier for the order state.</param>
    /// <param name="stateName">Human-readable name of the state (e.g., "New Order", "Confirmed").</param>
    public OrderState(int id, string stateName)
        : base(id)
    {
        this.StateName = stateName;
    }

    /// <summary>
    /// Gets or sets the human-readable name of this order state.
    /// </summary>
    /// <example>
    /// "New Order", "Confirmed", "In Delivery", "Delivered to client".
    /// </example>
    [Column("state_name")]
    public string StateName { get; set; }

    /// <summary>
    /// Gets or sets the collection of orders that have this state.
    /// Navigation property for EF Core relationship.
    /// </summary>
    public virtual IList<CustomerOrder> Order { get; set; }
}
