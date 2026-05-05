namespace TrashBinTracker.Model
{
    public class Notification
    {
        public string NotificationMessage { set; get; }

        public int TrashLevel { set; get; }

        public int TrashCanID { set; get; }

        public int NotificationId { set; get; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Notification(int trashLevel, int trashCanID, int notaficationId)
        {
            TrashLevel = trashLevel;
            TrashCanID = trashCanID;
            NotificationId = notaficationId;
            NotificationMessage = "";
        }
    }
}
