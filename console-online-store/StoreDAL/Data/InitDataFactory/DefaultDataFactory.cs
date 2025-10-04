// Path: C:\Users\SK\source\repos\C#\1313\console-online-store\StoreDAL\Data\InitDataFactory\DefaultDataFactory.cs
namespace StoreDAL.Data.InitDataFactory;

using StoreDAL.Entities;

public sealed class DefaultDataFactory : AbstractDataFactory
{
    public override Category[] GetCategoryData() => Array.Empty<Category>();

    public override Manufacturer[] GetManufacturerData() => Array.Empty<Manufacturer>();

    public override OrderState[] GetOrderStateData() => Array.Empty<OrderState>();

    public override UserRole[] GetUserRoleData() => Array.Empty<UserRole>();

    public override User[] GetUserData() => Array.Empty<User>();

    public override ProductTitle[] GetProductTitleData() => Array.Empty<ProductTitle>();

    public override Product[] GetProductData() => Array.Empty<Product>();

    public override CustomerOrder[] GetCustomerOrderData() => Array.Empty<CustomerOrder>();

    public override OrderDetail[] GetOrderDetailData() => Array.Empty<OrderDetail>();
}
