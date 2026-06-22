using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Corporate
{
    public class SetStaffActiveRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
