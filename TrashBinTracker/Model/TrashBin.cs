namespace TrashBinTracker.Model
{
    public enum WasteType
    {
        General,
        Paper,
        Organic,
        Metal
    }
    public class TrashBin
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int LocationId { get; set; }
        public string? Location { get; set; }
        public WasteType WasteType { get; set; }
        public int FillLevel { get; set; }

        public TrashBin()
        {
            Name = "";
            WasteType = WasteType.General;
            FillLevel = 0;
        }
    }

}
