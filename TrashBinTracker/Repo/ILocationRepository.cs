using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public interface ILocationRepository
    {
        Location Add(Location location);
        Location? Delete(int id);
        IEnumerable<Location> GetAll();
        Location? GetById(int id);
        Location? Update(int id, Location updated);
    }
}