namespace ConsoleApp.Helpers
{
    /// <summary>
    /// Maps OrderStateId to a readable state name for UI.
    /// </summary>
    public static class OrderStateHelper
    {
        public static string GetStateName(int stateId)
        {
            return stateId switch
            {
                1 => "New",
                2 => "Canceled by User",
                3 => "Canceled by Admin",
                4 => "Confirmed",
                5 => "Moved to Delivery",
                6 => "In Delivery",
                7 => "Delivered to Client",
                8 => "Confirmed by Client",
                _ => $"Unknown ({stateId})",
            };
        }
    }
}
