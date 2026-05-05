namespace TrashBinTracker.Model
{
    public class FillHistory
    {
        public int Id { get; set; }
        public int TrashBinId { get; set; }
        public int FillLevel { get; set; }
        public DateTime Register { get; set; }
        public FillHistory()
        {
            Register = DateTime.UtcNow;
        }
    }
}
