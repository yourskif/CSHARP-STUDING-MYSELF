namespace StoreBLL.Models
{
    public class CustomerOrderModel : AbstractModel
    {
        public CustomerOrderModel() : base()
        {
        }

        public CustomerOrderModel(int id, int userId, string operationTime, int orderStateId)
            : base(id)
        {
            this.UserId = userId;
            this.OperationTime = operationTime;
            this.OrderStateId = orderStateId;
        }

        public int UserId { get; set; }
        public string OperationTime { get; set; }
        public int OrderStateId { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id}, UserId: {this.UserId}, Time: {this.OperationTime}, StateId: {this.OrderStateId}";
        }
    }
}