namespace StoreDAL.Data.InitDataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreDAL.Entities;

public class ReleaseDataFactory : AbstractDataFactory
{
    public override Category[] GetCategoryData()
    {
        return Array.Empty<Category>();
    }

    public override CustomerOrder[] GetCustomerOrderData()
    {
        return Array.Empty<CustomerOrder>();
    }

    public override Manufacturer[] GetManufacturerData()
    {
        return Array.Empty<Manufacturer>();
    }

    public override OrderDetail[] GetOrderDetailData()
    {
        return Array.Empty<OrderDetail>();
    }

    public override OrderState[] GetOrderStateData()
    {
        return new[]
        {
            new OrderState(1, "New Order"),
            new OrderState(2, "Canceled by user"),
            new OrderState(3, "Canceled by administrator"),
            new OrderState(4, "Confirmed"),
            new OrderState(5, "Moved to delivery company"),
            new OrderState(6, "In delivery"),
            new OrderState(7, "Delivered to client"),
            new OrderState(8, "Delivery aproved by client"),
        };
    }

    public override Product[] GetProductData()
    {
        return Array.Empty<Product>();
    }

    public override ProductTitle[] GetProductTitleData()
    {
        return Array.Empty<ProductTitle>();
    }

    public override User[] GetUserData()
    {
        return Array.Empty<User>();
    }

    public override UserRole[] GetUserRoleData()
    {
        return new[]
        {
            new UserRole(1, "Admin"),
            new UserRole(2, "Registred"),
            new UserRole(3, "Guest"),
        };
    }
}