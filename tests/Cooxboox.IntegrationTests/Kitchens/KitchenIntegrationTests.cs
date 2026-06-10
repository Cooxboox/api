using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
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
    KitchenModel? kitchen = await _kitchenService.ReadAsync(_kitchen.Entity.Id);
    Assert.NotNull(kitchen);
    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
  }

  [Fact(DisplayName = "It should replace an existing kitchen.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceKitchenPayload payload = new($"  {Faker.Company.CompanyName()}  ");

    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, _kitchen.Entity.Id);
    Assert.False(result.Created);
    KitchenModel kitchen = result.Kitchen;

    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
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

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NotMatching_When_Search_Then_EmptyResults()
  {
    Context.User = new UserBuilder().Build();

    SearchKitchensPayload payload = new();
    SearchResults<KitchenModel> results = await _kitchenService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
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

    Assert.Null(await _kitchenService.ReadAsync(_kitchen.Entity.Id));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matching_When_Search_Then_CorrectResults()
  {
    SearchKitchensPayload payload = new()
    {
      Confidentiality = _kitchen.Confidentiality,
      Skip = 0,
      Limit = 1
    };
    payload.Ids.Add(_kitchen.Entity.Id);
    payload.Search.Terms.Add(new SearchTerm($"%{_kitchen.Name.Value[1..^1]}%"));

    SearchResults<KitchenModel> results = await _kitchenService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    KitchenModel kitchen = Assert.Single(results.Items);
    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
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

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.CreateOrReplaceAsync(payload, _kitchen.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Kitchen.EntityKind, _kitchen.Entity.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing kitchen.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateKitchenPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.UpdateAsync(_kitchen.Entity.Id, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Kitchen.EntityKind, _kitchen.Entity.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing kitchen.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateKitchenPayload payload = new()
    {
      Slug = new Optional<string>("my-new-kitchen"),
      Notes = new Optional<string>(" Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris non facilisis sapien. Proin vivamus.\r\n\r\n ")
    };

    KitchenModel? kitchen = await _kitchenService.UpdateAsync(_kitchen.Entity.Id, payload);
    Assert.NotNull(kitchen);

    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
    Assert.Equal(_kitchen.Version + 2, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_kitchen.OwnerId.Value, kitchen.Owner.ToActorId().Value);
    Assert.Equal(_kitchen.Confidentiality, kitchen.Confidentiality);
    Assert.Equal(_kitchen.Name.Value, kitchen.Name);
    Assert.Equal(payload.Slug.Value, kitchen.Slug);
    Assert.Equal(payload.Notes.Value?.Trim(), kitchen.Notes);
  }

  [Fact(DisplayName = "It should publish a kitchen.")]
  public async Task Given_Kitchen_When_PublishAll_Then_AllPublished()
  {
    Language language = Faker.Language();
    _kitchen.SetLocale(language, new KitchenLocale(null, null, null), Actor.ToActorId());
    await _kitchenRepository.SaveAsync(_kitchen);

    KitchenModel? kitchen = await _kitchenService.PublishAllAsync(_kitchen.Entity.Id);
    Assert.NotNull(kitchen);

    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
    Assert.Equal(_kitchen.Version + 2, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, kitchen.Status);
    Assert.Equal(kitchen.Version - 1, kitchen.PublishedVersion);
    Assert.Equal(Actor, kitchen.PublishedBy);
    Assert.True(kitchen.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, kitchen.PublishedOn.Value, TimeSpan.FromSeconds(10));

    KitchenLocaleModel locale = Assert.Single(kitchen.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should publish a kitchen invariant.")]
  public async Task Given_Kitchen_When_PublishInvariant_Then_InvariantPublished()
  {
    Language language = Faker.Language();
    _kitchen.SetLocale(language, new KitchenLocale(null, null, null), Actor.ToActorId());
    await _kitchenRepository.SaveAsync(_kitchen);

    KitchenModel? kitchen = await _kitchenService.PublishAsync(_kitchen.Entity.Id);
    Assert.NotNull(kitchen);

    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
    Assert.Equal(_kitchen.Version + 1, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, kitchen.Status);
    Assert.Equal(kitchen.Version, kitchen.PublishedVersion);
    Assert.Equal(Actor, kitchen.PublishedBy);
    Assert.True(kitchen.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, kitchen.PublishedOn.Value, TimeSpan.FromSeconds(10));

    KitchenLocaleModel locale = Assert.Single(kitchen.Locales);
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }

  [Fact(DisplayName = "It should publish a kitchen locale.")]
  public async Task Given_Kitchen_When_PublishLocale_Then_LocalePublished()
  {
    Language language = Faker.Language();
    _kitchen.PublishInvariant(Actor.ToActorId());
    _kitchen.SetLocale(language, new KitchenLocale(null, null, null), Actor.ToActorId());
    await _kitchenRepository.SaveAsync(_kitchen);

    KitchenModel? kitchen = await _kitchenService.PublishAsync(_kitchen.Entity.Id, language.Code);
    Assert.NotNull(kitchen);

    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
    Assert.Equal(_kitchen.Version + 1, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, kitchen.Status);
    Assert.Equal(kitchen.Version - 2, kitchen.PublishedVersion);
    Assert.Equal(Actor, kitchen.PublishedBy);
    Assert.True(kitchen.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, kitchen.PublishedOn.Value, TimeSpan.FromSeconds(10));

    KitchenLocaleModel locale = Assert.Single(kitchen.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Theory(DisplayName = "It should unpublish a kitchen.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_Kitchen_When_Unpublish_Then_Unpublished(bool unpublishInvariant, bool unpublishLocale)
  {
    Language language = Faker.Language();
    _kitchen.SetLocale(language, new KitchenLocale(null, null, null), Actor.ToActorId());
    _kitchen.Publish(Actor.ToActorId());
    await _kitchenRepository.SaveAsync(_kitchen);

    KitchenModel? kitchen = null;
    long version = _kitchen.Version;
    if (unpublishInvariant && unpublishLocale)
    {
      kitchen = await _kitchenService.UnpublishAllAsync(_kitchen.Entity.Id);
      version += 2;
    }
    else if (unpublishInvariant)
    {
      kitchen = await _kitchenService.UnpublishAsync(_kitchen.Entity.Id);
      version++;
    }
    else if (unpublishLocale)
    {
      kitchen = await _kitchenService.UnpublishAsync(_kitchen.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(kitchen);

    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
    Assert.Equal(version, kitchen.Version);
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));

    if (unpublishInvariant)
    {
      Assert.Equal(ContentStatus.Unpublished, kitchen.Status);
      Assert.Null(kitchen.PublishedVersion);
      Assert.Null(kitchen.PublishedBy);
      Assert.Null(kitchen.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, kitchen.Status);
      Assert.Equal(Actor, kitchen.PublishedBy);
      Assert.True(kitchen.PublishedOn.HasValue);
    }

    KitchenLocaleModel locale = Assert.Single(kitchen.Locales);
    if (unpublishLocale)
    {
      Assert.Equal(ContentStatus.Unpublished, locale.Status);
      Assert.Null(locale.PublishedVersion);
      Assert.Null(locale.PublishedBy);
      Assert.Null(locale.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, locale.Status);
      Assert.Equal(Actor, locale.PublishedBy);
      Assert.True(locale.PublishedOn.HasValue);
    }
  }

  [Fact(DisplayName = "It should return null when the kitchen does not exist (Publish).")]
  public async Task Given_NotExist_When_Publish_Then_NullReturned()
  {
    Assert.Null(await _kitchenService.PublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the kitchen does not exist (SaveLocale).")]
  public async Task Given_NotExist_When_SaveLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    SaveKitchenLocalePayload payload = new();
    Assert.Null(await _kitchenService.SaveLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the kitchen does not exist (Unpublish).")]
  public async Task Given_NotExist_When_Unpublish_Then_NullReturned()
  {
    Assert.Null(await _kitchenService.UnpublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the kitchen does not exist (UpdateLocale).")]
  public async Task Given_NotExist_When_UpdateLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    UpdateKitchenLocalePayload payload = new();
    Assert.Null(await _kitchenService.UpdateLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Theory(DisplayName = "It should save a kitchen locale.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_Kitchen_When_SaveLocale_Then_Saved(bool exists)
  {
    Language language = Faker.Language();
    if (exists)
    {
      _kitchen.SetLocale(language, new KitchenLocale(null, null, null), Actor.ToActorId());
      await _kitchenRepository.SaveAsync(_kitchen);
    }

    SaveKitchenLocalePayload payload = new()
    {
      MetaDescription = "   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   ",
      HtmlContent = "  La cuisine est l'espace où sont préparés et partagés les repas de la maison.  ",
      Notes = "    "
    };

    KitchenModel? kitchen = await _kitchenService.SaveLocaleAsync(_kitchen.Entity.Id, language.Code, payload);
    Assert.NotNull(kitchen);

    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
    Assert.Equal(_kitchen.Version + 1, kitchen.Version);
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);

    KitchenLocaleModel locale = Assert.Single(kitchen.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(payload.MetaDescription.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(kitchen.Version, locale.Version);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.Equal(locale.CreatedBy, locale.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, locale.CreatedOn, TimeSpan.FromSeconds(10));
    if (exists)
    {
      Assert.True(locale.CreatedOn < locale.UpdatedOn);
    }
    else
    {
      Assert.Equal(locale.CreatedOn, locale.UpdatedOn);
    }
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }

  [Fact(DisplayName = "It should throw InvariantNotPublishedException when the invariant is not published.")]
  public async Task Given_UnpublishedInvariant_When_PublishLocale_Then_InvariantNotPublishedException()
  {
    Language language = Faker.Language();
    _kitchen.SetLocale(language, new KitchenLocale(null, null, null), Actor.ToActorId());
    await _kitchenRepository.SaveAsync(_kitchen);

    var exception = await Assert.ThrowsAsync<InvariantNotPublishedException>(async () => await _kitchenService.PublishAsync(_kitchen.Entity.Id, language.Code));
    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_kitchen.Entity, entity);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when publishing a kitchen.")]
  public async Task Given_Exists_When_Publishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.PublishAsync(_kitchen.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Publish, exception.Action);
    Assert.Equal(new Entity(Kitchen.EntityKind, _kitchen.Entity.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when saving a kitchen locale.")]
  public async Task Given_Exists_When_SaveLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    SaveKitchenLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.SaveLocaleAsync(_kitchen.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Kitchen.EntityKind, _kitchen.Entity.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when unpublishing a kitchen.")]
  public async Task Given_Exists_When_Unpublishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.UnpublishAsync(_kitchen.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Unpublish, exception.Action);
    Assert.Equal(new Entity(Kitchen.EntityKind, _kitchen.Entity.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a kitchen locale.")]
  public async Task Given_Exists_When_UpdateLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    UpdateKitchenLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _kitchenService.UpdateLocaleAsync(_kitchen.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Kitchen.EntityKind, _kitchen.Entity.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw LocaleNotFoundException when the kitchen locale was not found.")]
  public async Task Given_LocaleNotFound_When_UpdateLocale_Then_LocaleNotFoundException()
  {
    Language language = Faker.Language();
    UpdateKitchenLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<LocaleNotFoundException>(async () => await _kitchenService.UpdateLocaleAsync(_kitchen.Entity.Id, language.Code, payload));
    Assert.Equal(language.ToString(), exception.Language);

    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_kitchen.Entity, entity);
  }

  [Fact(DisplayName = "It should update a kitchen locale.")]
  public async Task Given_LocaleFound_When_UpdateLocale_Then_Updated()
  {
    Language language = Faker.Language();
    _kitchen.SetLocale(language, new KitchenLocale(null, null, null), Actor.ToActorId());
    await _kitchenRepository.SaveAsync(_kitchen);

    UpdateKitchenLocalePayload payload = new()
    {
      MetaDescription = new Optional<string>("   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   "),
      HtmlContent = new Optional<string>("  La cuisine est l'espace où sont préparés et partagés les repas de la maison.  "),
      Notes = new Optional<string>("    ")
    };

    KitchenModel? kitchen = await _kitchenService.UpdateLocaleAsync(_kitchen.Entity.Id, language.Code, payload);
    Assert.NotNull(kitchen);

    Assert.Equal(_kitchen.Entity.Id, kitchen.Id);
    Assert.Equal(_kitchen.Version + 1, kitchen.Version);
    Assert.Equal(_kitchen.CreatedOn.AsUniversalTime(), kitchen.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_kitchen.CreatedBy, kitchen.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, kitchen.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, kitchen.UpdatedBy);

    KitchenLocaleModel locale = Assert.Single(kitchen.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(payload.MetaDescription.Value?.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Value?.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(kitchen.Version, locale.Version);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.Equal(locale.CreatedBy, locale.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, locale.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.True(locale.CreatedOn < locale.UpdatedOn);
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }
}
