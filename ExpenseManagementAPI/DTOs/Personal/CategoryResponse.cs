namespace ExpenseManagementAPI.DTOs.Personal
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Branch { get; set; } = null!;
        /// <summary>Danh mục seed hệ thống — không sửa/xóa qua API.</summary>
        public bool IsSystem { get; set; }
        public int? OwnerUserId { get; set; }
    }
}
