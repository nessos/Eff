namespace Nessos.Eff.Examples.AspNetCore.Domain
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    // real-world enterprise application™
    // based on https://twitter.com/shit_sharp/status/1186637048335290368

    public interface IUserService
    {
        Task<bool> Exists(string username);

        Task Create(string username, string password);

        Task<bool> Authenticate(string username, string password);

        Task Delete(string username);
    }

    /// <summary>
    ///   Simple, in-memory implementation of the above service.
    /// </summary>
    public class InMemoryUserService : IUserService
    {
        private ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();

        public async Task<bool> Exists(string username) => _users.ContainsKey(username);
        public async Task Create(string username, string password)
        {
            if (!_users.TryAdd(username, password))
            {
                throw new System.Exception($"User {username} already exists.");
            }
        }

        public async Task Delete(string username) => _users.TryRemove(username, out var _);
        public async Task<bool> Authenticate(string username, string password) => _users.TryGetValue(username, out var pass) && password == pass;
    }
}
