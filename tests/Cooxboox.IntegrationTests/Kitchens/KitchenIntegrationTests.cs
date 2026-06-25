using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
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

    _kitchen = new KitchenBuilder().WithOwner(Context.User).Build();
    _kitchenRepository.Add(_kitchen);
    await Context.SaveChangesAsync();
  }

  [Theory(DisplayName = "It should create a new kitchen.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceKitchenPayload payload = new()
    {
      Name = " Hell’s Kitchen ",
      Slug = "Hell-s-Kitchen",
      Notes = "  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec consequat tortor sem, eu molestie in.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    KitchenModel kitchen = result.Kitchen;
    Assert.NotNull(kitchen);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, kitchen.Id);
    }
    Assert.Equal(1, kitchen.Version);
    Assert.Equal(Actor, kitchen.CreatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(kitchen.CreatedBy, kitchen.UpdatedBy);
    Assert.Equal(kitchen.CreatedOn, kitchen.UpdatedOn);

    Assert.Equal(kitchen.CreatedBy, kitchen.Owner);
    Assert.Equal(Confidentiality.Private, kitchen.Confidentiality);
    Assert.Equal(payload.Name.Trim(), kitchen.Name);
    Assert.Equal(payload.Slug.ToLowerInvariant(), kitchen.Slug);
    Assert.Equal(payload.Notes.Trim(), kitchen.Notes);
  }

  [Fact(DisplayName = "It should replace an existing kitchen.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceKitchenPayload payload = new()
    {
      Name = " Hell’s Kitchen ",
      Slug = "Hell-s-Kitchen",
      Notes = "  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec consequat tortor sem, eu molestie in.  "
    };
    Guid id = _kitchen.Id;

    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, id);
    Assert.False(result.Created);

    KitchenModel kitchen = result.Kitchen;
    Assert.NotNull(kitchen);

    Assert.Equal(id, kitchen.Id);
    Assert.Equal(2, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.Id);
    Assert.Equal(_kitchen.CreatedOn, kitchen.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(Actor, kitchen.Owner);
    Assert.Equal(Confidentiality.Private, kitchen.Confidentiality);
    Assert.Equal(payload.Name.Trim(), kitchen.Name);
    Assert.Equal(payload.Slug.ToLowerInvariant(), kitchen.Slug);
    Assert.Equal(payload.Notes.Trim(), kitchen.Notes);
  }

  [Fact(DisplayName = "It should read a kitchen by ID.")]
  public async Task Given_Id_When_Read_Then_Kitchen()
  {
    KitchenModel? kitchen = await _kitchenService.ReadAsync(_kitchen.Id);
    Assert.NotNull(kitchen);
    Assert.Equal(_kitchen.Id, kitchen.Id);
  }

  [Fact(DisplayName = "It should return null when the kitchen does not exist.")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    Assert.Null(await _kitchenService.UpdateAsync(Guid.Empty, new UpdateKitchenPayload()));
  }

  [Fact(DisplayName = "It should return null when the user does not own the kitchen.")]
  public async Task Given_NotOwner_When_Read_Then_NullReturned()
  {
    Context.User = new UserBuilder().Build();

    Assert.Null(await _kitchenService.ReadAsync(_kitchen.Id));
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new kitchen.")]
  public async Task Given_NotAllowed_When_Create_Then_PermissionDeniedException()
  {
    Kitchen anotherKitchen = new KitchenBuilder().WithOwner(Context.User).Build();
    _kitchenRepository.Add(anotherKitchen);
    await Context.SaveChangesAsync();

    CreateOrReplaceKitchenPayload payload = new()
    {
      Name = " Hell’s Kitchen ",
      Slug = "Hell-s-Kitchen",
      Notes = "  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec consequat tortor sem, eu molestie in.  "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.CreateOrReplaceAsync(payload));
    Assert.Equal(Context.UserId, exception.UserId);
    Assert.Equal(Actions.CreateKitchen, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing kitchen.")]
  public async Task Given_NotAllowed_When_Replace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceKitchenPayload payload = new()
    {
      Name = " Hell’s Kitchen ",
      Slug = "Hell-s-Kitchen",
      Notes = "  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec consequat tortor sem, eu molestie in.  "
    };

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.CreateOrReplaceAsync(payload, _kitchen.Id));
    Assert.Equal(Context.User.Id, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new ResourceIdentifier(Kitchen.ResourceKind, _kitchen.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing kitchen.")]
  public async Task Given_NotAllowed_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.UpdateAsync(_kitchen.Id, new UpdateKitchenPayload()));
    Assert.Equal(Context.User.Id, exception.UserId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new ResourceIdentifier(Kitchen.ResourceKind, _kitchen.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing kitchen.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Guid id = _kitchen.Id;
    UpdateKitchenPayload payload = new()
    {
      Name = " Hell’s Kitchen ",
      Slug = new Optional<string>("Hell-s-Kitchen"),
      Notes = new Optional<string>("  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec consequat tortor sem, eu molestie in.  ")
    };

    KitchenModel? kitchen = await _kitchenService.UpdateAsync(id, payload);
    Assert.NotNull(kitchen);

    Assert.Equal(id, kitchen.Id);
    Assert.Equal(2, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.Id);
    Assert.Equal(_kitchen.CreatedOn, kitchen.CreatedOn, TimeSpan.FromMilliseconds(1));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(Actor, kitchen.Owner);
    Assert.Equal(Confidentiality.Private, kitchen.Confidentiality);
    Assert.Equal(payload.Name.Trim(), kitchen.Name);
    Assert.Equal(payload.Slug.Value?.ToLowerInvariant(), kitchen.Slug);
    Assert.Equal(payload.Notes.Value?.Trim(), kitchen.Notes);
  }
}
