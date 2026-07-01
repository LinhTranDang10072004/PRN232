using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IMonthClosingRepository
    {
        Task<MonthClosing?> GetForMonthAsync(int userId, int month, int year);
        Task<bool> ExistsForMonthAsync(int userId, int month, int year);
        Task AddAsync(MonthClosing closing);
    }
}
