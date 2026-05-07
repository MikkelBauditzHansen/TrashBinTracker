using TrashBinTracker.Data;
using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public class LocationRepositoryDB : ILocationRepository
    {
        private readonly TrashDbContext _context;

        public LocationRepositoryDB(TrashDbContext context)
        {
            _context = context;
        }

        public Location Add(Location location)
        {
            _context.Locations.Add(location);
            _context.SaveChanges();

            return location;
        }

        public IEnumerable<Location> GetAll()
        {
            return _context.Locations.ToList();
        }

        public Location? GetById(int id)
        {
            return _context.Locations.FirstOrDefault(l => l.Id == id);
        }

        public Location? Update(int id, Location updated)
        {
            var existing = GetById(id);

            if (existing == null)
            {
                return null;
            }

            existing.Name = updated.Name;
            existing.IsIndoor = updated.IsIndoor;

            _context.SaveChanges();

            return existing;
        }

        public Location? Delete(int id)
        {
            var location = GetById(id);

            if (location == null)
            {
                return null;
            }

            _context.Locations.Remove(location);

            _context.SaveChanges();

            return location;
        }
    }
}