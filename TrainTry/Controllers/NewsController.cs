using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<News> PutNews(int id, DateTime dateBegin, DateTime dateEnd, string topic, string article, int importance, DateTime datePublish, string author)
        {
            dateBegin = DateTime.SpecifyKind(dateBegin, DateTimeKind.Utc);
            dateEnd = DateTime.SpecifyKind(dateEnd, DateTimeKind.Utc);
            datePublish = DateTime.SpecifyKind(datePublish, DateTimeKind.Utc);

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
            return news;
        }

        [HttpGet(Name = "GetNews")]
        public ActionResult<List<News>> GetNews()
        {
            var news = _context.News.ToList();
            return news;
        }
    }
}
