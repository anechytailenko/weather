using System.Text;
using DNET.Backend.Api.Services.Interfaces;
using Newtonsoft.Json;

namespace DNET.Backend.Api.Services;
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        var sendGridApiKey = _configuration["SendGrid:ApiKey"];
        var fromEmail = _configuration["SendGrid:FromEmail"];

        using (var httpClient = new HttpClient())
        {
            var emailData = new
            {
                personalizations = new[]
                {
                    new
                    {
                        to = new[] { new { email = toEmail } },
                        subject = subject
                    }
                },
                from = new
                {
                    email = fromEmail
                },
                content = new[]
                {
                    new
                    {
                        type = "text/plain",
                        value = body
                    }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(emailData);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send")
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sendGridApiKey);

            var response = await httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}