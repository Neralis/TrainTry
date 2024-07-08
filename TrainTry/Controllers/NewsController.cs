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

        [HttpPut(Name = "PutNews")]
        public async void PutNews(int id, DateTime dateBegin, DateTime dateEnd, string topic, string article, int importance, string author)
        {
            dateBegin = DateTime.SpecifyKind(dateBegin, DateTimeKind.Utc);
            dateEnd = DateTime.SpecifyKind(dateEnd, DateTimeKind.Utc);

            News news = new News
            {
                id = id,
                dateBegin = dateBegin,
                dateEnd = dateEnd,
                topic = topic,
                article = article,
                importance = importance,
                datePublish = DateTime.UtcNow,
                author = author
            };

            _context.News.Add(news);
            _context.SaveChanges();
        }

        [HttpGet(Name = "GetNews")]
        public async Task<ActionResult<List<News>>> GetNews()
        {
            var news = await _context.News.ToListAsync();
            return news;
        }

        [HttpDelete(Name = "DeleteNews")]
        public async void DeleteNews(int id)
        {
            News news = new News
            {
                id = id
            };
            
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
        }
    }
}
