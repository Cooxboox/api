using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Identity;
using Cooxboox.Core.Kitchens;
using Cooxboox.Extensions;
using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;

namespace Cooxboox;

internal class HttpApplicationContext : IContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is required.");

  public ActorId? ActorId
  {
    get
    {
      User? user = Context.GetUser();
      if (user is not null)
      {
        Actor actor = new(user);
        return actor.ToActorId();
      }

      ApiKey? apiKey = Context.GetApiKey();
      if (apiKey is not null)
      {
        Actor actor = new(apiKey);
        return actor.ToActorId();
      }

      return null;
    }
  }
  public UserId UserId
  {
    get
    {
      User user = Context.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
      return new UserId(user);
    }
  }

  public KitchenId KitchenId => throw new NotImplementedException(); // TODO(fpion): implement
  public bool IsKitchenOwner => throw new NotImplementedException(); // TODO(fpion): implement

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes() => Context.GetSessionCustomAttributes();
}
