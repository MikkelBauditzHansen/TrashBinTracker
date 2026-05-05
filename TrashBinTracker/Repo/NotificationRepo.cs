using System.Diagnostics;
using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public class NotificationRepo : INotificationRepo
    {
        private readonly List<Notification> _notifications = new();
        private readonly ITrashRepository _trashRepository;
        private int _nextId = 1;

        public NotificationRepo(ITrashRepository trashRepository)
        {
            _trashRepository = trashRepository;
        }

        public Notification Add(int trashLevel, int trashCanID)
        {
            Notification notification = new Notification(trashLevel, trashCanID, _nextId++);
            var binName = _trashRepository.GetById(trashCanID)?.Name ?? "Unknown bin";
            notification.NotificationMessage = $"{binName} trash level is {trashLevel}%";
            _notifications.Add(notification);
            return notification;
        }

        public Notification Delete(int id)
        {
            var notif = Get(id);
            if (notif == null)
            {
                return null;
            }

            _notifications.Remove(notif);
            return notif;
        }

        public Notification Get(int id)
        {
            return _notifications.FirstOrDefault(n => n.NotificationId == id);
        }

        public List<Notification> GetAll()
        {
            return _notifications;
        }

        public Notification Update(int trashLevel, int trashBinId, int id)
        {
            var notif = Get(id);
            if (notif == null)
            {
                return null;
            }

            notif.TrashLevel = trashLevel;
            notif.TrashCanID = trashBinId;
            var binName = _trashRepository.GetById(trashBinId)?.Name ?? "Unknown bin";
            notif.NotificationMessage = $"{binName} trash level is {trashLevel}%";
            return notif;
        }
    }
}

