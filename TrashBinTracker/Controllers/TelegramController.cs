using Microsoft.AspNetCore.Mvc;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;
using TrashBinTracker.Service;

namespace TrashBinTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly ITrashRepository _trashRepository;

        private readonly TelegramService _telegramService;

        public TelegramController(
            ITrashRepository trashRepository,
            TelegramService telegramService)
        {
            _trashRepository = trashRepository;
            _telegramService = telegramService;
        }

        [HttpPost("status")]
        public async Task<IActionResult> Status()
        {
            IEnumerable<TrashBin> bins =
                _trashRepository.GetAll();

            string message =
                "Status på skraldespande:\n\n";

            foreach (TrashBin bin in bins)
            {
                message +=
                    $"{bin.Name}: {bin.FillLevel}% fuld\n";
            }

            await _telegramService.SendMessage(message);

            return Ok(message);
        }

        [HttpPost("tomning")]
        public async Task<IActionResult> LastEmpty()
        {
            IEnumerable<TrashBin> bins =
                _trashRepository.GetAll();

            string message =
                "Sidste tømning:\n\n";

            foreach (TrashBin bin in bins)
            {
                message +=
                    $"{bin.Name}: {bin.LastEmptied}\n";
            }

            await _telegramService.SendMessage(message);

            return Ok(message);
        }

        [HttpPost("temperaturewarning")]
        public async Task<IActionResult> TemperatureWarning()
        {
            string message =
                "Temperatur-advarsel: Madaffald er over 50% fyldt og temperaturen er over 20°C.";

            await _telegramService.SendMessage(message);

            return Ok(message);
        }

        [HttpPost("fullwarning")]
        public async Task<IActionResult> FullWarning()
        {
            string message =
                "Advarsel: En skraldespand er næsten fuld eller fuld.";

            await _telegramService.SendMessage(message);

            return Ok(message);
        }

        [HttpPost("48hourswarning")]
        public async Task<IActionResult> TimeWarning()
        {
            string message =
                "Påmindelse: En skraldespand er ikke blevet tømt i over 48 timer.";

            await _telegramService.SendMessage(message);

            return Ok(message);
        }
    }
}