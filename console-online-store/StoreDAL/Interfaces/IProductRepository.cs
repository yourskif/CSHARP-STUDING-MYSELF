// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreDAL\Interfaces\IProductRepository.cs
namespace StoreDAL.Interfaces;

using System.Collections.Generic;

using StoreDAL.Entities;

/// <summary>
/// Product repository contract: read operations with includes + full CUD.
/// </summary>
public interface IProductRepository
{
    // READ
    IEnumerable<Product> GetAllWithIncludes();
    IEnumerable<Product> GetByCategoryId(int categoryId);
    Product? GetByIdWithIncludes(int id);

    // Convenience read (no includes)
    IEnumerable<Product> GetAll();
    Product? GetById(int id);

    // CUD
    /// <summary>Adds a new product and saves changes. Returns the created entity (with Id).</summary>
    Product Add(Product entity);

    /// <summary>Updates an existing product by Id (throws if not found). Saves changes.</summary>
    void Update(Product entity);

    /// <summary>Deletes a product by Id. Returns true if removed; false if not found.</summary>
    bool Delete(int id);
}
