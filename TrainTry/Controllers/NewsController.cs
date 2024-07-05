using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {

        [HttpPut(Name = "PutNews")]
        public News PutNews(int id, DateTime dateBegin, DateTime dateEnd, string topic, string article, int importance, DateTime datePublish, string author)
        {
            dateBegin = DateTime.SpecifyKind(dateBegin, DateTimeKind.Utc);
            dateEnd = DateTime.SpecifyKind(dateEnd, DateTimeKind.Utc);
            datePublish = DateTime.SpecifyKind(datePublish, DateTimeKind.Utc);

            using (ApplicationContext db = new ApplicationContext())
            {
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
                db.News.Add(news);
                db.SaveChanges();
                return news;
            }
        }

        [HttpGet(Name = "GetNews")]
        public List<News> GetNews()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var news = db.News.ToList();
                return news;
            }
        }
    }
}
