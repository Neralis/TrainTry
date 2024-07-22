using static TrainTry.Controllers.NewsController;
using TrainTry.Models;
using static TrainTry.Services.NewsService;

namespace TrainTry.Interfaces
{
    public interface INewsService
    {
        Task<News> CreateNews(DateTime dateBegin, DateTime dateEnd, string topic, string article, int importance, string author);
        Task<List<News>> GetNews();
        Task<Combine> GetNewsBySingleDate(DateTime date);
        Task<List<News>> GetNewsByDate(DateTime startDate, DateTime endDate);
        Task DeleteNews(int id);
    }
}
