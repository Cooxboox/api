using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.IngredientTypes;

[Trait(Traits.Category, Categories.Integration)]
public class IngredientTypeIntegrationTests : IntegrationTests
{
  private readonly IIngredientTypeRepository _ingredientTypeRepository;
  private readonly IIngredientTypeService _ingredientTypeService;

  private IngredientType _ingredientType = null!;

  public IngredientTypeIntegrationTests() : base()
  {
    _ingredientTypeRepository = ServiceProvider.GetRequiredService<IIngredientTypeRepository>();
    _ingredientTypeService = ServiceProvider.GetRequiredService<IIngredientTypeService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _ingredientType = new IngredientTypeBuilder(Faker).WithKitchen(Context.Kitchen).Build();
    await _ingredientTypeRepository.SaveAsync(_ingredientType);
  }

  [Theory(DisplayName = "It should create a new ingredient type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExists_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceIngredientTypePayload payload = new()
    {
      Name = " Fruits et légumes ",
      Notes = "  Dans le langage courant, il désigne généralement une plante cultivée au jardin ou dans les champs. Un fruit peut donc bien être un légume.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceIngredientTypeResult result = await _ingredientTypeService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    IngredientTypeModel ingredientType = result.IngredientType;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, ingredientType.Id);
    }
    Assert.Equal(2, ingredientType.Version);
    Assert.Equal(Actor, ingredientType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(ingredientType.CreatedBy, ingredientType.UpdatedBy);
    Assert.True(ingredientType.CreatedOn < ingredientType.UpdatedOn);

    Assert.Equal(payload.Name.Trim(), ingredientType.Name);
    Assert.Equal(payload.Notes.Trim(), ingredientType.Notes);
  }

  [Fact(DisplayName = "It should publish an ingredient type.")]
  public async Task Given_IngredientType_When_PublishAll_Then_AllPublished()
  {
    Language language = Faker.Language();
    _ingredientType.SetLocale(language, new IngredientTypeLocale(_ingredientType.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientTypeRepository.SaveAsync(_ingredientType);

    IngredientTypeModel? ingredientType = await _ingredientTypeService.PublishAllAsync(_ingredientType.Entity.Id);
    Assert.NotNull(ingredientType);

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(_ingredientType.Version + 2, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredientType.Status);
    Assert.Equal(ingredientType.Version - 1, ingredientType.PublishedVersion);
    Assert.Equal(Actor, ingredientType.PublishedBy);
    Assert.True(ingredientType.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredientType.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientTypeLocaleModel locale = Assert.Single(ingredientType.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should publish an ingredient type invariant.")]
  public async Task Given_IngredientType_When_PublishInvariant_Then_InvariantPublished()
  {
    Language language = Faker.Language();
    _ingredientType.SetLocale(language, new IngredientTypeLocale(_ingredientType.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientTypeRepository.SaveAsync(_ingredientType);

    IngredientTypeModel? ingredientType = await _ingredientTypeService.PublishAsync(_ingredientType.Entity.Id);
    Assert.NotNull(ingredientType);

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(_ingredientType.Version + 1, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredientType.Status);
    Assert.Equal(ingredientType.Version, ingredientType.PublishedVersion);
    Assert.Equal(Actor, ingredientType.PublishedBy);
    Assert.True(ingredientType.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredientType.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientTypeLocaleModel locale = Assert.Single(ingredientType.Locales);
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }

  [Fact(DisplayName = "It should publish an ingredient type locale.")]
  public async Task Given_IngredientType_When_PublishLocale_Then_LocalePublished()
  {
    Language language = Faker.Language();
    _ingredientType.PublishInvariant(Actor.ToActorId());
    _ingredientType.SetLocale(language, new IngredientTypeLocale(_ingredientType.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientTypeRepository.SaveAsync(_ingredientType);

    IngredientTypeModel? ingredientType = await _ingredientTypeService.PublishAsync(_ingredientType.Entity.Id, language.Code);
    Assert.NotNull(ingredientType);

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(_ingredientType.Version + 1, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredientType.Status);
    Assert.Equal(ingredientType.Version - 2, ingredientType.PublishedVersion);
    Assert.Equal(Actor, ingredientType.PublishedBy);
    Assert.True(ingredientType.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredientType.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientTypeLocaleModel locale = Assert.Single(ingredientType.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Theory(DisplayName = "It should unpublish an ingredient type.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_IngredientType_When_Unpublish_Then_Unpublished(bool unpublishInvariant, bool unpublishLocale)
  {
    Language language = Faker.Language();
    _ingredientType.SetLocale(language, new IngredientTypeLocale(_ingredientType.Name, null, null, null, null), Actor.ToActorId());
    _ingredientType.Publish(Actor.ToActorId());
    await _ingredientTypeRepository.SaveAsync(_ingredientType);

    IngredientTypeModel? ingredientType = null;
    long version = _ingredientType.Version;
    if (unpublishInvariant && unpublishLocale)
    {
      ingredientType = await _ingredientTypeService.UnpublishAllAsync(_ingredientType.Entity.Id);
      version += 2;
    }
    else if (unpublishInvariant)
    {
      ingredientType = await _ingredientTypeService.UnpublishAsync(_ingredientType.Entity.Id);
      version++;
    }
    else if (unpublishLocale)
    {
      ingredientType = await _ingredientTypeService.UnpublishAsync(_ingredientType.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(ingredientType);

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(version, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));

    if (unpublishInvariant)
    {
      Assert.Equal(ContentStatus.Unpublished, ingredientType.Status);
      Assert.Null(ingredientType.PublishedVersion);
      Assert.Null(ingredientType.PublishedBy);
      Assert.Null(ingredientType.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, ingredientType.Status);
      Assert.Equal(Actor, ingredientType.PublishedBy);
      Assert.True(ingredientType.PublishedOn.HasValue);
    }

    IngredientTypeLocaleModel locale = Assert.Single(ingredientType.Locales);
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

  [Fact(DisplayName = "It should read an ingredient type by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.ReadAsync(_ingredientType.Entity.Id);
    Assert.NotNull(ingredientType);
    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
  }

  [Fact(DisplayName = "It should replace an existing ingredient type.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceIngredientTypePayload payload = new()
    {
      Name = " Fruits et légumes ",
      Notes = "  Dans le langage courant, il désigne généralement une plante cultivée au jardin ou dans les champs. Un fruit peut donc bien être un légume.  "
    };

    CreateOrReplaceIngredientTypeResult result = await _ingredientTypeService.CreateOrReplaceAsync(payload, _ingredientType.Entity.Id);
    Assert.False(result.Created);
    IngredientTypeModel ingredientType = result.IngredientType;

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(_ingredientType.Version + 2, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), ingredientType.Name);
    Assert.Equal(payload.Notes.Trim(), ingredientType.Notes);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NotMatching_When_Search_Then_EmptyResults()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    SearchIngredientTypesPayload payload = new();
    SearchResults<IngredientTypeModel> results = await _ingredientTypeService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when the ingredient type does not exist (Publish).")]
  public async Task Given_NotExist_When_Publish_Then_NullReturned()
  {
    Assert.Null(await _ingredientTypeService.PublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the ingredient type does not exist (SaveLocale).")]
  public async Task Given_NotExist_When_SaveLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    SaveIngredientTypeLocalePayload payload = new("Fruits et légumes");
    Assert.Null(await _ingredientTypeService.SaveLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the ingredient type does not exist (Unpublish).")]
  public async Task Given_NotExist_When_Unpublish_Then_NullReturned()
  {
    Assert.Null(await _ingredientTypeService.UnpublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the ingredient type does not exist (Update).")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    UpdateIngredientTypePayload payload = new();
    Assert.Null(await _ingredientTypeService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "It should return null when the ingredient type does not exist (UpdateLocale).")]
  public async Task Given_NotExist_When_UpdateLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    UpdateIngredientTypeLocalePayload payload = new();
    Assert.Null(await _ingredientTypeService.UpdateLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the user does not own the kitchen.")]
  public async Task Given_NotOwner_When_Read_Then_NullReturned()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    Assert.Null(await _ingredientTypeService.ReadAsync(_ingredientType.Entity.Id));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matching_When_Search_Then_CorrectResults()
  {
    SearchIngredientTypesPayload payload = new()
    {
      Skip = 0,
      Limit = 1
    };
    payload.Ids.Add(_ingredientType.Entity.Id);
    payload.Search.Terms.Add(new SearchTerm($"%{_ingredientType.Name.Value[1..^1]}%"));

    SearchResults<IngredientTypeModel> results = await _ingredientTypeService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    IngredientTypeModel ingredientType = Assert.Single(results.Items);
    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
  }

  [Theory(DisplayName = "It should save an ingredient type locale.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_IngredientType_When_SaveLocale_Then_Saved(bool exists)
  {
    Language language = Faker.Language();
    if (exists)
    {
      _ingredientType.SetLocale(language, new IngredientTypeLocale(_ingredientType.Name, null, null, null, null), Actor.ToActorId());
      await _ingredientTypeRepository.SaveAsync(_ingredientType);
    }

    SaveIngredientTypeLocalePayload payload = new()
    {
      Name = " Fruits et légumes ",
      Slug = "fruits-et-legumes",
      MetaDescription = "   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   ",
      HtmlContent = "  Dans le langage courant, il désigne généralement une plante cultivée au jardin ou dans les champs. Un fruit peut donc bien être un légume.  ",
      Notes = "    "
    };

    IngredientTypeModel? ingredientType = await _ingredientTypeService.SaveLocaleAsync(_ingredientType.Entity.Id, language.Code, payload);
    Assert.NotNull(ingredientType);

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(_ingredientType.Version + 1, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);

    IngredientTypeLocaleModel locale = Assert.Single(ingredientType.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(payload.Name.Trim(), locale.Name);
    Assert.Equal(payload.Slug, locale.Slug);
    Assert.Equal(payload.MetaDescription.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(ingredientType.Version, locale.Version);
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
    _ingredientType.SetLocale(language, new IngredientTypeLocale(_ingredientType.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientTypeRepository.SaveAsync(_ingredientType);

    var exception = await Assert.ThrowsAsync<InvariantNotPublishedException>(async () => await _ingredientTypeService.PublishAsync(_ingredientType.Entity.Id, language.Code));
    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_ingredientType.Entity, entity);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new ingredient type.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceIngredientTypePayload payload = new("Fruits et légumes");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.CreateOrReplaceAsync(payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.CreateIngredientType, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when publishing an ingredient type.")]
  public async Task Given_Exists_When_Publishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.PublishAsync(_ingredientType.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Publish, exception.Action);
    Assert.Equal(new Entity(IngredientType.EntityKind, _ingredientType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing ingredient type.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceIngredientTypePayload payload = new("Fruits et légumes");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.CreateOrReplaceAsync(payload, _ingredientType.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientType.EntityKind, _ingredientType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when saving an ingredient type locale.")]
  public async Task Given_Exists_When_SaveLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    SaveIngredientTypeLocalePayload payload = new("Fruits et légumes");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.SaveLocaleAsync(_ingredientType.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientType.EntityKind, _ingredientType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when unpublishing an ingredient type.")]
  public async Task Given_Exists_When_Unpublishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.UnpublishAsync(_ingredientType.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Unpublish, exception.Action);
    Assert.Equal(new Entity(IngredientType.EntityKind, _ingredientType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing ingredient type.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateIngredientTypePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.UpdateAsync(_ingredientType.Entity.Id, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientType.EntityKind, _ingredientType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an ingredient type locale.")]
  public async Task Given_Exists_When_UpdateLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    UpdateIngredientTypeLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.UpdateLocaleAsync(_ingredientType.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientType.EntityKind, _ingredientType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw LocaleNotFoundException when the ingredient type locale was not found.")]
  public async Task Given_LocaleNotFound_When_UpdateLocale_Then_LocaleNotFoundException()
  {
    Language language = Faker.Language();
    UpdateIngredientTypeLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<LocaleNotFoundException>(async () => await _ingredientTypeService.UpdateLocaleAsync(_ingredientType.Entity.Id, language.Code, payload));
    Assert.Equal(language.ToString(), exception.Language);

    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_ingredientType.Entity, entity);
  }

  [Fact(DisplayName = "It should update an existing ingredient type.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateIngredientTypePayload payload = new()
    {
      Icon = new Optional<string>("Emoji:Vegetables"),
      Notes = new Optional<string>("  Dans le langage courant, il désigne généralement une plante cultivée au jardin ou dans les champs. Un fruit peut donc bien être un légume.  ")
    };

    IngredientTypeModel? ingredientType = await _ingredientTypeService.UpdateAsync(_ingredientType.Entity.Id, payload);
    Assert.NotNull(ingredientType);

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(_ingredientType.Version + 2, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_ingredientType.Name.Value, ingredientType.Name);
    Assert.Equal(payload.Icon.Value?.ToLowerInvariant(), ingredientType.Icon);
    Assert.Equal(payload.Notes.Value?.Trim(), ingredientType.Notes);
  }

  [Fact(DisplayName = "It should update an ingredient type locale.")]
  public async Task Given_LocaleFound_When_UpdateLocale_Then_Updated()
  {
    Language language = Faker.Language();
    _ingredientType.SetLocale(language, new IngredientTypeLocale(_ingredientType.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientTypeRepository.SaveAsync(_ingredientType);

    UpdateIngredientTypeLocalePayload payload = new()
    {
      Slug = new Optional<string>("fruits-et-legumes"),
      MetaDescription = new Optional<string>("   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   "),
      HtmlContent = new Optional<string>("  Dans le langage courant, il désigne généralement une plante cultivée au jardin ou dans les champs. Un fruit peut donc bien être un légume.  "),
      Notes = new Optional<string>("    ")
    };

    IngredientTypeModel? ingredientType = await _ingredientTypeService.UpdateLocaleAsync(_ingredientType.Entity.Id, language.Code, payload);
    Assert.NotNull(ingredientType);

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(_ingredientType.Version + 1, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);

    IngredientTypeLocaleModel locale = Assert.Single(ingredientType.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(_ingredientType.Name.Value, locale.Name);
    Assert.Equal(payload.Slug.Value, locale.Slug);
    Assert.Equal(payload.MetaDescription.Value?.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Value?.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(ingredientType.Version, locale.Version);
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
