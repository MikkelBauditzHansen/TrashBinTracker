using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public class NotificationRepo : INotificationrepo
    {
        List<Notification> notifications;
        public NotificationRepo()
        {
            notifications = new List<Notification>();
        }
        public Notification Add(int notficationID)
        {
            throw new NotImplementedException();
        }

        public void Delete(int notficationID)
        {
            throw new NotImplementedException();
        }

        public Notification Get(int notficationID)
        {
            throw new NotImplementedException();
        }

        public List<Notification> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(int notficationID)
        {
            throw new NotImplementedException();
        }
    }
}

