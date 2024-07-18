using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TrainTry.Configuration;
using TrainTry.Models;

namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MemorableDatesController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<MemorableDatesController> _logger;

        public MemorableDatesController(ApplicationContext context, ILogger<MemorableDatesController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogDebug(1, "NLog внедрен в MemorableDatesController");
        }

        #region [Создание памятных дат]

        [HttpPut("PutMemorablesDates", Name = "PutMemorablesDates")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutMemorablesDates(DateTime eventDate, string notificationText, string author)
        {
            _logger.LogInformation("Попытка установить памятную дату '{eventDate}' для события '{notificationText}'", eventDate, notificationText);

            MemorableDates MemoDates = new MemorableDates
            {
                EventDate = eventDate,
                NotificationText = notificationText,
                Created = DateTime.Now,
                Author = author
            };

            try
            {
                _context.MemorableDates.Add(MemoDates);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Памятная дата '{eventDate}' для события '{notificationText}' установлена", eventDate, notificationText);
                return Ok(MemoDates);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании памятной даты '{notificationText}'", notificationText);
                return StatusCode(500, "Произошла ошибка при создании памятной даты");
            }      
        }

        #endregion

        #region [Получение памятных дат]

        [HttpGet("GetMemorablesDates", Name = "GetMemorablesDates")]
        public async Task<ActionResult> GetMemoDates(DateTime date)
        {
            try
            {
                var news = await _context.MemorableDates
                .Where(s => s.EventDate == date)
                .ToListAsync();

                _logger.LogInformation("Памятная дата за '{date}' получена:", date);
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении памятного события за дату '{date}'", date);
                return StatusCode(500, "Произошла ошибка при получении памятной даты");
            }
            
        }

        #endregion

        #region [Удаление памятных дат]

        [HttpDelete("DeleteMemorablesDates", Name = "DeleteMemorablesDates")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteMemoDates(int id)
        {
            try
            {
                MemorableDates MemoDates = new MemorableDates { Id = id };
                _context.MemorableDates.Remove(MemoDates);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении памятной даты с id: '{id}'", id);
                return StatusCode(500, "Произошла ошибка при удалении памятной даты");
            }

            #endregion

        }
    }
}
