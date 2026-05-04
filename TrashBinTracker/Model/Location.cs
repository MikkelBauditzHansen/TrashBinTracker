using System.Diagnostics.Eventing.Reader;

namespace TrashBinTracker.Model
{
    public class Location
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsIndoor { get; set; }
    }
}
