using System;

namespace ConsoleApp.Security
{
    /// <summary>
    /// Simple in-memory session context for the current console run.
    /// </summary>
    public static class SessionContext
    {
        public static bool IsAuthenticated { get; private set; }
        public static int? UserId { get; private set; }
        public static string UserName { get; private set; } = string.Empty;
        public static string RoleName { get; private set; } = "Guest";

        public static void SignIn(int userId, string userName, string roleName)
        {
            UserId = userId;
            UserName = userName ?? string.Empty;
            RoleName = string.IsNullOrWhiteSpace(roleName) ? "Guest" : roleName;
            IsAuthenticated = true;
        }

        public static void SignOut()
        {
            UserId = null;
            UserName = string.Empty;
            RoleName = "Guest";
            IsAuthenticated = false;
        }
    }
}
