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
    /// </summary>
    public class OrderDetailService : ICrud
    {
        private readonly IOrderDetailRepository orderDetailRepository;
        private readonly IProductRepository productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailService"/> class.
        /// </summary>
        /// <param name="orderDetailRepository">Order detail repository.</param>
        /// <param name="productRepository">Product repository.</param>
        /// <exception cref="ArgumentNullException">Thrown when any dependency is null.</exception>
        public OrderDetailService(
            IOrderDetailRepository orderDetailRepository,
            IProductRepository productRepository)
        {
            this.orderDetailRepository = orderDetailRepository
                ?? throw new ArgumentNullException(nameof(orderDetailRepository));
            this.productRepository = productRepository
                ?? throw new ArgumentNullException(nameof(productRepository));
        }

        /// <inheritdoc/>
        public IEnumerable<AbstractModel> GetAll()
        {
            return this.orderDetailRepository
                .GetAll()
                .Select(e => (AbstractModel)MapToModel(e));
        }

        /// <inheritdoc/>
        /// <exception cref="KeyNotFoundException">Thrown when the entity is not found.</exception>
        public AbstractModel GetById(int id)
        {
            var entity = this.orderDetailRepository.GetById(id)
                ?? throw new KeyNotFoundException($"Order detail with id {id} not found.");

            return MapToModel(entity);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Thrown when model type is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the product does not exist.</exception>
        public void Add(AbstractModel model)
        {
            if (model is not OrderDetailModel m)
            {
                throw new ArgumentException("Model must be OrderDetailModel", nameof(model));
            }

            // Validate product existence without introducing an unused local variable
            if (this.productRepository.GetByIdWithIncludes(m.ProductId) is null)
            {
                throw new InvalidOperationException($"Product with id {m.ProductId} not found.");
            }

            var entity = MapToEntity(m);
            this.orderDetailRepository.Add(entity);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Thrown when model type is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when entity does not exist or product not found.</exception>
        public void Update(AbstractModel model)
        {
            if (model is not OrderDetailModel m)
            {
                throw new ArgumentException("Model must be OrderDetailModel", nameof(model));
            }

            var existing = this.orderDetailRepository.GetById(m.Id)
                ?? throw new InvalidOperationException($"Order detail with id {m.Id} not found.");

            // Validate product existence without unused local
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

        /// <inheritdoc/>
        public void Delete(int modelId)
        {
            this.orderDetailRepository.DeleteById(modelId);
        }

        /// <summary>
        /// Maps an <see cref="OrderDetail"/> entity to <see cref="OrderDetailModel"/>.
        /// </summary>
        /// <param name="entity">Entity to map.</param>
        /// <returns>Mapped model.</returns>
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
        /// </summary>
        /// <param name="model">Model to map.</param>
        /// <returns>Mapped entity.</returns>
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
