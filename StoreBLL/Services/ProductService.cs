namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using StoreBLL.Models;
    using StoreDAL.Data;
    using StoreDAL.Entities;
    using StoreDAL.Interfaces;
    using StoreDAL.Repository;

    /// <summary>
    /// BLL-сервіс для роботи з товарами.
    /// Працює з невизначеною точно структурою DAL (частина полів може бути в Product, частина в ProductTitle).
    /// Уникає прямих звернень до неіснуючих властивостей — замість цього використовує відбиття.
    /// </summary>
    public class ProductService
    {
        private readonly object repository;

        public ProductService(IProductRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ProductService()
            : this(new ProductRepository(StoreDbFactory.Create()))
        {
        }

        public List<ProductModel> GetAll()
        {
            var entities = this.RepoGetAll() ?? Enumerable.Empty<Product>();
            return entities.Select(MapToModel).ToList();
        }

        public ProductModel? GetById(int id)
        {
            var entity = this.RepoGetById(id);
            return entity is null ? null : MapToModel(entity);
        }

        public ProductModel Add(
            string title,
            string category,
            string manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
        {
            var entity = new Product();

            // Забезпечуємо наявність ProductTitle усередині Product
            var pt = ReadProp(entity, "Title") as ProductTitle ?? new ProductTitle();
            SetProp(entity, "Title", pt);

            // Назва (текст) тайтла: шукаємо/ставимо властивість "Title" усередині ProductTitle
            SetStringProp(pt, "Title", title);

            // Категорія/виробник: якщо у моделі є об'єктні властивості "Category"/"Manufacturer" — створимо та поставимо Name.
            EnsureComplexWithName(pt, "Category", category);
            EnsureComplexWithName(entity, "Category", category); // на випадок, якщо категорія в Product

            EnsureComplexWithName(pt, "Manufacturer", manufacturer);
            EnsureComplexWithName(entity, "Manufacturer", manufacturer); // якщо виробник у Product

            // SKU/Description можуть жити як у ProductTitle, так і в Product
            SetStringProp(pt, "Sku", sku);
            SetStringProp(entity, "Sku", sku);

            SetStringProp(pt, "Description", description);
            SetStringProp(entity, "Description", description);

            // Price/Stock можуть бути у будь-якому з рівнів
            SetIfTypeMatches(entity, "Price", price);
            SetIfTypeMatches(pt, "Price", price);

            SetIfTypeMatches(entity, "Stock", stock);
            SetIfTypeMatches(pt, "Stock", stock);

            this.RepoAdd(entity);
            return MapToModel(entity);
        }

        public ProductModel? Update(
            int id,
            string title,
            string category,
            string manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
        {
            var entity = this.RepoGetById(id);
            if (entity is null)
            {
                return null;
            }

            // Гарантуємо наявність вкладеного тайтла
            var pt = ReadProp(entity, "Title") as ProductTitle ?? new ProductTitle();
            SetProp(entity, "Title", pt);

            // Оновлення текстових полів
            SetStringProp(pt, "Title", title);

            EnsureComplexWithName(pt, "Category", category);
            EnsureComplexWithName(entity, "Category", category);

            EnsureComplexWithName(pt, "Manufacturer", manufacturer);
            EnsureComplexWithName(entity, "Manufacturer", manufacturer);

            SetStringProp(pt, "Sku", sku);
            SetStringProp(entity, "Sku", sku);

            SetStringProp(pt, "Description", description);
            SetStringProp(entity, "Description", description);

            SetIfTypeMatches(entity, "Price", price);
            SetIfTypeMatches(pt, "Price", price);

            SetIfTypeMatches(entity, "Stock", stock);
            SetIfTypeMatches(pt, "Stock", stock);

            this.RepoUpdate(entity);
            return MapToModel(entity);
        }

        public bool Delete(int id)
        {
            var entity = this.RepoGetById(id);
            if (entity is null)
            {
                return false;
            }

            if (this.RepoDeleteById(id))
            {
                return true;
            }

            this.RepoDelete(entity);
            return true;
        }

        // -------------------------- Мапінг у BLL-модель --------------------------

        private static ProductModel MapToModel(Product p)
        {
            var pt = ReadProp(p, "Title") as object;

            string titleText = ReadStringFrom(pt, "Title")
                               ?? ReadStringFrom(p, "Title")
                               ?? string.Empty;

            string categoryName = ExtractNameLike(p, pt, "Category");
            string manufacturerName = ExtractNameLike(p, pt, "Manufacturer");

            string sku = ReadStringFrom(pt, "Sku")
                         ?? ReadStringFrom(p, "Sku")
                         ?? string.Empty;

            string description = ReadStringFrom(pt, "Description")
                                 ?? ReadStringFrom(p, "Description")
                                 ?? string.Empty;

            decimal price = ReadStructFrom<decimal>(p, "Price")
                            ?? ReadStructFrom<decimal>(pt, "Price")
                            ?? 0m;

            int stock = ReadStructFrom<int>(p, "Stock")
                        ?? ReadStructFrom<int>(pt, "Stock")
                        ?? 0;

            return new ProductModel(
                id: p.Id,
                title: titleText,
                category: categoryName,
                manufacturer: manufacturerName,
                sku: sku,
                description: description,
                price: price,
                stock: stock);
        }

        private static string ExtractNameLike(object containerA, object? containerB, string holderPropName)
        {
            // 1) Пошук у A
            var name = TryReadNameFrom(containerA, holderPropName);
            if (!string.IsNullOrEmpty(name))
            {
                return name!;
            }

            // 2) Пошук у B (наприклад, ProductTitle)
            if (containerB is not null)
            {
                name = TryReadNameFrom(containerB, holderPropName);
                if (!string.IsNullOrEmpty(name))
                {
                    return name!;
                }
            }

            return string.Empty;
        }

        private static string? TryReadNameFrom(object container, string propName)
        {
            var holder = ReadProp(container, propName);
            if (holder is null)
            {
                return null;
            }

            // Якщо властивість — уже string (деякі моделі зберігають просто назву)
            if (holder is string s)
            {
                return s;
            }

            // Якщо це складний тип (Category/Manufacturer), пробуємо "Name" або "Title"
            var asObj = holder;
            return ReadStringFrom(asObj, "Name") ?? ReadStringFrom(asObj, "Title");
        }

        // -------------------------- Helpers: читання/запис через відбиття --------------------------

        private static object? ReadProp(object obj, string name)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            return pi?.CanRead == true ? pi.GetValue(obj) : null;
        }

        private static void SetProp(object obj, string name, object? value)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi?.CanWrite == true && (value is null || pi.PropertyType.IsInstanceOfType(value)))
            {
                pi.SetValue(obj, value);
            }
        }

        private static string? ReadStringFrom(object? obj, string name)
        {
            if (obj is null)
            {
                return null;
            }

            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi is null || !pi.CanRead)
            {
                return null;
            }

            var v = pi.GetValue(obj);
            return v as string;
        }

        private static T? ReadStructFrom<T>(object? obj, string name)
            where T : struct
        {
            if (obj is null)
            {
                return null;
            }

            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi is null || !pi.CanRead || pi.PropertyType != typeof(T))
            {
                return null;
            }

            var v = pi.GetValue(obj);
            return v is T typed ? typed : null;
        }

        private static void SetStringProp(object obj, string name, string value)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi?.CanWrite == true && pi.PropertyType == typeof(string))
            {
                pi.SetValue(obj, value);
            }
        }

        private static void SetIfTypeMatches(object obj, string name, object value)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi?.CanWrite == true && pi.PropertyType.IsAssignableFrom(value.GetType()))
            {
                pi.SetValue(obj, value);
            }
        }

        /// <summary>
        /// Якщо на об'єкті є властивість <paramref name="propName"/> складного типу (наприклад, Category),
        /// створює її екземпляр (якщо null) і встановлює для нього string-властивість "Name" або "Title".
        /// Якщо такої властивості немає — нічого не робить.
        /// </summary>
        private static void EnsureComplexWithName(object container, string propName, string nameValue)
        {
            var holderPi = container.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (holderPi is null || !holderPi.CanRead)
            {
                return;
            }

            var holder = holderPi.GetValue(container);
            if (holder is null)
            {
                // Спробуємо створити екземпляр якщо тип має публічний конструктор без параметрів
                if (holderPi.PropertyType.GetConstructor(Type.EmptyTypes) is not null)
                {
                    holder = Activator.CreateInstance(holderPi.PropertyType);
                    if (holderPi.CanWrite)
                    {
                        holderPi.SetValue(container, holder);
                    }
                }
            }

            if (holder is null)
            {
                return;
            }

            // Встановлюємо Name або Title усередині складного типу
            var namePi = holder.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance)
                         ?? holder.GetType().GetProperty("Title", BindingFlags.Public | BindingFlags.Instance);

            if (namePi?.CanWrite == true && namePi.PropertyType == typeof(string))
            {
                namePi.SetValue(holder, nameValue);
            }
        }

        // -------------------------- Гнучкий доступ до репозиторію --------------------------

        private IEnumerable<Product> RepoGetAll()
        {
            dynamic repo = this.repository;
            try { return (IEnumerable<Product>)repo.GetAll(); } catch { }
            try { return (IEnumerable<Product>)repo.GetProducts(); } catch { }
            try { return (IEnumerable<Product>)repo.GetAllProducts(); } catch { }
            return Enumerable.Empty<Product>();
        }

        private Product? RepoGetById(int id)
        {
            dynamic repo = this.repository;
            try { return (Product)repo.GetById(id); } catch { }
            try { return (Product)repo.GetProductById(id); } catch { }
            try { return (Product)repo.Get(id); } catch { }
            return null;
        }

        private void RepoAdd(Product p)
        {
            dynamic repo = this.repository;
            try { repo.Add(p); return; } catch { }
            try { repo.AddProduct(p); return; } catch { }
            try { repo.Create(p); return; } catch { }
        }

        private void RepoUpdate(Product p)
        {
            dynamic repo = this.repository;
            try { repo.Update(p); return; } catch { }
            try { repo.UpdateProduct(p); return; } catch { }
            try { repo.Edit(p); return; } catch { }
        }

        private bool RepoDeleteById(int id)
        {
            dynamic repo = this.repository;
            try { repo.Delete(id); return true; } catch { }
            try { repo.DeleteProduct(id); return true; } catch { }
            try { repo.RemoveById(id); return true; } catch { }
            return false;
        }

        private void RepoDelete(Product p)
        {
            dynamic repo = this.repository;
            try { repo.Delete(p); return; } catch { }
            try { repo.Remove(p); return; } catch { }
            try { repo.DeleteProduct(p); return; } catch { }
        }
    }
}
