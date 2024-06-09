using Azure.Identity;
using EmailClientCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using MimeKit;
using System.Net.Http.Headers;

namespace Email.IMAP
{
    public class OutlookBasedService: IEmailAggregator
    {
        private readonly IConfiguration _configuration;

        public OutlookBasedService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<string> GetOAuthTokenAsync(string clientId, string clientSecret, string tenantId)
        {
            var confidentialClient = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            var authResult = await confidentialClient.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" }).ExecuteAsync();
            return authResult.AccessToken;
        }

        public async Task<List<MimeMessage>> FetchEmailsAsync(string emailId, DateTimeOffset lastRunDateTimeOffset)
        {
            var clientId = _configuration["Authentication:Microsoft:ClientId"];
            var tenantId = _configuration["Authentication:Microsoft:TenantId"];
            var clientSecret = _configuration["Authentication:Microsoft:ClientSecret"];

            var clientApp = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var authResult = await clientApp.AcquireTokenForClient(scopes).ExecuteAsync();
            var token = await GetOAuthTokenAsync(clientId, tenantId, clientSecret);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"https://graph.microsoft.com/v1.0/users/{emailId}/messages");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching user photo: {response.StatusCode}, {errorContent}");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
            }

            return new List<MimeMessage>();
        }

        public List<MimeMessage> FetchEmails(string emailId, DateTimeOffset lastRunDateTimeOffset)
        {
            return FetchEmailsAsync(emailId, lastRunDateTimeOffset).Result;
        }
    }
}
