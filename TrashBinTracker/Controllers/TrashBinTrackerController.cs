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
        [HttpGet]
        public ActionResult<IEnumerable<TrashBin>> GetAllTrashBins()
        {
            var trashBins = _trashRepository.GetAll();
            if (trashBins == null || trashBins.Count() == 0)
            {
                return Ok(new List<TrashBin>());
            }
            return Ok(trashBins);
        }
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
        [HttpPut("{id}")]
        public ActionResult<TrashBin> UpdateTrashBin(int id, [FromBody] TrashBin trashBin)
        {
            TrashBin? updatedTrashBin = _trashRepository.Update(id, trashBin);

            if (updatedTrashBin == null)
                return NotFound();

            if (updatedTrashBin.FillLevel >= 80)
            {
                var exists = _notificationRepo.GetAll()
                    .Any(n => n.TrashCanID == updatedTrashBin.Id);

                if (!exists)
                {
                    _notificationRepo.Add(
                        updatedTrashBin.FillLevel,
                        updatedTrashBin.Id
                    );
                }
            }

            return Ok(updatedTrashBin);
        }
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
        public ActionResult<TrashBin> EmptyTrashBin(int id)
        {
            TrashBin? emptiedTrashBin = _trashRepository.EmptyTrash(id);
            if (emptiedTrashBin == null)
            {
                return NotFound();
            }
            return Ok(emptiedTrashBin);
        }
    }
}
