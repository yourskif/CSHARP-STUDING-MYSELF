namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoreBLL.Interfaces;
    using StoreBLL.Models;

    using StoreDAL.Entities;
    using StoreDAL.Interfaces;

    /// <summary>
    /// Service for managing order line items (OrderDetail).
    /// Provides CRUD operations and mapping between DAL entities and BLL models.
    /// Handles validation of product references and maintains order integrity.
    /// </summary>
    public sealed class OrderDetailService : ICrud
    {
        private readonly IOrderDetailRepository orderDetailRepository;
        private readonly IProductRepository productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailService"/> class.
        /// </summary>
        /// <param name="orderDetailRepository">Repository for order detail operations.</param>
        /// <param name="productRepository">Repository for product validation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any dependency is null.</exception>
        public OrderDetailService(
            IOrderDetailRepository orderDetailRepository,
            IProductRepository productRepository)
        {
            ArgumentNullException.ThrowIfNull(orderDetailRepository);
            ArgumentNullException.ThrowIfNull(productRepository);

            this.orderDetailRepository = orderDetailRepository;
            this.productRepository = productRepository;
        }

        /// <summary>
        /// Retrieves all order details from the repository.
        /// </summary>
        /// <returns>Collection of all order details as <see cref="AbstractModel"/> instances.</returns>
        public IEnumerable<AbstractModel> GetAll()
        {
            return this.orderDetailRepository
                .GetAll()
                .Select(e => (AbstractModel)MapToModel(e))
                .ToList();
        }

        /// <summary>
        /// Retrieves a specific order detail by its unique identifier.
        /// </summary>
        /// <param name="id">Order detail identifier.</param>
        /// <returns>Order detail model.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the entity is not found.</exception>
        public AbstractModel GetById(int id)
        {
            var entity = this.orderDetailRepository.GetById(id)
                ?? throw new KeyNotFoundException($"Order detail with id {id} not found.");

            return MapToModel(entity);
        }

        /// <summary>
        /// Adds a new order detail to the database.
        /// Validates that the referenced product exists before insertion.
        /// </summary>
        /// <param name="model">Order detail model to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when model is null.</exception>
        /// <exception cref="ArgumentException">Thrown when model type is invalid or quantity/price is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the product does not exist.</exception>
        public void Add(AbstractModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (model is not OrderDetailModel m)
            {
                throw new ArgumentException("Model must be OrderDetailModel", nameof(model));
            }

            if (m.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(model));
            }

            if (m.UnitPrice < 0)
            {
                throw new ArgumentException("Price cannot be negative.", nameof(model));
            }

            // Validate product existence
            if (this.productRepository.GetByIdWithIncludes(m.ProductId) is null)
            {
                throw new InvalidOperationException($"Product with id {m.ProductId} not found.");
            }

            var entity = MapToEntity(m);
            this.orderDetailRepository.Add(entity);
        }

        /// <summary>
        /// Updates an existing order detail in the database.
        /// Validates that both the order detail and the referenced product exist.
        /// </summary>
        /// <param name="model">Order detail model with updated data.</param>
        /// <exception cref="ArgumentNullException">Thrown when model is null.</exception>
        /// <exception cref="ArgumentException">Thrown when model type is invalid or quantity/price is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when entity does not exist or product not found.</exception>
        public void Update(AbstractModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (model is not OrderDetailModel m)
            {
                throw new ArgumentException("Model must be OrderDetailModel", nameof(model));
            }

            if (m.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(model));
            }

            if (m.UnitPrice < 0)
            {
                throw new ArgumentException("Price cannot be negative.", nameof(model));
            }

            var existing = this.orderDetailRepository.GetById(m.Id)
                ?? throw new InvalidOperationException($"Order detail with id {m.Id} not found.");

            // Validate product existence
            if (this.productRepository.GetByIdWithIncludes(m.ProductId) is null)
            {
                throw new InvalidOperationException($"Product with id {m.ProductId} not found.");
            }

            existing.OrderId = m.OrderId;
            existing.ProductId = m.ProductId;
            existing.ProductAmount = m.Quantity;
            existing.Price = m.UnitPrice;

            this.orderDetailRepository.Update(existing);
        }

        /// <summary>
        /// Deletes an order detail by its identifier.
        /// Idempotent operation - no error if the order detail doesn't exist.
        /// </summary>
        /// <param name="modelId">Order detail identifier to delete.</param>
        public void Delete(int modelId)
        {
            this.orderDetailRepository.DeleteById(modelId);
        }

        /// <summary>
        /// Maps an <see cref="OrderDetail"/> entity to <see cref="OrderDetailModel"/>.
        /// Converts database representation to business logic model.
        /// </summary>
        /// <param name="entity">Entity to map.</param>
        /// <returns>Mapped business model.</returns>
        private static OrderDetailModel MapToModel(OrderDetail entity)
        {
            return new OrderDetailModel(
                entity.Id,
                entity.OrderId,
                entity.ProductId,
                entity.ProductAmount,
                entity.Price);
        }

        /// <summary>
        /// Maps an <see cref="OrderDetailModel"/> to <see cref="OrderDetail"/> entity.
        /// Converts business logic model to database representation.
        /// </summary>
        /// <param name="model">Model to map.</param>
        /// <returns>Mapped database entity.</returns>
        private static OrderDetail MapToEntity(OrderDetailModel model)
        {
            return new OrderDetail(
                model.Id,
                model.OrderId,
                model.ProductId,
                model.Quantity,
                model.UnitPrice);
        }
    }
}
