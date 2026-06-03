namespace ExpenseManagementAPI.DTOs.Personal
{
    public class ExpenseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Status { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
