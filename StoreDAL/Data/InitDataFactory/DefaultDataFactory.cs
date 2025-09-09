using StoreDAL.Entities;

namespace StoreDAL.Data.InitDataFactory
{
    /// <summary>
    /// Фабрика, яка повертає порожні масиви.
    /// Використовується як "дефолт" у випадках, коли не потрібні тестові чи релізні дані.
    /// </summary>
    public class DefaultDataFactory : AbstractDataFactory
    {
        public override Category[] GetCategoryData() => new Category[0];
        public override Manufacturer[] GetManufacturerData() => new Manufacturer[0];
        public override OrderState[] GetOrderStateData() => new OrderState[0];
        public override UserRole[] GetUserRoleData() => new UserRole[0];
        public override User[] GetUserData() => new User[0];
        public override ProductTitle[] GetProductTitleData() => new ProductTitle[0];
        public override Product[] GetProductData() => new Product[0];
        public override CustomerOrder[] GetCustomerOrderData() => new CustomerOrder[0];
        public override OrderDetail[] GetOrderDetailData() => new OrderDetail[0];
    }
}
