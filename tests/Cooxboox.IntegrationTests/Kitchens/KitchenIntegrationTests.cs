using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Kitchens;

[Trait(Traits.Category, Categories.Integration)]
public class KitchenIntegrationTests : IntegrationTests
{
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IKitchenService _kitchenService;

  private Kitchen _kitchen = null!;

  public KitchenIntegrationTests() : base()
  {
    _kitchenRepository = ServiceProvider.GetRequiredService<IKitchenRepository>();
    _kitchenService = ServiceProvider.GetRequiredService<IKitchenService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _kitchen = new Kitchen(Context.UserId, new Name(Faker.Company.CompanyName()));
    await _kitchenRepository.SaveAsync(_kitchen);
  }

  [Theory(DisplayName = "It should create a new kitchen.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    User user = new UserBuilder().Build();
    Actor actor = new(user);
    Context.User = user;

    CreateOrReplaceKitchenPayload payload = new($"  {Faker.Company.CompanyName()}  ");
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    KitchenModel kitchen = result.Kitchen;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, kitchen.Id);
    }
    Assert.Equal(1, kitchen.Version);
    Assert.Equal(actor, kitchen.CreatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(kitchen.CreatedBy, kitchen.UpdatedBy);
    Assert.Equal(kitchen.CreatedOn, kitchen.UpdatedOn);

    Assert.Equal(kitchen.CreatedBy, kitchen.Owner);
    Assert.Equal(Confidentiality.Private, kitchen.Confidentiality);
    Assert.Equal(payload.Name.Trim(), kitchen.Name);
    Assert.Null(kitchen.Slug);
  }

  [Fact(DisplayName = "It should read a kitchen by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    KitchenModel? kitchen = await _kitchenService.ReadAsync(_kitchen.EntityId);
    Assert.NotNull(kitchen);
    Assert.Equal(_kitchen.EntityId, kitchen.Id);
  }

  [Fact(DisplayName = "It should replace an existing kitchen.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceKitchenPayload payload = new($"  {Faker.Company.CompanyName()}  ");

    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, _kitchen.EntityId);
    Assert.False(result.Created);
    KitchenModel kitchen = result.Kitchen;

    Assert.Equal(_kitchen.EntityId, kitchen.Id);
    Assert.Equal(_kitchen.Version + 1, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_kitchen.OwnerId.Value, kitchen.Owner.ToActorId().Value);
    Assert.Equal(_kitchen.Confidentiality, kitchen.Confidentiality);
    Assert.Equal(payload.Name.Trim(), kitchen.Name);
    Assert.Equal(_kitchen.Slug?.Value, kitchen.Slug);
  }

  [Fact(DisplayName = "It should return null when the kitchen does not exist.")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    UpdateKitchenPayload payload = new();
    Assert.Null(await _kitchenService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "It should return null when the user does not own the kitchen.")]
  public async Task Given_NotOwner_When_Read_Then_NullReturned()
  {
    Context.User = new UserBuilder().Build();

    Assert.Null(await _kitchenService.ReadAsync(_kitchen.EntityId));
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new kitchen.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    CreateOrReplaceKitchenPayload payload = new(Faker.Company.CompanyName());

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.CreateOrReplaceAsync(payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.CreateKitchen, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing kitchen.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceKitchenPayload payload = new(Faker.Company.CompanyName());

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.CreateOrReplaceAsync(payload, _kitchen.EntityId));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Kitchen.EntityKind, _kitchen.EntityId).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing kitchen.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateKitchenPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.UpdateAsync(_kitchen.EntityId, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Kitchen.EntityKind, _kitchen.EntityId).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing kitchen.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateKitchenPayload payload = new()
    {
      Name = $"  {Faker.Company.CompanyName()}  "
    };

    KitchenModel? kitchen = await _kitchenService.UpdateAsync(_kitchen.EntityId, payload);
    Assert.NotNull(kitchen);

    Assert.Equal(_kitchen.EntityId, kitchen.Id);
    Assert.Equal(_kitchen.Version + 1, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_kitchen.OwnerId.Value, kitchen.Owner.ToActorId().Value);
    Assert.Equal(_kitchen.Confidentiality, kitchen.Confidentiality);
    Assert.Equal(payload.Name.Trim(), kitchen.Name);
    Assert.Equal(_kitchen.Slug?.Value, kitchen.Slug);
  }
}
