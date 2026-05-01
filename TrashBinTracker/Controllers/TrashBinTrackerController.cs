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
        public TrashBinTrackerController(ITrashRepository trashRepository)
        {
            _trashRepository = trashRepository;
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
    }
}
