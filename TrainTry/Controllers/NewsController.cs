using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainTry.Configuration;
using TrainTry.Interfaces;
using TrainTry.Models;
using static TrainTry.Services.NewsService;

namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPut("PutNews", Name = "PutNews")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutNews(DateTime dateBegin, DateTime dateEnd, string topic, string article, int importance, string author)
        {
            try
            {
                var news = await _newsService.CreateNews(dateBegin, dateEnd, topic, article, importance, author);
                return Ok(news);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetNews", Name = "GetNews")]
        [AllowAnonymous]
        public async Task<ActionResult<List<News>>> GetNews()
        {
            try
            {
                var news = await _newsService.GetNews();
                return Ok(news);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetNewsBySingleDate", Name = "GetNewsBySingleDate")]
        [AllowAnonymous]
        public async Task<ActionResult<Combine>> GetNewsBySingleDate(DateTime date)
        {
            try
            {
                var combine = await _newsService.GetNewsBySingleDate(date);
                return Ok(combine);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetNewsByDate", Name = "GetNewsByDate")]
        [AllowAnonymous]
        public async Task<ActionResult<List<News>>> GetNewsByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                var news = await _newsService.GetNewsByDate(startDate, endDate);
                return Ok(news);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteNews", Name = "DeleteNews")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                await _newsService.DeleteNews(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
