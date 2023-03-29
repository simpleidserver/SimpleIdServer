using Website.Models;

namespace Website.Stores
{
    public interface IUserStore
    {
        User Get(string id);
    }

    public class UserStore : IUserStore
    {
        private readonly ICollection<User> _users;

        public UserStore(ICollection<User> users)
        {
            _users = users;
        }

        public User Get(string id) => _users.First(u => u.Id == id);
    }
}
