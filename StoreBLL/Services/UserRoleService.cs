// Path: console-online-store/StoreBLL/Services/UserRoleService.cs
namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoreBLL.Models;

    using StoreDAL.Entities;
    using StoreDAL.Interfaces;
    using StoreDAL.UnitOfWork;

    /// <summary>
    /// Service for managing user roles in the business logic layer.
    /// Provides CRUD operations for user roles with validation.
    /// </summary>
    public sealed class UserRoleService
    {
        private readonly IStoreUnitOfWork unitOfWork;
        private readonly IUserRoleRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleService"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="unitOfWork"/> is <see langword="null"/>.</exception>
        public UserRoleService(IStoreUnitOfWork unitOfWork)
        {
            ArgumentNullException.ThrowIfNull(unitOfWork);
            this.unitOfWork = unitOfWork;
            this.repository = new StoreDAL.Repository.UserRoleRepository(unitOfWork.Context);
        }

        /// <summary>
        /// Gets all user roles.
        /// </summary>
        /// <returns>Collection of all user roles as <see cref="UserRoleModel"/> instances.</returns>
        public IEnumerable<UserRoleModel> GetAll()
        {
            return this.repository.GetAll()
                .Select(r => new UserRoleModel { Id = r.Id, RoleName = r.RoleName })
                .ToList();
        }

        /// <summary>
        /// Gets a user role by its unique identifier.
        /// </summary>
        /// <param name="id">Role identifier.</param>
        /// <returns>
        /// <see cref="UserRoleModel"/> instance when found; otherwise, <see langword="null"/>.
        /// </returns>
        public UserRoleModel? GetById(int id)
        {
            var entity = this.repository.GetById(id);
            return entity == null ? null : new UserRoleModel { Id = entity.Id, RoleName = entity.RoleName };
        }

        /// <summary>
        /// Gets a user role by name.
        /// </summary>
        /// <param name="name">Role name.</param>
        /// <returns>
        /// <see cref="UserRoleModel"/> instance when found; otherwise, <see langword="null"/>.
        /// </returns>
        public UserRoleModel? GetByName(string name)
        {
            var entity = this.repository.GetByName(name);
            return entity == null ? null : new UserRoleModel { Id = entity.Id, RoleName = entity.RoleName };
        }

        /// <summary>
        /// Adds a new user role.
        /// Validates that the role name is not empty.
        /// </summary>
        /// <param name="model">Role model to add.</param>
        /// <returns>Created role model with assigned identifier.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when role name is null or whitespace.</exception>
        public UserRoleModel Add(UserRoleModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (string.IsNullOrWhiteSpace(model.RoleName))
            {
                throw new ArgumentException("Role name cannot be empty.", nameof(model));
            }

            var entity = new UserRole { RoleName = model.RoleName };
            this.repository.Add(entity);
            this.unitOfWork.SaveChanges();

            return new UserRoleModel { Id = entity.Id, RoleName = entity.RoleName };
        }

        /// <summary>
        /// Updates an existing user role.
        /// Validates that the new name is not empty.
        /// </summary>
        /// <param name="model">Role model with updated data.</param>
        /// <returns><see langword="true"/> if the role was updated; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when role name is null or whitespace.</exception>
        public bool Update(UserRoleModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = this.repository.GetById(model.Id);
            if (entity is null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.RoleName))
            {
                throw new ArgumentException("Role name cannot be empty.", nameof(model));
            }

            entity.RoleName = model.RoleName;
            this.repository.Update(entity);
            this.unitOfWork.SaveChanges();

            return true;
        }

        /// <summary>
        /// Deletes a user role by its identifier.
        /// </summary>
        /// <param name="id">Role identifier.</param>
        /// <returns><see langword="true"/> if the role was deleted; otherwise, <see langword="false"/>.</returns>
        public bool Delete(int id)
        {
            var entity = this.repository.GetById(id);
            if (entity is null)
            {
                return false;
            }

            this.repository.DeleteById(id);
            this.unitOfWork.SaveChanges();

            return true;
        }
    }
}
