using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public class LocationRepositoryList : ILocationRepository
    {
        private readonly List<Location> _locations = new();
        private int _nextId = 1;
        public Location Add(Location location)
        {
            location.Id = _nextId++;
            _locations.Add(location);
            return location;
        }
        public IEnumerable<Location> GetAll()
        {
            return _locations;
        }
        public Location? GetById(int id)
        {
            return _locations.FirstOrDefault(l => l.Id == id);
        }
        public Location? Update(int id, Location updated)
        {
            var existing = GetById(id);
            if (existing == null) return null;

            existing.Name = updated.Name;
            existing.IsIndoor = updated.IsIndoor;

            return existing;
        }
        public Location? Delete(int id)
        {
            var location = GetById(id);
            if (location == null)
            {
                return null;
            }

            _locations.Remove(location);
            return location;
        }
    }
}
