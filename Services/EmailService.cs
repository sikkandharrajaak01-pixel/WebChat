using System.Text;
using System.Text.Json;

namespace Chat_App.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            var apiKey = _configuration["Brevo:ApiKey"];
            var fromEmail = _configuration["Brevo:FromEmail"];

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Brevo API Key is missing.");

            if (string.IsNullOrEmpty(fromEmail))
                throw new Exception("Sender email is missing.");

            var payload = new
            {
                sender = new
                {
                    name = "Chat App",
                    email = fromEmail
                },

                to = new[]
                {
                    new
                    {
                        email = toEmail
                    }
                },

                subject = "OTP Verification",

                htmlContent = $@"
                <html>
                <body style='font-family:Arial'>
                    <h2>Password Reset OTP</h2>

                    <p>Your OTP code is:</p>

                    <div style='
                        font-size:32px;
                        font-weight:bold;
                        letter-spacing:5px;
                        background:#f4f4f4;
                        padding:15px;
                        border-radius:8px;
                        text-align:center;'>
                        {otp}
                    </div>

                    <p>
                        This OTP expires in 5 minutes.
                    </p>
                </body>
                </html>"
            };

            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.brevo.com/v3/smtp/email");

            request.Headers.Add("api-key", apiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception(
                    $"Brevo Error {(int)response.StatusCode}: {error}");
            }
        }
    }
}