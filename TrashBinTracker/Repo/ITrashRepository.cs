using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public interface ITrashRepository
    {
        TrashBin Add(TrashBin trashBin);
        IEnumerable<TrashBin> GetAll();
        TrashBin? Update(int id, TrashBin updatedTrashBin);
        TrashBin? Delete(int id);
        TrashBin? GetById(int id);
        TrashBin? EmptyTrash(int id);
    }
}