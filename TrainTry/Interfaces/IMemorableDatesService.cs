using TrainTry.Models;

namespace TrainTry.Interfaces
{
    public interface IMemorableDatesService
    {
        Task<MemorableDates> AddMemorableDate(DateTime eventDate, string notificationText, string author);
        Task<List<MemorableDates>> GetMemorableDatesByDate(DateTime date);
        Task DeleteMemorableDate(int id);
    }
}
