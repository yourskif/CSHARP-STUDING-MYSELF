// Path: console-online-store/StoreBLL/Patterns/ServiceLocator.cs
namespace StoreBLL.Patterns;

using System;
using System.Collections.Generic;

using StoreBLL.Services;

using StoreDAL.Data;

/// <summary>
/// Service Locator pattern for centralized service management.
/// Provides singleton access to business logic services.
/// </summary>
public sealed class ServiceLocator
{
    private static ServiceLocator? instance;
    private readonly Dictionary<Type, object> services = new();
    private readonly StoreDbContext context;

    private ServiceLocator(StoreDbContext context)
    {
        this.context = context;
        this.RegisterServices();
    }

    public static ServiceLocator Instance(StoreDbContext context)
    {
        if (instance == null)
        {
            instance = new ServiceLocator(context);
        }

        return instance;
    }

    public static void Reset()
    {
        instance = null;
    }

    public T GetService<T>()
        where T : class
    {
        var type = typeof(T);
        if (this.services.TryGetValue(type, out var service))
        {
            return (T)service;
        }

        throw new InvalidOperationException($"Service {type.Name} not registered");
    }

    private void RegisterServices()
    {
        this.services[typeof(ProductService)] = new ProductService(new StoreDAL.Repository.ProductRepository(this.context));
        this.services[typeof(UserService)] = new UserService(this.context);
        this.services[typeof(CategoryService)] = new CategoryService(this.context);
        this.services[typeof(CustomerOrderService)] = new CustomerOrderService(this.context);
        this.services[typeof(StockReservationService)] = new StockReservationService(this.context);
    }
}
