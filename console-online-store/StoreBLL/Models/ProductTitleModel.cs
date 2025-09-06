namespace StoreBLL.Models
{
    public class ProductTitleModel : AbstractModel
    {
        public ProductTitleModel() : base()
        {
        }

        public ProductTitleModel(int id, string title, int categoryId) : base(id)
        {
            this.Title = title;
            this.CategoryId = categoryId;
        }

        public string Title { get; set; }
        public int CategoryId { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id}, Title: {this.Title}, CategoryId: {this.CategoryId}";
        }
    }
}