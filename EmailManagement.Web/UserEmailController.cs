using EmailManagement.Common;
using EmailManagement.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;

namespace EmailManagement.Web
{
    public class UserEmailController : ControllerBase
    {
        private readonly ElasticsearchService _elasticsearchService;

        public UserEmailController(ElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }
        public async Task<List<EmailMessage>> GetUpdatedData(string emailId)
        {
            try
            {
                return await _elasticsearchService.GetEmailMessages(Constants.MAILMESSAGES_INDEX_NAME, emailId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
