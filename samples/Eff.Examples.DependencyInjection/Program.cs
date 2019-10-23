#pragma warning disable 1998
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eff.Core;

namespace Eff.Examples.DependencyInjection
{
    class Program
    {

        class ConsoleLogger : ILogger
        {
            public void Log(string message) => Console.WriteLine(message);
        }

        class MockUserService : IUserService
        {
            private HashSet<string> _users = new HashSet<string>();

            public async Task<bool> CreateUser(string username, string password) => _users.Add(username);
            public async Task<bool> Exists(string username) => _users.Contains(username);
        }

        static async Task Main()
        {
            var container = new Container();
            container.Add<ILogger>(new ConsoleLogger());
            container.Add<IUserService>(new MockUserService());

            var handler = new CustomEffectHandler(container);
            await DomainLogic.CreateNewUsers(new[] { ("user1", "sekrid"), ("user1", "sekrider"), ("user1", "sekridest") }).Run(handler);
        }
    }
}
