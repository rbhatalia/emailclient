using EmailManagement.Common;
using Microsoft.Graph;
using Nest;
using EmailClientCore;
using Constants = EmailManagement.Common.Constants;
using MimeKit;
using System.Net.Mail;

namespace EmailManagement.Web.Services
{
    public class MessageSynchronizer : IMessageSynchronizer
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly IEmailAggregator _emailAggregator;
        private readonly UserDatabase _userDatabase;

        public MessageSynchronizer(ElasticsearchService elasticsearchService, IEmailAggregator emailAggregator, UserDatabase userDatabase)
        {
            _elasticsearchService = elasticsearchService;
            _emailAggregator = emailAggregator;
            _userDatabase = userDatabase;
        }

        public async Task SyncOutlookMessages(DateTimeOffset lastRunDateTimeOffset)
        {
            foreach (var users in _userDatabase.lstUsers)
            {
                // foreach users from the database
                var messages = GetMessagesFromOutlook(users.Email, lastRunDateTimeOffset);

                foreach (var message in messages)
                {
                    // Convert message for indexing document
                    var emailMessage = ConvertToEmailMessage(users.Email, message);

                    _elasticsearchService.IndexEmailMessage(Constants.MAILMESSAGES_INDEX_NAME, emailMessage);
                }
            }
        }

        private List<MimeMessage> GetMessagesFromOutlook(string userEmailAddress, DateTimeOffset lastRunDateTimeOffset)
        {
            return _emailAggregator.FetchEmails(userEmailAddress, lastRunDateTimeOffset);
        }

        private EmailMessage ConvertToEmailMessage(string emailAddress, MimeMessage message)
        {
            return new EmailMessage
            {
                EmailId = emailAddress,
                Id = message.MessageId,
                Subject = message.Subject,
                Body = message.TextBody,
                ReceivedDate = message.Date.DateTime,
                SenderName = message!.Sender?.Name,
                SenderEmail = message!.Sender?.Address,
            };
        }
    }

}
