using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;

namespace TrashBinTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmptyHistoryController : Controller
    {
        private readonly IEmptyHistoryRepo _emptyHistoryRepo;

        public EmptyHistoryController(IEmptyHistoryRepo emptyHistoryRepo)
        {
            _emptyHistoryRepo = emptyHistoryRepo;
        }

        // GET ALL HISTORY
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public ActionResult<IEnumerable<EmptyHistory>> GetAll()
        {
            var history = _emptyHistoryRepo.GetAll();

            if (history == null || !history.Any())
            {
                return Ok(new List<EmptyHistory>());
            }

            return Ok(history);
        }

        // GET HISTORY FOR SPECIFIC BIN
        [Authorize(Roles = "Admin, User")]
        [HttpGet("{trashBinId}")]
        public ActionResult<IEnumerable<EmptyHistory>> GetByTrashBinId(int trashBinId)
        {
            var history = _emptyHistoryRepo.GetByTrashBinId(trashBinId);

            if (history == null || !history.Any())
            {
                return Ok(new List<EmptyHistory>());
            }

            return Ok(history);
        }

        // ADD HISTORY MANUALLY
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<EmptyHistory> Add([FromBody] EmptyHistory history)
        {
            if (history == null)
            {
                return BadRequest();
            }

            var added = _emptyHistoryRepo.Add(history);

            if (added == null)
            {
                return StatusCode(500);
            }

            return CreatedAtAction(nameof(Add), new { id = added.Id }, added);
        }

        // DELETE HISTORY
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<EmptyHistory> Delete(int id)
        {
            var deleted = _emptyHistoryRepo.Delete(id);

            if (deleted == null)
            {
                return NotFound();
            }

            return Ok(deleted);
        }
    }
}