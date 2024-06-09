using EmailManagement.Common;
using EmailManagement.Web.Pages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace EmailManagement.Web.Pages
{
    public class AddAccountModel : PageModel
    {
        private readonly HttpClient _httpClient; 
        private readonly ILogger<AddAccountModel> _logger;
        public string Error { get; set; }
        private readonly ElasticsearchService _elasticsearchService;
        private readonly UserDatabase _userDatabase;

        public AddAccountModel(ElasticsearchService elasticsearchService, UserDatabase userDb)
        {
            _httpClient = new HttpClient();
            _elasticsearchService = elasticsearchService;
            _userDatabase = userDb;
        }


        public async Task<IActionResult> OnPost(User user)
        {
            if (user == null)
            {
                return Unauthorized();
            }
            else if (_userDatabase.lstUsers.Where(o => o.Email == user.Email).Any())
            {
            }
            else
            {
                _userDatabase.lstUsers.Add(user); // Add the user in local database
            }

            var properties = new AuthenticationProperties { RedirectUri = "/privacy?username=" + user.Username };
            return Challenge(properties, Constants.SCHEME);
        }

    }

}