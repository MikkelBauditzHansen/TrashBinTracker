using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public interface IEmptyHistoryRepo
    {
        EmptyHistory? Add(EmptyHistory history);
        EmptyHistory? Delete(int id);
        IEnumerable<EmptyHistory> GetAll();
        IEnumerable<EmptyHistory> GetByTrashBinId(int trashBinId);
    }
}