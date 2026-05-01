using System.Diagnostics;
using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public class NotificationRepo : INotificationrepo
    {
        List<Notification> notifications;
        public NotificationRepo()
        {
            notifications = new List<Notification>();
            notifications.Add(new Notification(1, 2, 0));
            notifications.Add(new Notification(1, 2, 1));
            notifications.Add(new Notification(1, 2, 2));
        }
        public Notification Add(int trashLevel, int trashCanID, int notaficationId)
        {
            Notification notification = new Notification( trashLevel,trashCanID, notaficationId);
            notifications.Add(notification);
            return notification;
        }

        public Notification Delete(int notficationID)
        {
            Notification notification = notifications[notficationID];
            notifications.RemoveAt(notficationID);
            return notification;

        }

        public Notification Get(int notficationID)
        {
            return notifications[notficationID];
        }

        public List<Notification> GetAll()
        {
            return notifications;
        }

        public Notification Update(int trashLevel, int trashCanID, int notaficationId)
        {
            Notification notification = new Notification(trashLevel, trashCanID, notaficationId);
            notifications[notaficationId] = notification;
            return notification;

        }
    }
}

