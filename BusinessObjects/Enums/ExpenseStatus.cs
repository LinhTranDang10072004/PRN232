namespace BusinessObjects.Enums
{
    /// <summary>
    /// Nhánh 1: Completed khi tạo (không cần duyệt).
    /// Nhánh 2: Pending → Approved / Rejected. Approved = khóa sửa/xóa.
    /// </summary>
    public enum ExpenseStatus
    {
        Completed = 0,
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
