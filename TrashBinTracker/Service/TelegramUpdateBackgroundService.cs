using System.Text.Json;
using TrashBinTracker.Model;
using TrashBinTracker.Repo;

namespace TrashBinTracker.Service
{
    public class TelegramUpdateBackgroundService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TelegramService _telegramService;

        private readonly string _botToken =
            "8627431919:AAEVa4Fq78A0UpgzCtNOSvT4XGMw6ht_Y7U";

        private readonly string _allowedChatId =
            "8739095942";

        private int _lastUpdateId = 0;

        public TelegramUpdateBackgroundService(
            HttpClient httpClient,
            IServiceScopeFactory scopeFactory,
            TelegramService telegramService)
        {
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
            _telegramService = telegramService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckTelegramCommands();

                await Task.Delay(3000, stoppingToken);
            }
        }

        private async Task CheckTelegramCommands()
        {
            string url =
                $"https://api.telegram.org/bot{_botToken}/getUpdates?offset={_lastUpdateId + 1}";

            string json =
                await _httpClient.GetStringAsync(url);

            JsonDocument document =
                JsonDocument.Parse(json);

            JsonElement result =
                document.RootElement.GetProperty("result");

            foreach (JsonElement update in result.EnumerateArray())
            {
                int updateId =
                    update.GetProperty("update_id").GetInt32();

                _lastUpdateId = updateId;

                if (!update.TryGetProperty("message", out JsonElement message))
                {
                    continue;
                }

                string text =
                    message.GetProperty("text").GetString();

                JsonElement chat =
                    message.GetProperty("chat");

                string chatId =
                    chat.GetProperty("id").GetInt64().ToString();

                if (chatId != _allowedChatId)
                {
                    continue;
                }

                if (text == "/status")
                {
                    await SendStatus();
                }

                if (text == "/tømning" || text == "/tomning")
                {
                    await SendLastEmpty();
                }
            }
        }

        private async Task SendStatus()
        {
            using IServiceScope scope =
                _scopeFactory.CreateScope();

            ITrashRepository trashRepository =
                scope.ServiceProvider.GetRequiredService<ITrashRepository>();

            IEnumerable<TrashBin> bins =
                trashRepository.GetAll();

            await _telegramService.SendStatus(bins);
        }

        private async Task SendLastEmpty()
        {
            using IServiceScope scope =
                _scopeFactory.CreateScope();

            ITrashRepository trashRepository =
                scope.ServiceProvider.GetRequiredService<ITrashRepository>();

            IEnumerable<TrashBin> bins =
                trashRepository.GetAll();

            await _telegramService.SendLastEmptyStatus(bins);
        }
    }
}