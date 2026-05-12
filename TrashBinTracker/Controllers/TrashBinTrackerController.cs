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
            //  NOTIFIKATION FOR TRASH BINS OVER 48 timer
            foreach (var bin in trashBins)
            {
                var hours =
                    (DateTime.UtcNow - bin.LastEmptied)
                    .TotalHours;

                if (hours >= 48)
                {
                    string message =
                        $"{bin.Name} har ikke været tømt i over 48 timer!";

                    bool alreadyExists =
                        _notificationRepo.Exists(message);

                    if (!alreadyExists)
                    {
                        _notificationRepo.Add(
                            bin.FillLevel,
                            bin.Id,
                            message
                        );
                    }
                }
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
    }
}
