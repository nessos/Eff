namespace Nessos.Effects.Examples.AspNetCore.Controllers;

using Microsoft.AspNetCore.Mvc;
using Nessos.Effects.Examples.AspNetCore.Domain;
using Nessos.Effects.Examples.AspNetCore.EffBindings;
using System.Threading.Tasks;

[Route("api/users")]
public class UsersController : EffControllerBase
{
    public UsersController(IEffectHandlerFactory handlerFactory) : base(handlerFactory)
    {

    }

    /// <remarks>
    ///   Implementation of this endpoint is deliberately flawed.
    ///   For instance, attempting to create the same username twice will not fail.
    ///   However doing it four times will result in a 500 error.
    ///   
    ///   Each request will return an <code>eff-replay-token</code> header, 
    ///   which can be passed to future requests to induce a dry run.
    ///   This can be used to step through requests that were handled in the past.
    /// </remarks>
    [HttpPost("create")]
    public Task CreateUser(string username, string password)
    {
        return Execute(DomainLogic.CreateNewUser(username, password));
    }

    /// <remarks>
    ///   Each request will return an <code>eff-replay-token</code> header, 
    ///   which can be passed to future requests to induce a dry run.
    ///   This can be used to step through requests that were handled in the past.
    /// </remarks>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteUser(string username)
    {
        if (await Execute(DomainLogic.DeleteUser(username)))
        {
            return Accepted();
        }
        else
        {
            return NotFound();
        }
    }

    /// <remarks>
    ///   Each request will return an <code>Eff-Replay-Token</code> header, 
    ///   which can be passed to future requests to induce a dry run.
    ///   This can be used to step through requests that were handled in the past.
    /// </remarks>
    [HttpPost("authenticate")]
    public async Task<ActionResult> Authenticate(string username, string password)
    {
        if (await Execute(DomainLogic.Authenticate(username, password)))
        {
            return Accepted();
        }
        else
        {
            return Unauthorized();
        }
    }
}
