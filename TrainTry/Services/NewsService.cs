using static TrainTry.Controllers.NewsController;
using TrainTry.Configuration;
using TrainTry.Interfaces;
using TrainTry.Models;
using Microsoft.EntityFrameworkCore;

namespace TrainTry.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<NewsService> _logger;

        public NewsService(ApplicationContext context, ILogger<NewsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<News> CreateNews(DateTime dateBegin, DateTime dateEnd, string topic, string article, int importance, string author)
        {
            _logger.LogInformation("Попытка создать новость '{article}'", article);

            News news = new News
            {
                dateBegin = dateBegin,
                dateEnd = dateEnd,
                topic = topic,
                article = article,
                importance = importance,
                datePublish = DateTime.Now,
                author = author
            };

            try
            {
                _context.News.Add(news);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Новость '{article}' успешно создана", article);
                return news;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании новости '{article}'", article);
                throw new InvalidOperationException("Произошла ошибка при создании новости", ex);
            }
        }

        public async Task<List<News>> GetNews()
        {
            _logger.LogInformation("Попытка получить все новости");

            try
            {
                var news = await _context.News.ToListAsync();
                _logger.LogInformation("Попытка получить все новости завершилась успешно");
                return news;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении новостей");
                throw new InvalidOperationException("Ошибка при получении новостей", ex);
            }
        }

        public async Task<Combine> GetNewsBySingleDate(DateTime date)
        {
            date = date.Date; // Удаление времени, оставляя только дату

            // Фильтрация новостей за дату
            var news = await _context.News
                .Where(n => n.dateBegin.Date <= date && date <= n.dateEnd.Date)
                .ToListAsync();

            var memodates = await _context.MemorableDates
                .Where(s => s.EventDate.Date == date)
                .Select(s => s.NotificationText)
                .ToListAsync();

            var combine = new Combine
            {
                News = news,
                MemorableDates = memodates,
            };

            _logger.LogInformation("Новости за дату получены успешно");
            return combine;
        }

        public class Combine
        {
            public List<News> News { get; set; }
            public List<string> MemorableDates { get; set; }
        }


        public async Task<List<News>> GetNewsByDate(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            if (startDate > endDate)
            {
                _logger.LogInformation("Неверно введен диапазон даты (Дата начала не может быть позже даты окончания)");
                throw new ArgumentException("Дата начала не может быть позже даты окончания");
            }

            var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            // Фильтрация новостей по диапазону дат
            var news = await _context.News
                                     .Where(n => DateTime.SpecifyKind(n.dateBegin, DateTimeKind.Utc) >= startDateUtc && DateTime.SpecifyKind(n.dateEnd, DateTimeKind.Utc) <= endDateUtc)
                                     .ToListAsync();

            _logger.LogInformation("Новости за диапазон дат получены успешно");
            return news;
        }

        public async Task DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                _logger.LogWarning("Попытка удалить несуществующую новость с id '{id}'", id);
                throw new KeyNotFoundException("Новость не найдена");
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Новость с id '{id}' успешно удалена", id);
        }
    }
}
