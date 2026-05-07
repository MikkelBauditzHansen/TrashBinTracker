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
            var bin = _trashRepository.GetById(trashCanID);
            var binName = bin?.Name ?? "Unknown bin";

            string message;

            if (trashLevel >= 80)
            {
                message = $"{binName} er {trashLevel}% fuld!";
            }
            else if (trashLevel == 0)
            {
                message = $"{binName} er blevet tømt";
            }
            else
            {
                message = $"{binName} er {trashLevel}% fuld";
            }

            Notification notification = new Notification(trashLevel, trashCanID, _nextId++)
            {
                NotificationMessage = message
            };

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

