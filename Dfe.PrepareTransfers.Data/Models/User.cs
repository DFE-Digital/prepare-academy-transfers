namespace Dfe.PrepareTransfers.Web.Models
{
    public class User
    {
        public User(string id, string emailAddress, string fullName)
        {
            Id = id;
            EmailAddress = emailAddress;
            FullName = fullName;
        }

        public string Id { get; set; }
        public string EmailAddress { get; private set; }
        public string FullName { get; private set; }
    }
}
