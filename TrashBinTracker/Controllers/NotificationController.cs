using Microsoft.AspNetCore.Mvc;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrashBinTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        NotificationRepo _repo;
        public NotificationController(NotificationRepo repo)
        {
            _repo = repo;
        }

        // GET: api/<NotificationController>
        [HttpGet]
        public ActionResult<IEnumerable<Notification>>Get()
        {
            return Ok(_repo.GetAll());
        }

        // GET api/<NotificationController>/5
        [HttpGet("{id}")]
        public ActionResult<Notification>Get(int id)
        {
            return Ok(_repo.Get(id));
        }

        // POST api/<NotificationController>
        [HttpPost]
        public ActionResult<Notification> Post([FromBody] Notification value)
        {
            return Ok(_repo.Add(value.TrashLevel,value.TrashCanID,value.NotificationId));
        }

        // PUT api/<NotificationController>/5
        [HttpPut("{id}")]
        public ActionResult<Notification> Put(int id, [FromBody] Notification value)
        {
            return Ok(_repo.Update(value.TrashLevel, value.TrashCanID, id));

        }

        // DELETE api/<NotificationController>/5
        [HttpDelete("{id}")]
        public ActionResult<Notification> Delete(int id)
        {

            return Ok(_repo.Delete(id));
        }
    }
}
