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

            if(trashBin.LastEmptied == default)
            {
                trashBin.LastEmptied = DateTime.Now;
            }
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
            bool wasEmtied = existing.FillLevel > 0 && updatedTrashBin.FillLevel == 100;
            existing.Name = updatedTrashBin.Name;
            existing.LocationId = updatedTrashBin.LocationId;
            existing.WasteType = updatedTrashBin.WasteType;
            existing.FillLevel = updatedTrashBin.FillLevel;

            if(wasEmtied)
            {
                existing.LastEmptied = DateTime.Now;
            }
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
        public TrashBin? EmptyTrash(int id)
        {
                TrashBin? trashBin = GetById(id);
                if (trashBin == null)
                {
                    return null;
                }
                trashBin.FillLevel = 0;
                trashBin.LastEmptied = DateTime.Now;
                EmptyHistory history = new EmptyHistory
                {
                    Id = trashBin.EmptyHistory.Count + 1,
                    TrashBinId = trashBin.Id,
                    EmptiedAt = trashBin.LastEmptied
                };
                trashBin.EmptyHistory.Add(history);
                return trashBin;
        }
    }
}

