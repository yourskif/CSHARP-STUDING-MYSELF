namespace StoreDAL.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

[Table("customer_orders")]
public class CustomerOrder : BaseEntity
{
    public CustomerOrder() : base()
    {
    }

    // BLL often uses ctor with 3rd arg = operationTime (string or DateTime).
    public CustomerOrder(int id, int userId, string operationTime)
        : base(id)
    {
        this.UserId = userId;
        this.OperationTimeUtc = ParseDate(operationTime);
    }

    public CustomerOrder(int id, int userId, DateTime operationTime)
        : base(id)
    {
        this.UserId = userId;
        this.OperationTimeUtc = operationTime;
    }

    // Variants with orderStateId for convenience/back-compat.
    public CustomerOrder(int id, int userId, int orderStateId, string operationTime)
        : this(id, userId, operationTime)
    {
        this.StateId = orderStateId;
    }

    public CustomerOrder(int id, int userId, int orderStateId, DateTime operationTime)
        : this(id, userId, operationTime)
    {
        this.StateId = orderStateId;
    }

    private static DateTime ParseDate(string value)
    {
        // Accept ISO first; fallback to invariant parsing; default = UtcNow
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
        {
            return dt;
        }

        if (DateTime.TryParse(value, out dt))
        {
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }

        return DateTime.UtcNow;
    }

    [Column("user_id")]
    public int UserId { get; set; }

    // Canonical FK to order_states (column order_state_id)
    [Column("order_state_id")]
    public int StateId { get; set; }

    // Alias expected by BLL
    [NotMapped]
    public int OrderStateId
    {
        get => this.StateId;
        set => this.StateId = value;
    }

    // Real stored value in DB
    [Column("operation_time")]
    public DateTime OperationTimeUtc { get; set; } = DateTime.UtcNow;

    // String alias for BLL (so expressions like string ?? DateTime stop failing on compile)
    // Format to ISO 8601 for stable roundtrip.
    [NotMapped]
    public string OperationTime
    {
        get => this.OperationTimeUtc.ToString("o", CultureInfo.InvariantCulture);
        set => this.OperationTimeUtc = ParseDate(value);
    }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("StateId")]
    public OrderState? State { get; set; }

    public virtual IList<OrderDetail>? OrderDetails { get; set; }
}
