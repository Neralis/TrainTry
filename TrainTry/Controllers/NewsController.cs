using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainTry.Configuration;
using TrainTry.Models;

namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<NewsController> _logger;

        public NewsController(ApplicationContext context, ILogger<NewsController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogDebug(1, "NLog внедрен в NewsController");
        }

        #region [Создание статьи]

        [HttpPut("PutNews", Name = "PutNews")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutNews(DateTime dateBegin, DateTime dateEnd, string topic, string article, int importance, string author)
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
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании новости '{article}'", article);
                return StatusCode(500, "Произошла ошибка при создании новости");
            }
        }

        #endregion

        #region [Выборка всех новостей]

        [HttpGet("GetNews", Name = "GetNews")]
        [AllowAnonymous]
        public async Task<ActionResult<List<News>>> GetNews()
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
                return StatusCode(500, "Ошибка при получении новостей");
            }
        }

        #endregion

        #region [Выборка новостей за определенную дату]

        [HttpGet("GetNewsBySingleDate", Name = "GetNewsBySingleDate")]
        [AllowAnonymous]
        public async Task<ActionResult<Combine>> GetNewsBySingleDate(DateTime date)
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
            return Ok(combine); // Используем Ok для возврата успешного результата
        }

        public class Combine
        {
            public List<News> News { get; set; }
            public List<string> MemorableDates { get; set; }
        }

        #endregion

        #region [Выборка новостей за диапазон дат]

        [HttpGet("GetNewsByDate", Name = "GetNewsByDate")]
        [AllowAnonymous]
        public async Task<ActionResult<List<News>>> GetNewsByDate(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            if (startDate > endDate)
            {
                _logger.LogInformation("Неверно введен диапазон даты (Дата начала не может быть позже даты окончания)");
                return BadRequest("Дата начала не может быть позже даты окончания");
            }

            var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            // Фильтрация новостей по диапазону дат
            var news = await _context.News
                                     .Where(n => DateTime.SpecifyKind(n.dateBegin, DateTimeKind.Utc) >= startDateUtc && DateTime.SpecifyKind(n.dateEnd, DateTimeKind.Utc) <= endDateUtc)
                                     .ToListAsync();

            _logger.LogInformation("Новости за диапазон дат получены успешно");
            return Ok(news); // Используем Ok для возврата успешного результата
        }

        #endregion

        #region [Удаление новостей по id]

        [HttpDelete("DeleteNews", Name = "DeleteNews")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                _logger.LogWarning("Попытка удалить несуществующую новость с id '{id}'", id);
                return NotFound();
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Новость с id '{id}' успешно удалена", id);
            return NoContent();
        }

        #endregion
    }
}
