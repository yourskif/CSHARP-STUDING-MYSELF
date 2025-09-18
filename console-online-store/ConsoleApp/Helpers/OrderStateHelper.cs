namespace ConsoleApp.Helpers
{
    /// <summary>
    /// Helper class for mapping OrderStateId to readable state names for UI display.
    /// </summary>
    public static class OrderStateHelper
    {
        /// <summary>
        /// Gets the human-readable name for an order state ID.
        /// </summary>
        /// <param name="stateId">Order state ID (1-8).</param>
        /// <returns>Readable state name.</returns>
        public static string GetStateName(int stateId)
        {
            return stateId switch
            {
                1 => "New Order",
                2 => "Cancelled by User",
                3 => "Cancelled by Administrator",
                4 => "Confirmed",
                5 => "Moved to Delivery Company",
                6 => "In Delivery",
                7 => "Delivered to Client",
                8 => "Delivery Confirmed by Client",
                _ => $"Unknown State ({stateId})"
            };
        }

        /// <summary>
        /// Gets the short state name for compact display.
        /// </summary>
        /// <param name="stateId">Order state ID (1-8).</param>
        /// <returns>Short state name.</returns>
        public static string GetShortStateName(int stateId)
        {
            return stateId switch
            {
                1 => "New",
                2 => "Cancelled",
                3 => "Admin Cancelled",
                4 => "Confirmed",
                5 => "To Delivery",
                6 => "In Transit",
                7 => "Delivered",
                8 => "Received",
                _ => $"Unknown ({stateId})"
            };
        }

        /// <summary>
        /// Checks if the state represents a cancelled order.
        /// </summary>
        /// <param name="stateId">Order state ID.</param>
        /// <returns>True if order is cancelled, false otherwise.</returns>
        public static bool IsCancelled(int stateId)
        {
            return stateId == 2 || stateId == 3;
        }

        /// <summary>
        /// Checks if the state represents a completed order.
        /// </summary>
        /// <param name="stateId">Order state ID.</param>
        /// <returns>True if order is completed, false otherwise.</returns>
        public static bool IsCompleted(int stateId)
        {
            return stateId == 8;
        }

        /// <summary>
        /// Checks if user can cancel this order state.
        /// </summary>
        /// <param name="stateId">Order state ID.</param>
        /// <returns>True if user can cancel, false otherwise.</returns>
        public static bool CanUserCancel(int stateId)
        {
            return stateId == 1; // Only "New" orders can be cancelled by user
        }

        /// <summary>
        /// Checks if user can mark this order as received.
        /// </summary>
        /// <param name="stateId">Order state ID.</param>
        /// <returns>True if user can mark as received, false otherwise.</returns>
        public static bool CanUserMarkReceived(int stateId)
        {
            return stateId == 7; // Only "Delivered to Client" orders can be marked as received
        }
    }
}
