namespace StoreBLL.Models
{
    public class ProductModel : AbstractModel
    {
        public ProductModel() : base()
        {
        }

        public ProductModel(int id, int titleId, int manufacturerId, decimal price, string description)
            : base(id)
        {
            this.TitleId = titleId;
            this.ManufacturerId = manufacturerId;
            this.Price = price;
            this.Description = description;
        }

        public int TitleId { get; set; }
        public int ManufacturerId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id}, TitleId: {this.TitleId}, Price: {this.Price:C}, Description: {this.Description}";
        }
    }
}