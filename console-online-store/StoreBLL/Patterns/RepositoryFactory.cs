// Path: console-online-store/StoreBLL/Patterns/RepositoryFactory.cs
namespace StoreBLL.Patterns;

using StoreDAL.Data;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

/// <summary>
/// Factory pattern for creating repository instances.
/// Centralizes repository creation logic.
/// </summary>
public class RepositoryFactory
{
    private readonly StoreDbContext context;

    public RepositoryFactory(StoreDbContext context)
    {
        this.context = context ?? throw new System.ArgumentNullException(nameof(context));
    }

    public IProductRepository CreateProductRepository()
    {
        return new ProductRepository(this.context);
    }

    public IUserRepository CreateUserRepository()
    {
        return new UserRepository(this.context);
    }

    public ICustomerOrderRepository CreateCustomerOrderRepository()
    {
        return new CustomerOrderRepository(this.context);
    }

    public IOrderDetailRepository CreateOrderDetailRepository()
    {
        return new OrderDetailRepository(this.context);
    }

    public ICategoryRepository CreateCategoryRepository()
    {
        return new CategoryRepository(this.context);
    }

    public IManufacturerRepository CreateManufacturerRepository()
    {
        return new ManufacturerRepository(this.context);
    }
}
