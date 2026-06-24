namespace BusinessObjects.Enums
{
    /// <summary>
    /// Nhánh 1: User (CompanyId = null).
    /// Nhánh 2: CompanyAdmin / CompanyStaff (CompanyId bắt buộc).
    /// </summary>
    public enum UserRole
    {
        User = 0,
        CompanyAdmin = 1,
        CompanyStaff = 2
    }
}
