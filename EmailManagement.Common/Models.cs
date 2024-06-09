namespace EmailManagement.Common
{

    public class EmailMessage
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string UserId { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string EmailId { get; set; }
    }

    public class MailboxDetail
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class UserDatabase {
        public List<User> lstUsers { get; set; } = new List<User>();
    }
}
