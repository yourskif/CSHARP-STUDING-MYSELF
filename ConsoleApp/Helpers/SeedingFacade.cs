using System;
using System.Reflection;

using StoreDAL.Data;

namespace ConsoleApp.Helpers
{
    /// <summary>
    /// Thin reflection-based adapter that calls into StoreDAL.Data.InitDataFactory.TestDataFactory
    /// without taking a hard compile-time dependency on a specific method name.
    /// Tries methods in this order: Seed(StoreDbContext), SeedIfEmpty(StoreDbContext).
    /// </summary>
    public static class SeedingFacade
    {
        private const string SeedNamespaceType = "StoreDAL.Data.InitDataFactory.TestDataFactory";

        public static bool TrySeed(StoreDbContext db, Action<string>? log = null)
        {
            ArgumentNullException.ThrowIfNull(db);

            var type = Type.GetType(SeedNamespaceType);
            if (type == null)
            {
                log?.Invoke($"[Seeding] Type not found: {SeedNamespaceType}");
                return false;
            }

            // Try preferred names
            var method = type.GetMethod("Seed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                      ?? type.GetMethod("SeedIfEmpty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
            {
                log?.Invoke("[Seeding] No suitable method found (Seed / SeedIfEmpty).");
                return false;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(StoreDbContext))
            {
                log?.Invoke($"[Seeding] Method signature mismatch: {method.Name}(StoreDbContext) expected.");
                return false;
            }

            try
            {
                method.Invoke(null, new object[] { db });
                log?.Invoke($"[Seeding] Invoked {type.FullName}.{method.Name}()");
                return true;
            }
            catch (TargetInvocationException tie)
            {
                log?.Invoke($"[Seeding] {method.Name} threw: {tie.InnerException?.Message ?? tie.Message}");
                return false;
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Seeding] Failed: {ex.Message}");
                return false;
            }
        }
    }
}
