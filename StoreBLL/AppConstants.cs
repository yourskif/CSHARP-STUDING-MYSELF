// Path: console-online-store/StoreBLL/AppConstants.cs
namespace StoreBLL
{
    /// <summary>
    /// Application-wide constants for role IDs, order states, and other magic numbers.
    /// Centralizes all hard-coded values to improve maintainability and readability.
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// User role identifiers.
        /// </summary>
        public static class UserRoles
        {
            /// <summary>
            /// Administrator role - full system access.
            /// </summary>
            public const int Administrator = 1;

            /// <summary>
            /// Registered user role - standard customer access.
            /// </summary>
            public const int RegisteredUser = 2;

            /// <summary>
            /// Guest role - read-only access.
            /// </summary>
            public const int Guest = 3;
        }

        /// <summary>
        /// Order state identifiers following the order lifecycle.
        /// </summary>
        public static class OrderStates
        {
            /// <summary>
            /// Initial state when order is created.
            /// </summary>
            public const int NewOrder = 1;

            /// <summary>
            /// Order cancelled by the customer.
            /// </summary>
            public const int CancelledByUser = 2;

            /// <summary>
            /// Order cancelled by administrator.
            /// </summary>
            public const int CancelledByAdministrator = 3;

            /// <summary>
            /// Order confirmed and ready for processing.
            /// </summary>
            public const int Confirmed = 4;

            /// <summary>
            /// Order handed over to delivery company.
            /// </summary>
            public const int MovedToDeliveryCompany = 5;

            /// <summary>
            /// Order is being delivered to customer.
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
    }
}
