using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public interface INotificationRepo
    {
        Notification Add(int trashLevel, int trashCanID, string? customMessage = null);
        Notification Delete(int id);
        Notification Get(int id);
        List<Notification> GetAll();
        Notification Update(int trashLevel, int trashBinId, int id);
        bool Exists(string message);
        bool ToggleBinFullNotifications();

        bool ToggleTelegram();

        bool GetBinFullStatus();

        bool GetTelegramStatus();
    }
}