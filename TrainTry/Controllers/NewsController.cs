using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class NewsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public NewsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPut("PutNews", Name = "PutNews")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutNews(DateTime dateBegin, DateTime dateEnd, string topic, string article, int importance, string author)
        {
            dateBegin = DateTime.SpecifyKind(dateBegin, DateTimeKind.Utc);
            dateEnd = DateTime.SpecifyKind(dateEnd, DateTimeKind.Utc);

            News news = new News
            {
                dateBegin = dateBegin,
                dateEnd = dateEnd,
                topic = topic,
                article = article,
                importance = importance,
                datePublish = DateTime.UtcNow,
                author = author
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return Ok(news);
        }

        [HttpGet("GetNews", Name = "GetNews")]
        [AllowAnonymous]
        public async Task<ActionResult<List<News>>> GetNews()
        {
            var news = await _context.News.ToListAsync();
            return news;
        }

        [HttpGet("GetNewsByDate", Name = "GetNewsByDate")]
        [AllowAnonymous]
        public async Task<ActionResult<List<News>>> GetNewsByDate(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Дата начала не может быть позже даты окончания");
            }

            var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            // Фильтрация новостей по диапазону дат
            var news = await _context.News
                                     .Where(n => DateTime.SpecifyKind(n.dateBegin, DateTimeKind.Utc) >= startDateUtc && DateTime.SpecifyKind(n.dateEnd, DateTimeKind.Utc) <= endDateUtc)
                                     .ToListAsync();

            return Ok(news); // Используем Ok для возврата успешного результата
        }

        [HttpDelete("DeleteNews", Name = "DeleteNews")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
