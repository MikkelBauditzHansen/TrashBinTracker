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
        public TrashBinTrackerController(ITrashRepository trashRepository)
        {
            _trashRepository = trashRepository;
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
            TrashBin? updatedTrashBin = _trashRepository.Update(id, trashBin);
            if (updatedTrashBin == null)
            {
                return NotFound();
            }
            return Ok(updatedTrashBin);
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
    }
}
