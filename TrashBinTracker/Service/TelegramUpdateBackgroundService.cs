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
        private readonly ILogger<TelegramUpdateBackgroundService> _logger;

        private readonly string _botToken =
            "8627431919:AAEVa4Fq78A0UpgzCtNOSvT4XGMw6ht_Y7U";

        private readonly string _allowedChatId =
            "8739095942";

        private int _lastUpdateId = 0;

        public TelegramUpdateBackgroundService(
            HttpClient httpClient,
            IServiceScopeFactory scopeFactory,
            TelegramService telegramService,
            ILogger<TelegramUpdateBackgroundService> logger)
        {
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
            _telegramService = telegramService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckTelegramCommands(stoppingToken);

                try
                {
                    await Task.Delay(3000, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task CheckTelegramCommands(CancellationToken stoppingToken)
        {
            try
            {
                string url =
                    $"https://api.telegram.org/bot{_botToken}/getUpdates?offset={_lastUpdateId + 1}";

                using HttpResponseMessage response =
                    await _httpClient.GetAsync(url, stoppingToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Telegram getUpdates failed with status {StatusCode}",
                        response.StatusCode);

                    return;
                }

                string json =
                    await response.Content.ReadAsStringAsync(stoppingToken);

                using JsonDocument document =
                    JsonDocument.Parse(json);

                if (
                    !document.RootElement.TryGetProperty("ok", out JsonElement okElement) ||
                    okElement.ValueKind != JsonValueKind.True ||
                    !okElement.GetBoolean() ||
                    !document.RootElement.TryGetProperty("result", out JsonElement result) ||
                    result.ValueKind != JsonValueKind.Array
                )
                {
                    _logger.LogWarning("Telegram getUpdates returned an unexpected response.");

                    return;
                }

                foreach (JsonElement update in result.EnumerateArray())
                {
                    if (
                        !update.TryGetProperty("update_id", out JsonElement updateIdElement) ||
                        !updateIdElement.TryGetInt32(out int updateId)
                    )
                    {
                        continue;
                    }

                    _lastUpdateId = updateId;

                    if (!update.TryGetProperty("message", out JsonElement message))
                    {
                        continue;
                    }

                    if (!message.TryGetProperty("text", out JsonElement textElement))
                    {
                        continue;
                    }

                    string? text = textElement.GetString();

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    if (!message.TryGetProperty("chat", out JsonElement chat))
                    {
                        continue;
                    }

                    if (!chat.TryGetProperty("id", out JsonElement chatIdElement))
                    {
                        continue;
                    }

                    string chatId =
                        chatIdElement.GetInt64().ToString();

                    if (chatId != _allowedChatId)
                    {
                        continue;
                    }

                    if (text == "/status")
                    {
                        await SendStatus();
                    }
                    else if (text == "/tømning" || text == "/tomning")
                    {
                        await SendLastEmpty();
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Telegram command polling failed.");
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
