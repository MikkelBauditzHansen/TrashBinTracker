using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public interface ITrashRepository
    {
        TrashBin Add(TrashBin trashBin);
    }
}