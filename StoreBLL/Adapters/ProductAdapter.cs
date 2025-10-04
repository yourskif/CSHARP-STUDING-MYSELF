// Path: console-online-store/StoreBLL/Adapters/ProductAdapter.cs
namespace StoreBLL.Adapters
{
    using StoreBLL.Models;

    using StoreDAL.Entities;

    /// <summary>
    /// Adapter for converting between Product entity and ProductModel.
    /// Provides type-safe mapping without reflection or dynamic types.
    /// </summary>
    public static class ProductAdapter
    {
        /// <summary>
        /// Converts Product entity to ProductModel.
        /// </summary>
        /// <param name="entity">Product entity from database.</param>
        /// <returns>Product model for business layer.</returns>
        public static ProductModel ToModel(Product entity)
        {
            if (entity == null)
            {
                return new ProductModel(
                    id: 0,
                    title: string.Empty,
                    category: string.Empty,
                    manufacturer: string.Empty,
                    sku: string.Empty,
                    description: string.Empty,
                    price: 0,
                    stock: 0,
                    reserved: 0);
            }

            return new ProductModel(
                id: entity.Id,
                title: entity.Title?.Title ?? $"Product {entity.Id}",
                category: entity.Title?.Category?.Name ?? "Unknown",
                manufacturer: entity.Manufacturer?.Name ?? "Unknown",
                sku: string.Empty,
                description: entity.Description ?? string.Empty,
                price: entity.UnitPrice,
                stock: entity.StockQuantity,
                reserved: entity.ReservedQuantity);
        }

        /// <summary>
        /// Updates entity with values from model.
        /// </summary>
        /// <param name="entity">Product entity to update.</param>
        /// <param name="model">Product model with new values.</param>
        public static void UpdateEntity(Product entity, ProductModel model)
        {
            if (entity == null || model == null)
            {
                return;
            }

            entity.UnitPrice = model.Price;
            entity.StockQuantity = model.Stock;
            entity.ReservedQuantity = model.Reserved;
            entity.Description = model.Description;

            if (entity.Title != null)
            {
                entity.Title.Title = model.Title;
            }
        }
    }
}
