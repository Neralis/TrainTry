using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TrainTry.Configuration;
using TrainTry.Interfaces;
using TrainTry.Models;

namespace TrainTry.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MemorableDatesController : ControllerBase
    {
        private readonly IMemorableDatesService _memorableDatesService;
        private readonly ILogger<MemorableDatesController> _logger;

        public MemorableDatesController(IMemorableDatesService memorableDatesService, ILogger<MemorableDatesController> logger)
        {
            _memorableDatesService = memorableDatesService;
            _logger = logger;
            _logger.LogDebug(1, "NLog внедрен в MemorableDatesController");
        }

        [HttpPut("PutMemorablesDates", Name = "PutMemorablesDates")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutMemorablesDates(DateTime eventDate, string notificationText, string author)
        {
            try
            {
                var memorableDate = await _memorableDatesService.AddMemorableDate(eventDate, notificationText, author);
                return Ok(memorableDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании памятной даты '{notificationText}'", notificationText);
                return StatusCode(500, "Произошла ошибка при создании памятной даты");
            }
        }

        [HttpGet("GetMemorablesDates", Name = "GetMemorablesDates")]
        public async Task<IActionResult> GetMemorablesDates(DateTime date)
        {
            try
            {
                var dates = await _memorableDatesService.GetMemorableDatesByDate(date);
                return Ok(dates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении памятного события за дату '{date}'", date);
                return StatusCode(500, "Произошла ошибка при получении памятной даты");
            }
        }

        [HttpDelete("DeleteMemorablesDates", Name = "DeleteMemorablesDates")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteMemorablesDates(int id)
        {
            try
            {
                await _memorableDatesService.DeleteMemorableDate(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Памятная дата не найдена.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении памятной даты с id '{id}'", id);
                return StatusCode(500, "Произошла ошибка при удалении памятной даты");
            }
        }
    }
}
