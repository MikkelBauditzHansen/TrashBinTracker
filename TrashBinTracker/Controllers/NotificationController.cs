using Microsoft.AspNetCore.Mvc;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;

namespace TrashBinTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ITrashRepository _trashRepo;
        private readonly INotificationRepo _repo;

        public NotificationController(ITrashRepository trashRepo, INotificationRepo repo)
        {
            _trashRepo = trashRepo;
            _repo = repo;
        }

        // GET: api/notification
        [HttpGet]
        public ActionResult<IEnumerable<Notification>> Get()
        {
            var list = _repo.GetAll();

            if (list == null || !list.Any())
                return Ok(new List<Notification>());

            return Ok(list);
        }

        // GET: api/notification/5
        [HttpGet("{id}")]
        public ActionResult<Notification> Get(int id)
        {
            var notification = _repo.Get(id);

            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        // POST: api/notification
        [HttpPost]
        public ActionResult<Notification> Post([FromBody] Notification value)
        {
            if (value == null)
                return BadRequest("Notification cannot be null");

            var trashBin = _trashRepo.GetById(value.TrashCanID);

            if (trashBin == null)
                return BadRequest("Trashbin does not exist");

            var created = _repo.Add(value.TrashLevel, value.TrashCanID);

            return CreatedAtAction(nameof(Get), new { id = created.NotificationId }, created);
        }

        // PUT: api/notification/5
        [HttpPut("{id}")]
        public ActionResult<Notification> Put(int id, [FromBody] Notification value)
        {
            var updated = _repo.Update(value.TrashLevel, value.TrashCanID, id);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: api/notification/5
        [HttpDelete("{id}")]
        public ActionResult<Notification> Delete(int id)
        {
            var deleted = _repo.Delete(id);

            if (deleted == null)
                return NotFound();

            return Ok(deleted);
        }
    }
}