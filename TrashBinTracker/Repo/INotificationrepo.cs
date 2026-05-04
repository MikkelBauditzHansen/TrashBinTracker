using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public interface INotificationRepo
    {
        Notification Add(int trashLevel, int trashCanID);
        Notification Delete(int id);
        Notification Get(int id);
        List<Notification> GetAll();
        Notification Update(int trashLevel, int trashBinId, int id);
    }
}