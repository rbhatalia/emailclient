namespace EmailManagement.Web.Services
{
    public interface IMessageSynchronizer
    {
        Task SyncOutlookMessages(DateTimeOffset lastRunDateTimeOffset);
    }

}
