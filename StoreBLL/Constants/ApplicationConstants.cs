namespace StoreBLL.Constants
{
    /// <summary>
    /// Application-wide constants for user roles, order states, and business rules.
    /// Centralizes magic numbers to improve maintainability and reduce errors.
    /// </summary>
    public static class ApplicationConstants
    {
        /// <summary>
        /// User role identifiers.
        /// </summary>
        public static class UserRoles
        {
            /// <summary>
            /// Administrator role with full system access.
            /// </summary>
            public const int Admin = 1;

            /// <summary>
            /// Registered user role with standard customer privileges.
            /// </summary>
            public const int RegisteredUser = 2;

            /// <summary>
            /// Guest role with limited read-only access.
            /// </summary>
            public const int Guest = 3;
        }

        /// <summary>
        /// Order state identifiers representing the order lifecycle.
        /// </summary>
        public static class OrderStates
        {
            /// <summary>
            /// Order has been created but not yet processed.
            /// </summary>
            public const int NewOrder = 1;

            /// <summary>
            /// Order cancelled by the customer.
            /// </summary>
            public const int CancelledByUser = 2;

            /// <summary>
            /// Order cancelled by system administrator.
            /// </summary>
            public const int CancelledByAdministrator = 3;

            /// <summary>
            /// Order confirmed and ready for fulfillment.
            /// </summary>
            public const int Confirmed = 4;

            /// <summary>
            /// Order handed over to delivery company.
            /// </summary>
            public const int MovedToDelivery = 5;

            /// <summary>
            /// Order is currently being delivered.
            /// </summary>
            public const int InDelivery = 6;

            /// <summary>
            /// Order delivered to customer address.
            /// </summary>
            public const int DeliveredToClient = 7;

            /// <summary>
            /// Customer confirmed receipt of the order.
            /// </summary>
            public const int DeliveryConfirmedByClient = 8;
        }

        /// <summary>
        /// Business validation rules and constraints.
        /// </summary>
        public static class ValidationRules
        {
            /// <summary>
            /// Minimum password length for user accounts.
            /// </summary>
            public const int MinPasswordLength = 6;

            /// <summary>
            /// Maximum login name length.
            /// </summary>
            public const int MaxLoginLength = 50;

            /// <summary>
            /// Minimum product price in the system currency.
            /// </summary>
            public const decimal MinProductPrice = 0.01m;

            /// <summary>
            /// Maximum product name length.
            /// </summary>
            public const int MaxProductNameLength = 200;
        }
    }
}
