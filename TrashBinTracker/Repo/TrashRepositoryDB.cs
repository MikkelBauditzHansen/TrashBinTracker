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

        private readonly ILogger<TrashRepositoryDB> _logger;

        public TrashRepositoryDB(
            TrashDbContext context,
            TelegramService telegramService,
            ILogger<TrashRepositoryDB> logger)
        {
            _context = context;
            _telegramService = telegramService;
            _logger = logger;
        }

        public TrashBin Add(TrashBin trashBin)
        {
            if (trashBin.LastEmptied == default)
            {
                trashBin.LastEmptied = DateTime.UtcNow;
            }

            trashBin.IsActiveSensorBin = false;

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
                return null;

            // Bevar eksisterende værdier hvis det nye er tomt
            existing.Name = string.IsNullOrWhiteSpace(updatedTrashBin.Name)
                ? existing.Name
                : updatedTrashBin.Name;

            existing.LocationId = updatedTrashBin.LocationId == 0
                ? existing.LocationId
                : updatedTrashBin.LocationId;

            existing.WasteType = updatedTrashBin.WasteType;

            existing.FillLevel = updatedTrashBin.FillLevel;

            _context.SaveChanges();

            bool useTemperatureWarningRule =
                ShouldUseTemperatureWarningRule(existing);

            if (!useTemperatureWarningRule && existing.FillLevel >= 95)
            {
                SendTelegramNotification(
                    () => _telegramService.SendFullWarning(
                        existing.Name ?? "Ukendt skraldespand",
                        existing.FillLevel));
            }
            else if (!useTemperatureWarningRule && existing.FillLevel >= 80)
            {
                SendTelegramNotification(
                    () => _telegramService.SendFillWarning(
                        existing.Name ?? "Ukendt skraldespand",
                        existing.FillLevel));
            }

            if (
                existing.LastEmptied <
                DateTime.UtcNow.AddHours(-48) &&
                Is48HourFillLevel(existing.FillLevel)
            )
            {
                SendTelegramNotification(
                    () => _telegramService.SendTimeWarning(
                        existing.Name ?? "Ukendt skraldespand"));
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

            int previousFillLevel =
                bin.FillLevel;

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

            if (previousFillLevel >= 10)
            {
                SendTelegramNotification(
                    () => _telegramService.SendMessage(
                        $"{bin.Name ?? "Ukendt skraldespand"} er blevet tømt. Den var: {previousFillLevel}% fyldt."));
            }

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

            IEnumerable<TrashBin> allBins =
                _context.TrashBins.ToList();

            foreach (TrashBin bin in allBins)
            {
                bin.IsActiveSensorBin = false;
            }

            selectedBin.IsActiveSensorBin = true;

            _context.SaveChanges();

            return selectedBin;
        }

        public TrashBin? GetActiveSensorBin()
        {
            return _context.TrashBins
                .Include(t => t.EmptyHistory)
                .FirstOrDefault(t => t.IsActiveSensorBin);
        }

        public TrashBin? UpdateActiveSensorFillLevel(int fillLevel)
        {
            TrashBin? activeBin =
                GetActiveSensorBin();

            if (activeBin == null)
            {
                return null;
            }

            TrashBin updatedBin =
                new TrashBin
                {
                    Name = activeBin.Name,
                    LocationId = activeBin.LocationId,
                    WasteType = activeBin.WasteType,
                    FillLevel = fillLevel,
                    LastEmptied = activeBin.LastEmptied
                };

            return Update(activeBin.Id, updatedBin);
        }

        private bool ShouldUseTemperatureWarningRule(TrashBin bin)
        {
            if (
                bin.WasteType != WasteType.Organic ||
                bin.FillLevel < 50
            )
            {
                return false;
            }

            Location? location =
                _context.Locations.Find(bin.LocationId);

            return location != null && !location.IsIndoor;
        }

        private static bool Is48HourFillLevel(int fillLevel)
        {
            return fillLevel == 80;
        }

        private void SendTelegramNotification(Func<Task> sendMessage)
        {
            try
            {
                sendMessage()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not send Telegram notification.");
            }
        }
    }
}
