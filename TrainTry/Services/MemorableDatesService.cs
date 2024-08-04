using Microsoft.EntityFrameworkCore;
using TrainTry.Configuration;
using TrainTry.Interfaces;
using TrainTry.Models;

namespace TrainTry.Services
{
    public class MemorableDatesService : IMemorableDatesService
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<MemorableDatesService> _logger;

        public MemorableDatesService(ApplicationContext context, ILogger<MemorableDatesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MemorableDates> AddMemorableDate(DateTime eventDate, string notificationText, string author)
        {
            _logger.LogInformation("Попытка установить памятную дату '{eventDate}' для события '{notificationText}'", eventDate, notificationText);

            var memorableDate = new MemorableDates
            {
                EventDate = eventDate,
                NotificationText = notificationText,
                Created = DateTime.Now,
                Author = author
            };

            try
            {
                _context.MemorableDates.Add(memorableDate);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Памятная дата '{eventDate}' для события '{notificationText}' установлена", eventDate, notificationText);
                return memorableDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании памятной даты '{notificationText}'", notificationText);
                throw;
            }
        }

        public async Task<List<MemorableDates>> GetMemorableDatesByDate(DateTime date)
        {
            try
            {
                var dates = await _context.MemorableDates
                    .Where(md => md.EventDate == date)
                    .ToListAsync();

                _logger.LogInformation("Памятная дата за '{date}' получена", date);
                return dates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении памятного события за дату '{date}'", date);
                throw;
            }
        }

        public async Task DeleteMemorableDate(int id)
        {
            try
            {
                var memorableDate = await _context.MemorableDates.FindAsync(id);
                if (memorableDate == null)
                {
                    _logger.LogWarning("Попытка удалить несуществующую памятную дату с id '{id}'", id);
                    throw new KeyNotFoundException("Памятная дата не найдена.");
                }

                _context.MemorableDates.Remove(memorableDate);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Памятная дата с id '{id}' успешно удалена", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении памятной даты с id '{id}'", id);
                throw;
            }
        }
    }
}
