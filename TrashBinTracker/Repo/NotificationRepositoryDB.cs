using TrashBinTracker.Data;
using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public class NotificationRepositoryDB : INotificationRepo
    {
        private readonly TrashDbContext _context;
        private readonly ITrashRepository _trashRepository;

        public NotificationRepositoryDB(
            TrashDbContext context,
            ITrashRepository trashRepository)
        {
            _context = context;
            _trashRepository = trashRepository;
        }

        public Notification Add(int trashLevel, int trashCanID, string? customMessage = null)
        {
            var bin = _trashRepository.GetById(trashCanID);

            string message =
            customMessage ??
          (
              trashLevel >= 80
                ? $"{bin?.Name} er {trashLevel}% fuld!"
              : $"{bin?.Name} er blevet tømt"
          );

            Notification notification = new Notification
            {
                TrashLevel = trashLevel,
                TrashCanID = trashCanID,
                NotificationMessage = message,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);

            _context.SaveChanges();

            return notification;
        }

        public List<Notification> GetAll()
        {
            return _context.Notifications.ToList();
        }

        public Notification? Get(int id)
        {
            return _context.Notifications
                .FirstOrDefault(n => n.NotificationId == id);
        }

        public Notification? Update(int trashLevel, int trashBinId, int id)
        {
            var notif = Get(id);

            if (notif == null)
            {
                return null;
            }

            notif.TrashLevel = trashLevel;
            notif.TrashCanID = trashBinId;

            _context.SaveChanges();

            return notif;
        }

        public Notification? Delete(int id)
        {
            var notif = Get(id);

            if (notif == null)
            {
                return null;
            }

            _context.Notifications.Remove(notif);

            _context.SaveChanges();

            return notif;
        }
        //? TILFØJ EN METODE DER CHECKER OM EN NOTIFICATION MED SAMME MESSAGE ALLEREDE FINDES, FOR AT UNDGÅ DUPLIKATER
        public bool Exists(string message)
        {
            return _context.Notifications
                .Any(n => n.NotificationMessage == message);
        }
    }
}