namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

/// <summary>
/// Service for managing user roles with full CRUD operations.
/// Provides business logic layer for user role entities which define access permissions in the system.
/// </summary>
/// <remarks>
/// Standard user roles include:
/// <list type="bullet">
/// <item><description>1 - Admin: Full system access including user management and configuration</description></item>
/// <item><description>2 - Registered: Standard user with ordering capabilities</description></item>
/// <item><description>3 - Guest: Limited access for browsing products only</description></item>
/// </list>
/// User roles are typically predefined but can be extended for custom access control scenarios.
/// </remarks>
public class UserRoleService : ICrud
{
    private readonly IUserRoleRepository repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRoleService"/> class.
    /// </summary>
    /// <param name="context">EF Core database context for user role operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
    public UserRoleService(StoreDbContext context)
    {
        this.repository = new UserRoleRepository(context);
    }

    /// <summary>
    /// Adds a new user role to the database.
    /// </summary>
    /// <param name="model">User role model containing data to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="model"/> is not of type <see cref="UserRoleModel"/>.</exception>
    /// <remarks>
    /// The role name should be unique and descriptive of the permissions it grants.
    /// </remarks>
    public void Add(AbstractModel model)
    {
        if (model is not UserRoleModel m)
        {
            throw new ArgumentException("Expected UserRoleModel", nameof(model));
        }

        // Map BLL -> DAL
        this.repository.Add(new UserRole(m.Id, m.RoleName));
        this.repository.SaveChanges();
    }

    /// <summary>
    /// Deletes a user role by its identifier.
    /// </summary>
    /// <param name="modelId">The unique identifier of the user role to delete.</param>
    /// <remarks>
    /// Warning: Deleting a user role that is referenced by existing users may cause data integrity issues.
    /// Ensure no users are assigned to this role before deletion, or reassign them to another role.
    /// </remarks>
    public void Delete(int modelId)
    {
        this.repository.DeleteById(modelId);
        this.repository.SaveChanges();
    }

    /// <summary>
    /// Retrieves all user roles from the database.
    /// </summary>
    /// <returns>
    /// Collection of all user roles as <see cref="AbstractModel"/> instances.
    /// Returns the complete set of roles defined in the system.
    /// </returns>
    public IEnumerable<AbstractModel> GetAll()
    {
        // Map DAL -> BLL
        return this.repository
            .GetAll()
            .Select(x => (AbstractModel)new UserRoleModel(x.Id, x.RoleName))
            .ToList();
    }

    /// <summary>
    /// Retrieves a single user role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user role.</param>
    /// <returns>The user role model with the specified identifier.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user role with specified <paramref name="id"/> is not found.</exception>
    public AbstractModel GetById(int id)
    {
        var res = this.repository.GetById(id)
                  ?? throw new InvalidOperationException($"UserRole with id={id} not found");

        // Map DAL -> BLL
        return new UserRoleModel(res.Id, res.RoleName);
    }

    /// <summary>
    /// Updates an existing user role in the database.
    /// </summary>
    /// <param name="model">User role model with updated data.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="model"/> is not of type <see cref="UserRoleModel"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when user role with specified id is not found.</exception>
    /// <remarks>
    /// Changing role names that are hardcoded in the application logic may cause unexpected behavior.
    /// Consider the impact on existing authorization checks before modifying system roles.
    /// </remarks>
    public void Update(AbstractModel model)
    {
        if (model is not UserRoleModel m)
        {
            throw new ArgumentException("Expected UserRoleModel", nameof(model));
        }

        // Map BLL -> DAL
        this.repository.Update(new UserRole(m.Id, m.RoleName));
        this.repository.SaveChanges();
    }
}
