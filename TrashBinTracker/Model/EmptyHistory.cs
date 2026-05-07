namespace TrashBinTracker.Model
{
    public class EmptyHistory
    {
        public int Id { get; set; }
        public int TrashBinId { get; set; }
        public DateTime EmptiedAt { get; set; }
        public EmptyHistory()
        {
            EmptiedAt = DateTime.UtcNow;
        }
    }
}
