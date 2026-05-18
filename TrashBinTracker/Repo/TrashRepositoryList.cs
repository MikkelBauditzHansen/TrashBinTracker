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
            trashBin.IsActiveSensorBin = false;

            if (trashBin.LastEmptied == default)
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

        public TrashBin? GetById(int id)
        {
            return _trashBins.FirstOrDefault(
                b => b.Id == id
            );
        }

        public TrashBin? Update(
            int id,
            TrashBin updatedTrashBin)
        {
            TrashBin? existing =
                GetById(id);

            if (existing == null)
            {
                return null;
            }

            existing.Name =
                updatedTrashBin.Name;

            existing.LocationId =
                updatedTrashBin.LocationId;

            existing.WasteType =
                updatedTrashBin.WasteType;

            existing.FillLevel =
                updatedTrashBin.FillLevel;

            return existing;
        }

        public TrashBin? Delete(int id)
        {
            TrashBin? trashBin =
                GetById(id);

            if (trashBin == null)
            {
                return null;
            }

            _trashBins.Remove(trashBin);

            return trashBin;
        }

        public TrashBin? EmptyTrash(int id)
        {
            TrashBin? bin =
                GetById(id);

            if (bin == null)
            {
                return null;
            }

            bin.FillLevel = 0;

            bin.LastEmptied =
                DateTime.UtcNow;

            bin.EmptyHistory ??=
                new List<EmptyHistory>();

            bin.EmptyHistory.Add(
                new EmptyHistory
                {
                    Id = bin.EmptyHistory.Count + 1,

                    TrashBinId = bin.Id,

                    EmptiedAt = bin.LastEmptied
                }
            );

            return bin;
        }

        public TrashBin? SetActiveSensorBin(int id)
        {
            TrashBin? selectedBin =
                GetById(id);

            if (selectedBin == null)
            {
                return null;
            }

            foreach (TrashBin bin in _trashBins)
            {
                bin.IsActiveSensorBin = false;
            }

            selectedBin.IsActiveSensorBin = true;

            return selectedBin;
        }

        public TrashBin? GetActiveSensorBin()
        {
            return _trashBins.FirstOrDefault(
                b => b.IsActiveSensorBin
            );
        }

        public TrashBin? UpdateActiveSensorFillLevel(int fillLevel)
        {
            TrashBin? activeBin =
                GetActiveSensorBin();

            if (activeBin == null)
            {
                return null;
            }

            activeBin.FillLevel = fillLevel;

            return activeBin;
        }
    }
}
