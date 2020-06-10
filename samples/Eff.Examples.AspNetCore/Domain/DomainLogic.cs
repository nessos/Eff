namespace Nessos.Effects.Examples.AspNetCore.Domain
{
    using System;
    using Microsoft.Extensions.Logging;

    public class DomainLogic
    {
        // real-world enterprise application™
        // based on https://twitter.com/shit_sharp/status/1186637048335290368

        public static async Eff<string> CheckUsername(string username)
        {
            if (await IO<IUserService>.Do(svc => svc.Exists(username)))
            {
                username = username + "1";

                if (await IO<IUserService>.Do(svc => svc.Exists(username)))
                {
                    // Two checks should be enough
                    username = username.Replace("1", "2");
                }
            }

            return username;
        }

        public static async Eff CreateNewUser(string username, string password)
        {
            var newUsername = await CheckUsername(username);

            try
            {
                await IO<IUserService>.Do(svc => svc.Create(newUsername, password));
                await IO<ILogger<DomainLogic>>.Do(async logger => logger.LogInformation($"Successfully created user '{newUsername}'."));
            }
            catch (Exception e)
            {
                await IO<ILogger<DomainLogic>>.Do(async logger => logger.LogError($"Failed to create user '{newUsername}': {e}."));
                throw;
            }
        }

        public static async Eff DeleteUser(string username)
        {
            await IO<IUserService>.Do(svc => svc.Delete(username));
        }

        public static async Eff<bool> Authenticate(string username, string password)
        {
            return await IO<IUserService>.Do(svc => svc.Authenticate(username, password));
        }
    }
}
