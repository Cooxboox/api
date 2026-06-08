using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Identity;
using Cooxboox.Core.Kitchens;
using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;

namespace Cooxboox;

public class TestContext : IContext
{
  private readonly Faker _faker;

  public TestContext(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public User? User { get; set; }

  public ActorId? ActorId => User is null ? null : new Actor(User).ToActorId();
  public UserId UserId
  {
    get
    {
      if (User is null)
      {
        throw new InvalidOperationException("An authenticated user is required.");
      }
      return new UserId(User);
    }
  }

  public Kitchen? Kitchen { get; set; }
  public KitchenId KitchenId => Kitchen?.Id ?? throw new InvalidOperationException("A kitchen is required.");
  public bool IsKitchenOwner => Kitchen is not null && Kitchen.OwnerId == UserId;

  public IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes()
  {
    List<CustomAttribute> customAttributes = new(capacity: 2);
    customAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"));
    customAttributes.Add(new CustomAttribute("IpAddress", _faker.Internet.Ip()));
    return customAttributes.AsReadOnly();
  }
}
