namespace StoreBLL.Models
{
    /// <summary>
    /// Detailed product model (СЂРѕР·С€РёСЂСЋС” ProductModel).
    /// Р’РёРєРѕСЂРёСЃС‚РѕРІСѓС”С‚СЊСЃСЏ СЃРµСЂРІС–СЃРѕРј РґР»СЏ РїРѕРІРµСЂРЅРµРЅРЅСЏ РґРµС‚Р°Р»СЊРЅРѕС— С–РЅС„РѕСЂРјР°С†С–С— РїСЂРѕ С‚РѕРІР°СЂ.
    /// </summary>
    public class ProductDetailsModel : ProductModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsModel"/> class.
        /// </summary>
        public ProductDetailsModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsModel"/> class.
        /// Initializes a new instance with model-typed category/manufacturer.
        /// </summary>
        public ProductDetailsModel(
            int id,
            string title,
            CategoryModel category,
            ManufacturerModel manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
            : base(id, title, category, manufacturer, sku, description, price, stock)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsModel"/> class.
        /// Initializes a new instance with string category/manufacturer (РґР»СЏ СЃСѓРјС–СЃРЅРѕСЃС‚С– Р·С– СЃС‚Р°СЂРёРјРё РІРёРєР»РёРєР°РјРё).
        /// </summary>
        public ProductDetailsModel(
            int id,
            string title,
            string category,
            string manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
            : base(id, title, category, manufacturer, sku, description, price, stock)
        {
        }
    }
}
