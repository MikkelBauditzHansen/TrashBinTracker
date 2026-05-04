using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public interface INotificationrepo
    {
        List<Notification> GetAll();

        Notification Add(int trashLevel, int trashCanID, int notaficationId);

        Notification Get(int notficationID);

        Notification Delete(int notficationID);

        Notification Update(int trashLevel, int trashCanID, int notaficationId);

    }

}
