namespace StoreDAL.Data.InitDataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreDAL.Entities;

public abstract class AbstractDataFactory
{
    public abstract Category[] GetCategoryData();

    public abstract CustomerOrder[] GetCustomerOrderData();

    public abstract Manufacturer[] GetManufacturerData();

    public abstract OrderDetail[] GetOrderDetailData();

    public abstract OrderState[] GetOrderStateData();

    public abstract Product[] GetProductData();

    public abstract ProductTitle[] GetProductTitleData();

    public abstract User[] GetUserData();

    public abstract UserRole[] GetUserRoleData();
}
