using Microsoft.EntityFrameworkCore;
using TrashBinTracker.Data;
using TrashBinTracker.Model;
namespace TrashBinTracker.Repo
{
    public class TrashRepositoryDB : ITrashRepository
    {
        private readonly TrashDbContext _context;

        public TrashRepositoryDB(TrashDbContext context)
        {
            _context = context;
        }

        public TrashBin Add(TrashBin trashBin)
        {
            if (trashBin.LastEmptied == default)
            {
                trashBin.LastEmptied = DateTime.UtcNow;
            }

            _context.TrashBins.Add(trashBin);
            _context.SaveChanges();

            return trashBin;
        }

        public IEnumerable<TrashBin> GetAll()
        {
            return _context.TrashBins
                .Include(t => t.EmptyHistory)
                .ToList();
        }

        public TrashBin? GetById(int id)
        {
            return _context.TrashBins
                .Include(t => t.EmptyHistory)
                .FirstOrDefault(t => t.Id == id);
        }

        public TrashBin? Update(int id, TrashBin updatedTrashBin)
        {
            var existing = GetById(id);

            if (existing == null)
            {
                return null;
            }

            existing.Name = updatedTrashBin.Name;
            existing.LocationId = updatedTrashBin.LocationId;
            existing.WasteType = updatedTrashBin.WasteType;
            existing.FillLevel = updatedTrashBin.FillLevel;

            _context.SaveChanges();

            return existing;
        }

        public TrashBin? Delete(int id)
        {
            var trashBin = GetById(id);

            if (trashBin == null)
            {
                return null;
            }

            _context.TrashBins.Remove(trashBin);
            _context.SaveChanges();

            return trashBin;
        }

        public TrashBin? EmptyTrash(int id)
        {
            var bin = GetById(id);

            if (bin == null)
            {
                return null;
            }

            bin.FillLevel = 0;
            bin.LastEmptied = DateTime.UtcNow;

            var history = new EmptyHistory
            {
                TrashBinId = bin.Id,
                EmptiedAt = DateTime.UtcNow
            };

            _context.EmptyHistory.Add(history);

            _context.SaveChanges();

            return bin;
        }
    }
}
