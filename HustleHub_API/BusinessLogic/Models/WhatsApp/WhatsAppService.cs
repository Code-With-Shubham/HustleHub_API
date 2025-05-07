using System.Text;

namespace HustleHub_API.BusinessLogic.Models.WhatsApp
{
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly string _phoneNumberId = "YOUR_PHONE_NUMBER_ID";
        private readonly string _accessToken = "YOUR_ACCESS_TOKEN";

        public WhatsAppService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public async Task<string> SendTextMessageAsync(string toPhoneNumber, string message)
        {
            var url = $"https://graph.facebook.com/v18.0/{_phoneNumberId}/messages";

            var body = new
            {
                messaging_product = "whatsapp",
                to = toPhoneNumber, // in international format e.g., "91XXXXXXXXXX"
                type = "text",
                text = new { body = message }
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
    }



}
