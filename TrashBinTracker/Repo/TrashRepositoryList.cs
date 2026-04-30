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
    }
}
