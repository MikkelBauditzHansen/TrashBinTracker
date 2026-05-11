using System.Net.Http;

namespace TrashBinTracker.Service
{
    public class TelegramService
    {
        private readonly HttpClient _httpClient;

        private readonly string _botToken =
            "8627431919:AAEVa4Fq78A0UpgzCtNOSvT4XGMw6ht_Y7U";

        private readonly string _chatId =
            "8739095942";

        public TelegramService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendMessage(string message)
        {
            string url =
                $"https://api.telegram.org/bot{_botToken}/sendMessage" +
                $"?chat_id={_chatId}" +
                $"&text={Uri.EscapeDataString(message)}";

            await _httpClient.GetAsync(url);
        }

        public async Task SendFillWarning(string binName, int fillLevel)
        {
            string message =
                $"Advarsel: {binName} er næsten fuld. Fyldningsgrad: {fillLevel}%";

            await SendMessage(message);
        }

        public async Task SendFullWarning(string binName, int fillLevel)
        {
            string message =
                $"ALARM: {binName} er fuld. Fyldningsgrad: {fillLevel}%";

            await SendMessage(message);
        }

        public async Task SendTemperatureWarning(
            string binName,
            int fillLevel,
            double temperature)
        {
            string message =
                $"Temperatur-advarsel: {binName} er {fillLevel}% fuld, og temperaturen er {temperature}°C.";

            await SendMessage(message);
        }

        public async Task SendTimeWarning(string binName)
        {
            string message =
                $"Påmindelse: {binName} er ikke blevet tømt i over 48 timer.";

            await SendMessage(message);
        }

        public async Task SendStatus(IEnumerable<Model.TrashBin> bins)
        {
            string message =
                "Status på skraldespande:\n\n";

            foreach (Model.TrashBin bin in bins)
            {
                message +=
                    $"{bin.Name}: {bin.FillLevel}% fuld\n";
            }

            await SendMessage(message);
        }

        public async Task SendLastEmptyStatus(IEnumerable<Model.TrashBin> bins)
        {
            string message =
                "Sidste tømning:\n\n";

            foreach (Model.TrashBin bin in bins)
            {
                message +=
                    $"{bin.Name}: {bin.LastEmptied}\n";
            }

            await SendMessage(message);
        }
    }
}