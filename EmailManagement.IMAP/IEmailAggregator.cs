using MimeKit;

namespace EmailClientCore
{
    public interface IEmailAggregator
    {
        List<MimeMessage> FetchEmails(string emailId, DateTimeOffset lastRunDateTimeOffset);
    }
}