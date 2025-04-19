using System.Text;
using DNET.Backend.Api.Services.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace DNET.Backend.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        var sendGridApiKey = _configuration["SendGrid:ApiKey"];
        var fromEmail = _configuration["SendGrid:FromEmail"];
        
        _logger.LogInformation("Preparing to send email to {Recipient}", toEmail);

        try
        {
            using var httpClient = new HttpClient();

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
                from = new { email = fromEmail },
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

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email successfully sent to {Recipient}", toEmail);
                return true;
            }
           
            var errorMessage = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to send email to {Recipient}. Status: {StatusCode}. Message: {Error}", toEmail, response.StatusCode, errorMessage); 
            
            return false;
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending email to {Recipient}", toEmail);
            return false;
        }
    }
}
