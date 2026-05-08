using Microsoft.EntityFrameworkCore;
using TrashBinTracker.Data;
using TrashBinTracker.Model;

namespace TrashBinTracker.Repo
{
    public class EmptyHistoryRepoDB : IEmptyHistoryRepo
    {
        private readonly TrashDbContext _context;

        public EmptyHistoryRepoDB(TrashDbContext context)
        {
            _context = context;
        }

        public IEnumerable<EmptyHistory> GetAll()
        {
            return _context.EmptyHistory
                .OrderByDescending(h => h.EmptiedAt)
                .ToList();
        }

        public IEnumerable<EmptyHistory> GetByTrashBinId(int trashBinId)
        {
            return _context.EmptyHistory
                .Where(h => h.TrashBinId == trashBinId)
                .OrderByDescending(h => h.EmptiedAt)
                .ToList();
        }

        public EmptyHistory? Add(EmptyHistory history)
        {
            if (history == null)
            {
                return null;
            }

            _context.EmptyHistory.Add(history);
            _context.SaveChanges();

            return history;
        }

        public EmptyHistory? Delete(int id)
        {
            var history = _context.EmptyHistory.FirstOrDefault(h => h.Id == id);

            if (history == null)
            {
                return null;
            }

            _context.EmptyHistory.Remove(history);
            _context.SaveChanges();

            return history;
        }
    }
}