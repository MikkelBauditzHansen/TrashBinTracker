using Microsoft.EntityFrameworkCore;
using TrashBinTracker.Data;
using TrashBinTracker.Model;
using TrashBinTracker.Service;

namespace TrashBinTracker.Repo
{
    public class TrashRepositoryDB : ITrashRepository
    {
        private readonly TrashDbContext _context;

        private readonly TelegramService _telegramService;

        public TrashRepositoryDB(
            TrashDbContext context,
            TelegramService telegramService)
        {
            _context = context;

            _telegramService = telegramService;
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
            TrashBin? existing = GetById(id);

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

            _context.SaveChanges();

            if (existing.FillLevel >= 95)
            {
                _telegramService.SendFullWarning(
                    existing.Name,
                    existing.FillLevel
                ).Wait();
            }
            else if (existing.FillLevel >= 80)
            {
                _telegramService.SendFillWarning(
                    existing.Name,
                    existing.FillLevel
                ).Wait();
            }

            if (
                existing.WasteType == WasteType.Organic &&
                existing.FillLevel >= 50
            
            )
            {
                _telegramService.SendTemperatureWarning(
                    existing.Name,
                    existing.FillLevel,
                    22
                ).Wait();
            }

            if (
                existing.LastEmptied <
                DateTime.UtcNow.AddHours(-48)
            )
            {
                _telegramService.SendTimeWarning(
                    existing.Name
                ).Wait();
            }

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

            _context.TrashBins.Remove(trashBin);

            _context.SaveChanges();

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

            EmptyHistory history =
                new EmptyHistory
                {
                    TrashBinId = bin.Id,

                    EmptiedAt =
                        DateTime.UtcNow
                };

            _context.EmptyHistory.Add(history);

            _context.SaveChanges();

            return bin;
        }
    }
}