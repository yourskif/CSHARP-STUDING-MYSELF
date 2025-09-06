// ConsoleApp/Handlers/ContextMenuHandlers/OrderContextMenuHandler.cs
namespace ConsoleApp.Handlers.ContextMenuHandlers;

using System;

using ConsoleApp.Controllers;

using StoreDAL.Data;

public sealed class OrderContextMenuHandler
{
    private readonly AdminOrderController ctrl;

    public OrderContextMenuHandler(StoreDbContext db)
    {
        if (db is null) throw new ArgumentNullException(nameof(db));
        this.ctrl = new AdminOrderController(db);
    }

    /// <summary>
    /// Запускає екран замовлень (список + швидкі дії).
    /// </summary>
    public void Run() => this.ctrl.ShowOrders();

    /// <summary>
    /// Показати деталі конкретного замовлення (інтерактивно запитує Id).
    /// </summary>
    public void ViewDetails() => this.ctrl.ShowOrderDetails();

    /// <summary>
    /// Змінити статус (інтерактивно).
    /// </summary>
    public void ChangeStatus() => this.ctrl.ChangeOrderStatus();

    /// <summary>
    /// Скасувати замовлення адміністратором (інтерактивно).
    /// </summary>
    public void CancelByAdmin() => this.ctrl.CancelOrder();

    /// <summary>
    /// Створити замовлення (інтерактивно: вибір товару, кількість, логін).
    /// </summary>
    public void CreateOrder() => this.ctrl.CreateOrder();

    /// <summary>
    /// Створити замовлення з параметрами.
    /// Повертає Id створеного замовлення (0, якщо не вдалось).
    /// </summary>
    public int CreateOrder(int productId, int quantity, string login)
    {
        // Метод контролера уже друкує повідомлення сам.
        // Тут просто повертаємо Id для можливого подальшого використання.
        return this.ctrl.CreateOrder(productId, quantity, login);
    }
}
