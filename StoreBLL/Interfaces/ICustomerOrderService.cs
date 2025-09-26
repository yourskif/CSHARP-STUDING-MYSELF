// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\StoreBLL\Interfaces\ICustomerOrderService.cs
namespace StoreBLL.Interfaces;

using System.Collections.Generic;
using StoreBLL.Models;

public interface ICustomerOrderService : ICrud
{
    bool TryChangeState(int orderId, int newStateId, out string error);

    bool CancelOwnOrder(int orderId, int userId, out string error);

    bool MarkAsReceived(int orderId, int userId, out string error);

    IEnumerable<CustomerOrderModel> GetOrdersByUser(int userId);
}
