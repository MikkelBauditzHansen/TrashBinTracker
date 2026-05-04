using Microsoft.AspNetCore.Mvc;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;

namespace TrashBinTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : Controller
    {
        private readonly ILocationRepository _locationRepo;
        private readonly ITrashRepository _trashRepo;
        public LocationController(ILocationRepository locationRepo, ITrashRepository trashRepo)
        {
            _locationRepo = locationRepo;
            _trashRepo = trashRepo;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Location>> GetAll()
        {
            return Ok(_locationRepo.GetAll());
        }
        [HttpGet("{id}")]
        public ActionResult<Location> GetById(int id)
        {
            var loc = _locationRepo.GetById(id);
            if (loc == null)
            {
                return NotFound();
            }
            return Ok(loc);
        }
        [HttpPost]
        public ActionResult<Location> Add(Location location)
        {
            if (string.IsNullOrWhiteSpace(location.Name))
            {
                return BadRequest("Name is required");
            }
            var created = _locationRepo.Add(location);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPut("{id}")]
        public ActionResult<Location> Update(int id, Location location)
        {
            var updated = _locationRepo.Update(id, location);
            if (updated == null) 
            { 
                return NotFound(); 
            }

            return Ok(updated);
        }
        [HttpDelete("{id}")]
        public ActionResult<Location> Delete(int id)
        {
            bool inUse = _trashRepo.GetAll().Any(b => b.LocationId == id);
            if (inUse)
            {
                return BadRequest("Location is in use");
            }
            var deleted = _locationRepo.Delete(id);
            if (deleted == null)
            {
                return NotFound();
            }
            return Ok(deleted);
        }

    }
}
