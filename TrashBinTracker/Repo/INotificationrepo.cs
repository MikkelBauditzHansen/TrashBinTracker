using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public interface INotificationrepo
    {
        List<Notification> GetAll();

        Notification Add(int notficationID);

        Notification Get(int notficationID);

        void Delete(int notficationID);

        void Update(int notficationID);

    }
}
