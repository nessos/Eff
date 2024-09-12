using Nessos.Effects.DependencyInjection;
using Nessos.Effects.Examples.DependencyInjection;

Container container = new();
container.Add<ILogger>(new ConsoleLogger());
container.Add<IUserService>(new MockUserService());

var handler = new DependencyEffectHandler(container);
await DomainLogic.CreateNewUsers(
    [
        ("user1", "sekrid"),
        ("user1", "sekrider"),
        ("user1", "sekridest")
    ]).Run(handler);

class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine(message);
}

class MockUserService : IUserService
{
    private readonly HashSet<string> _users = new HashSet<string>();
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<bool> CreateUser(string username, string password) => _users.Add(username);
    public async Task<bool> Exists(string username) => _users.Contains(username);
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}