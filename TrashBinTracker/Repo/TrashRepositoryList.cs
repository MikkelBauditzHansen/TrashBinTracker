using TrashBinTracker.Model;
namespace TrashBinTracker.Repo
{
    public class TrashRepositoryList : ITrashRepository
    {
        private readonly List<TrashBin> _trashBins;
        private int _nextId;
        public TrashRepositoryList()
        {
            _trashBins = new List<TrashBin>();
            _nextId = 1;
        }
        public TrashBin Add(TrashBin trashBin)
        {
            trashBin.Id = _nextId++;
            _trashBins.Add(trashBin);
            return trashBin;
        }
        public IEnumerable<TrashBin> GetAll()
        {
            return _trashBins;
        }
        public TrashBin? Update(int id, TrashBin updatedTrashBin)
        {
            TrashBin? existing = GetById(id);
            if (existing == null)
            {
                return null;
            }
            existing.Name = updatedTrashBin.Name;
            existing.LocationID = updatedTrashBin.LocationID;
            existing.WasteType = updatedTrashBin.WasteType;
            existing.FillLevel = updatedTrashBin.FillLevel;
            return existing;
        }
        public TrashBin? Delete(int id)
        {
            TrashBin? trashBin = GetById(id);
            if (trashBin == null)
            {
                return null;
            }
            _trashBins.Remove(trashBin);
            return trashBin;
        }
        public TrashBin? GetById(int id)
        {
            return _trashBins.FirstOrDefault(b => b.Id == id);
        }
    }
}
