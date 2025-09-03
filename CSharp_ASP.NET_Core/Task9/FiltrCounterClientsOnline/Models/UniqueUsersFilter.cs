using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Collections.Generic;
using System;

namespace FiltrCounterClientsOnline
{
    public class UniqueUsersFilter : ActionFilterAttribute
    {
        private static readonly object _lock = new object();
        private static readonly HashSet<string> UniqueUsers = new HashSet<string>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(userIp))
            {
                lock (_lock)
                {
                    if (UniqueUsers.Add(userIp))
                    {
                        File.AppendAllText("unique_users_log.txt", $"{userIp} - {DateTime.Now}\n");
                    }
                }
            }
        }
    }
}
