using EmailManagement.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmailManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ElasticsearchService _elasticsearchService;

        public AccountController(ElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        [HttpPost("create")]
        public IActionResult CreateAccount([FromBody] User user)
        {
            // _elasticsearchService.CreateIndices(); // Ensure indices are created
                                                   // Save user in Elasticsearch or another storage
                                                   // For simplicity, let's assume user details are saved successfully and we have a local user ID

            // Redirect to Microsoft OAuth login
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("OAuthCallback") };
            return Challenge(properties, "Microsoft");
        }

        [HttpGet("oauth-callback")]
        public async Task<IActionResult> OAuthCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Microsoft");

            if (!result.Succeeded)
                return Unauthorized();

            var email = result.Principal.FindFirst(ClaimTypes.Email).Value;
            var outlookId = result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            var accessToken = result.Properties.GetTokenValue("access_token");

            // Retrieve user details from your storage using email or outlookId and update
            // For simplicity, let's assume we have a user ID from the storage
            var userId = "local-user-id"; // Replace with actual user ID

            var mailboxDetail = new MailboxDetail
            {
                Id = outlookId,
                EmailAddress = email,
                UserId = userId
            };

            _elasticsearchService.IndexMailboxDetail("mailbox_details_index", mailboxDetail);

            return Redirect("YourCallbackUrlHere");
        }
    }

}

