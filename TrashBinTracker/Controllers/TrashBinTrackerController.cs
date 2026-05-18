using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;

namespace TrashBinTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrashBinTrackerController : Controller
    {
        private readonly ITrashRepository _trashRepository;
        private readonly INotificationRepo _notificationRepo;
        public TrashBinTrackerController(ITrashRepository trashRepository, INotificationRepo notificationRepo)
        {
            _trashRepository = trashRepository;
            _notificationRepo = notificationRepo;
        }

        //accessible for admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<TrashBin> AddTrashBin([FromBody] TrashBin trashBin)
        {
            if (trashBin == null)
            {
                return BadRequest("Trash bin data is required.");
            }
            TrashBin? addedTrashbin = _trashRepository.Add(trashBin);
            if (addedTrashbin == null)
            {
                return StatusCode(500, "An error occurred while adding the trash bin.");
            }
            return CreatedAtAction(nameof(AddTrashBin), new { id = addedTrashbin.Id }, addedTrashbin);
        }

        //accessible for both admin and user
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public ActionResult<IEnumerable<TrashBin>> GetAllTrashBins()
        {
            var trashBins = _trashRepository.GetAll();
            if (trashBins == null || trashBins.Count() == 0)
            {
                return Ok(new List<TrashBin>());
            }
            foreach (var bin in trashBins)
            {
                Add48HourNotificationIfNeeded(bin);
            }
            return Ok(trashBins);
        }

        //accessible for both admin and user
        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public ActionResult<TrashBin> GetTrashBinById(int id)
        {
            TrashBin? trashBin = _trashRepository.GetById(id);
            if (trashBin == null)
            {
                return NotFound();
            }
            return Ok(trashBin);
        }

        //accessible for admin
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<TrashBin> UpdateTrashBin(int id, [FromBody] TrashBin trashBin)
        {
            TrashBin? updated = _trashRepository.Update(id, trashBin);

            if (updated == null)
                return NotFound();

            // 🚨 OVER 80% = NOTIFIKATION
            if (updated.FillLevel >= 80)
            {
                _notificationRepo.Add(
                    updated.FillLevel,
                    updated.Id
                );
            }

            Add48HourNotificationIfNeeded(updated);

            return Ok(updated);
        }

        //accessible for admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<TrashBin> DeleteTrashBin(int id)
        {
            TrashBin? deletedTrashBin = _trashRepository.Delete(id);
            if (deletedTrashBin == null)
            {
                return NotFound();
            }
            return Ok(deletedTrashBin);
        }
        [HttpPut("{id}/empty")]
        public ActionResult<TrashBin> Empty(int id)
        {
            TrashBin? emptied = _trashRepository.EmptyTrash(id);

            if (emptied == null)
                return NotFound();

            // 🧹 TØMNING NOTIFIKATION
            _notificationRepo.Add(
                0,
                emptied.Id
            );

            return Ok(emptied);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/active-sensor")]
        public ActionResult<TrashBin> SetActiveSensorBin(int id)
        {
            TrashBin? updatedBin =
                _trashRepository.SetActiveSensorBin(id);

            if (updatedBin == null)
            {
                return NotFound();
            }

            return Ok(updatedBin);
        }

        [AllowAnonymous]
        [HttpGet("active-sensor")]
        public ActionResult<TrashBin> GetActiveSensorBin()
        {
            TrashBin? activeBin =
                _trashRepository.GetActiveSensorBin();

            if (activeBin == null)
            {
                return NotFound("No active sensor bin selected.");
            }

            return Ok(activeBin);
        }

        [AllowAnonymous]
        [HttpPost("active-sensor/live-data")]
        public ActionResult<TrashBin> UpdateActiveSensorLiveData(
            [FromBody] SensorLiveDataDto sensorData)
        {
            if (sensorData == null)
            {
                return BadRequest("Sensor data is required.");
            }

            if (sensorData.FillLevel < 0 || sensorData.FillLevel > 100)
            {
                return BadRequest("FillLevel must be between 0 and 100.");
            }

            TrashBin? updated =
                _trashRepository.UpdateActiveSensorFillLevel(sensorData.FillLevel);

            if (updated == null)
            {
                return NotFound("No active sensor bin selected.");
            }

            if (updated.FillLevel >= 80)
            {
                _notificationRepo.Add(
                    updated.FillLevel,
                    updated.Id
                );
            }

            Add48HourNotificationIfNeeded(updated);

            return Ok(updated);
        }

        private void Add48HourNotificationIfNeeded(TrashBin bin)
        {
            double hoursSinceLastEmptied =
                (DateTime.UtcNow - bin.LastEmptied)
                .TotalHours;

            if (
                hoursSinceLastEmptied < 48 ||
                !Is48HourFillLevel(bin.FillLevel)
            )
            {
                return;
            }

            string message =
                $"{bin.Name} har ikke været tømt i over 48 timer og er {bin.FillLevel}% fuld!";

            if (_notificationRepo.Exists(message))
            {
                return;
            }

            _notificationRepo.Add(
                bin.FillLevel,
                bin.Id,
                message
            );
        }

        private static bool Is48HourFillLevel(int fillLevel)
        {
            return fillLevel == 80;
        }
    }

    public class SensorLiveDataDto
    {
        public int FillLevel { get; set; }
    }
}
