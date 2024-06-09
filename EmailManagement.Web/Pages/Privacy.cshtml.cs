using EmailClientCore;
using EmailManagement.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.TermStore;
using Microsoft.Identity.Client;
using Nest;
using System.Net.Http.Headers;
using System.Security.Claims;
using Constants = EmailManagement.Common.Constants;

namespace EmailManagement.Web.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly IEmailAggregator _emailAggregator;
        internal readonly List<EmailMessage> emailMessages = new List<EmailMessage>();

        public PrivacyModel(ILogger<PrivacyModel> logger, ElasticsearchService elasticsearchService, IEmailAggregator emailAggregator)
        {
            _logger = logger;
            _elasticsearchService = elasticsearchService;
            _emailAggregator = emailAggregator;
        }

        public async Task OnGet(string username)
        {
            var result = await HttpContext.AuthenticateAsync(Constants.SCHEME);

            if (!result.Succeeded)
                return;

            var email = result.Principal.FindFirst(ClaimTypes.Email)!.Value;
            var outlookId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var accessToken = result.Properties.GetTokenValue("access_token");

            ViewData["emailId"] = email;

            var mailboxDetail = new MailboxDetail
            {
                Id = outlookId,
                EmailAddress = email,
                UserId = username
            };

            // Keep users email details in elastic search
            _elasticsearchService.IndexMailboxDetail(Constants.MAILBOX_INDEX_NAME, mailboxDetail);
        }
    }

}
