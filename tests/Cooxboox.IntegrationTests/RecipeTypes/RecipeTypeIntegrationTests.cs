using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeTypes;
using Cooxboox.Core.RecipeTypes.Models;
using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.RecipeTypes;

[Trait(Traits.Category, Categories.Integration)]
public class RecipeTypeIntegrationTests : IntegrationTests
{
  private readonly IRecipeTypeRepository _recipeTypeRepository;
  private readonly IRecipeTypeService _recipeTypeService;

  private RecipeType _recipeType = null!;

  public RecipeTypeIntegrationTests() : base()
  {
    _recipeTypeRepository = ServiceProvider.GetRequiredService<IRecipeTypeRepository>();
    _recipeTypeService = ServiceProvider.GetRequiredService<IRecipeTypeService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _recipeType = new RecipeTypeBuilder(Faker).WithKitchen(Context.Kitchen).Build();
    await _recipeTypeRepository.SaveAsync(_recipeType);
  }

  [Theory(DisplayName = "It should create a new recipe type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExists_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceRecipeTypePayload payload = new()
    {
      Name = " Plat principal ",
      Notes = "  Le plat principal constitue le cœur du repas, généralement servi après l’entrée et avant le dessert.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceRecipeTypeResult result = await _recipeTypeService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    RecipeTypeModel recipeType = result.RecipeType;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, recipeType.Id);
    }
    Assert.Equal(2, recipeType.Version);
    Assert.Equal(Actor, recipeType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(recipeType.CreatedBy, recipeType.UpdatedBy);
    Assert.True(recipeType.CreatedOn < recipeType.UpdatedOn);

    Assert.Equal(payload.Name.Trim(), recipeType.Name);
    Assert.Equal(payload.Notes.Trim(), recipeType.Notes);
  }

  [Fact(DisplayName = "It should publish a recipe type.")]
  public async Task Given_RecipeType_When_PublishAll_Then_AllPublished()
  {
    Language language = Faker.Language();
    _recipeType.SetLocale(language, new RecipeTypeLocale(_recipeType.Name, null, null, null, null), Actor.ToActorId());
    await _recipeTypeRepository.SaveAsync(_recipeType);

    RecipeTypeModel? recipeType = await _recipeTypeService.PublishAllAsync(_recipeType.Entity.Id);
    Assert.NotNull(recipeType);

    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
    Assert.Equal(_recipeType.Version + 2, recipeType.Version);
    Assert.Equal(_recipeType.CreatedBy, recipeType.CreatedBy.ToActorId());
    Assert.Equal(_recipeType.CreatedOn.AsUniversalTime(), recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, recipeType.Status);
    Assert.Equal(recipeType.Version - 1, recipeType.PublishedVersion);
    Assert.Equal(Actor, recipeType.PublishedBy);
    Assert.True(recipeType.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, recipeType.PublishedOn.Value, TimeSpan.FromSeconds(10));

    RecipeTypeLocaleModel locale = Assert.Single(recipeType.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should publish a recipe type invariant.")]
  public async Task Given_RecipeType_When_PublishInvariant_Then_InvariantPublished()
  {
    Language language = Faker.Language();
    _recipeType.SetLocale(language, new RecipeTypeLocale(_recipeType.Name, null, null, null, null), Actor.ToActorId());
    await _recipeTypeRepository.SaveAsync(_recipeType);

    RecipeTypeModel? recipeType = await _recipeTypeService.PublishAsync(_recipeType.Entity.Id);
    Assert.NotNull(recipeType);

    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
    Assert.Equal(_recipeType.Version + 1, recipeType.Version);
    Assert.Equal(_recipeType.CreatedBy, recipeType.CreatedBy.ToActorId());
    Assert.Equal(_recipeType.CreatedOn.AsUniversalTime(), recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, recipeType.Status);
    Assert.Equal(recipeType.Version, recipeType.PublishedVersion);
    Assert.Equal(Actor, recipeType.PublishedBy);
    Assert.True(recipeType.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, recipeType.PublishedOn.Value, TimeSpan.FromSeconds(10));

    RecipeTypeLocaleModel locale = Assert.Single(recipeType.Locales);
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }

  [Fact(DisplayName = "It should publish a recipe type locale.")]
  public async Task Given_RecipeType_When_PublishLocale_Then_LocalePublished()
  {
    Language language = Faker.Language();
    _recipeType.PublishInvariant(Actor.ToActorId());
    _recipeType.SetLocale(language, new RecipeTypeLocale(_recipeType.Name, null, null, null, null), Actor.ToActorId());
    await _recipeTypeRepository.SaveAsync(_recipeType);

    RecipeTypeModel? recipeType = await _recipeTypeService.PublishAsync(_recipeType.Entity.Id, language.Code);
    Assert.NotNull(recipeType);

    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
    Assert.Equal(_recipeType.Version + 1, recipeType.Version);
    Assert.Equal(_recipeType.CreatedBy, recipeType.CreatedBy.ToActorId());
    Assert.Equal(_recipeType.CreatedOn.AsUniversalTime(), recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, recipeType.Status);
    Assert.Equal(recipeType.Version - 2, recipeType.PublishedVersion);
    Assert.Equal(Actor, recipeType.PublishedBy);
    Assert.True(recipeType.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, recipeType.PublishedOn.Value, TimeSpan.FromSeconds(10));

    RecipeTypeLocaleModel locale = Assert.Single(recipeType.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Theory(DisplayName = "It should unpublish an recipe type.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_RecipeType_When_Unpublish_Then_Unpublished(bool unpublishInvariant, bool unpublishLocale)
  {
    Language language = Faker.Language();
    _recipeType.SetLocale(language, new RecipeTypeLocale(_recipeType.Name, null, null, null, null), Actor.ToActorId());
    _recipeType.Publish(Actor.ToActorId());
    await _recipeTypeRepository.SaveAsync(_recipeType);

    RecipeTypeModel? recipeType = null;
    long version = _recipeType.Version;
    if (unpublishInvariant && unpublishLocale)
    {
      recipeType = await _recipeTypeService.UnpublishAllAsync(_recipeType.Entity.Id);
      version += 2;
    }
    else if (unpublishInvariant)
    {
      recipeType = await _recipeTypeService.UnpublishAsync(_recipeType.Entity.Id);
      version++;
    }
    else if (unpublishLocale)
    {
      recipeType = await _recipeTypeService.UnpublishAsync(_recipeType.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(recipeType);

    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
    Assert.Equal(version, recipeType.Version);
    Assert.Equal(_recipeType.CreatedBy, recipeType.CreatedBy.ToActorId());
    Assert.Equal(_recipeType.CreatedOn.AsUniversalTime(), recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeType.UpdatedOn, TimeSpan.FromSeconds(10));

    if (unpublishInvariant)
    {
      Assert.Equal(ContentStatus.Unpublished, recipeType.Status);
      Assert.Null(recipeType.PublishedVersion);
      Assert.Null(recipeType.PublishedBy);
      Assert.Null(recipeType.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, recipeType.Status);
      Assert.Equal(Actor, recipeType.PublishedBy);
      Assert.True(recipeType.PublishedOn.HasValue);
    }

    RecipeTypeLocaleModel locale = Assert.Single(recipeType.Locales);
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

  [Fact(DisplayName = "It should read an recipe type by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.ReadAsync(_recipeType.Entity.Id);
    Assert.NotNull(recipeType);
    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
  }

  [Fact(DisplayName = "It should replace an existing recipe type.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceRecipeTypePayload payload = new()
    {
      Name = " Plat principal ",
      Notes = "  Le plat principal constitue le cœur du repas, généralement servi après l’entrée et avant le dessert.  "
    };

    CreateOrReplaceRecipeTypeResult result = await _recipeTypeService.CreateOrReplaceAsync(payload, _recipeType.Entity.Id);
    Assert.False(result.Created);
    RecipeTypeModel recipeType = result.RecipeType;

    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
    Assert.Equal(_recipeType.Version + 2, recipeType.Version);
    Assert.Equal(_recipeType.CreatedBy, recipeType.CreatedBy.ToActorId());
    Assert.Equal(_recipeType.CreatedOn.AsUniversalTime(), recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), recipeType.Name);
    Assert.Equal(payload.Notes.Trim(), recipeType.Notes);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NotMatching_When_Search_Then_EmptyResults()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    SearchRecipeTypesPayload payload = new();
    SearchResults<RecipeTypeModel> results = await _recipeTypeService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when the recipe type does not exist (Publish).")]
  public async Task Given_NotExist_When_Publish_Then_NullReturned()
  {
    Assert.Null(await _recipeTypeService.PublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the recipe type does not exist (SaveLocale).")]
  public async Task Given_NotExist_When_SaveLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    SaveRecipeTypeLocalePayload payload = new("Plat principal");
    Assert.Null(await _recipeTypeService.SaveLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the recipe type does not exist (Unpublish).")]
  public async Task Given_NotExist_When_Unpublish_Then_NullReturned()
  {
    Assert.Null(await _recipeTypeService.UnpublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the recipe type does not exist (Update).")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    UpdateRecipeTypePayload payload = new();
    Assert.Null(await _recipeTypeService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "It should return null when the recipe type does not exist (UpdateLocale).")]
  public async Task Given_NotExist_When_UpdateLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    UpdateRecipeTypeLocalePayload payload = new();
    Assert.Null(await _recipeTypeService.UpdateLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the user does not own the kitchen.")]
  public async Task Given_NotOwner_When_Read_Then_NullReturned()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    Assert.Null(await _recipeTypeService.ReadAsync(_recipeType.Entity.Id));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matching_When_Search_Then_CorrectResults()
  {
    SearchRecipeTypesPayload payload = new()
    {
      Skip = 0,
      Limit = 1
    };
    payload.Ids.Add(_recipeType.Entity.Id);
    payload.Search.Terms.Add(new SearchTerm($"%{_recipeType.Name.Value[1..^1]}%"));

    SearchResults<RecipeTypeModel> results = await _recipeTypeService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    RecipeTypeModel recipeType = Assert.Single(results.Items);
    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
  }

  [Theory(DisplayName = "It should save an recipe type locale.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_RecipeType_When_SaveLocale_Then_Saved(bool exists)
  {
    Language language = Faker.Language();
    if (exists)
    {
      _recipeType.SetLocale(language, new RecipeTypeLocale(_recipeType.Name, null, null, null, null), Actor.ToActorId());
      await _recipeTypeRepository.SaveAsync(_recipeType);
    }

    SaveRecipeTypeLocalePayload payload = new()
    {
      Name = " Plat principal ",
      Slug = "plat-principal",
      MetaDescription = "   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   ",
      HtmlContent = "  Le plat principal constitue le cœur du repas, généralement servi après l’entrée et avant le dessert.  ",
      Notes = "    "
    };

    RecipeTypeModel? recipeType = await _recipeTypeService.SaveLocaleAsync(_recipeType.Entity.Id, language.Code, payload);
    Assert.NotNull(recipeType);

    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
    Assert.Equal(_recipeType.Version + 1, recipeType.Version);
    Assert.Equal(_recipeType.CreatedOn.AsUniversalTime(), recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_recipeType.CreatedBy, recipeType.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, recipeType.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeType.UpdatedBy);

    RecipeTypeLocaleModel locale = Assert.Single(recipeType.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(payload.Name.Trim(), locale.Name);
    Assert.Equal(payload.Slug, locale.Slug);
    Assert.Equal(payload.MetaDescription.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(recipeType.Version, locale.Version);
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
    _recipeType.SetLocale(language, new RecipeTypeLocale(_recipeType.Name, null, null, null, null), Actor.ToActorId());
    await _recipeTypeRepository.SaveAsync(_recipeType);

    var exception = await Assert.ThrowsAsync<InvariantNotPublishedException>(async () => await _recipeTypeService.PublishAsync(_recipeType.Entity.Id, language.Code));
    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_recipeType.Entity, entity);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new recipe type.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceRecipeTypePayload payload = new("Plat principal");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeTypeService.CreateOrReplaceAsync(payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.CreateRecipeType, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when publishing an recipe type.")]
  public async Task Given_Exists_When_Publishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeTypeService.PublishAsync(_recipeType.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Publish, exception.Action);
    Assert.Equal(new Entity(RecipeType.EntityKind, _recipeType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing recipe type.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceRecipeTypePayload payload = new("Plat principal");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeTypeService.CreateOrReplaceAsync(payload, _recipeType.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(RecipeType.EntityKind, _recipeType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when saving an recipe type locale.")]
  public async Task Given_Exists_When_SaveLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    SaveRecipeTypeLocalePayload payload = new("Plat principal");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeTypeService.SaveLocaleAsync(_recipeType.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(RecipeType.EntityKind, _recipeType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when unpublishing an recipe type.")]
  public async Task Given_Exists_When_Unpublishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeTypeService.UnpublishAsync(_recipeType.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Unpublish, exception.Action);
    Assert.Equal(new Entity(RecipeType.EntityKind, _recipeType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing recipe type.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateRecipeTypePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeTypeService.UpdateAsync(_recipeType.Entity.Id, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(RecipeType.EntityKind, _recipeType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an recipe type locale.")]
  public async Task Given_Exists_When_UpdateLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    UpdateRecipeTypeLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeTypeService.UpdateLocaleAsync(_recipeType.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(RecipeType.EntityKind, _recipeType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw LocaleNotFoundException when the recipe type locale was not found.")]
  public async Task Given_LocaleNotFound_When_UpdateLocale_Then_LocaleNotFoundException()
  {
    Language language = Faker.Language();
    UpdateRecipeTypeLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<LocaleNotFoundException>(async () => await _recipeTypeService.UpdateLocaleAsync(_recipeType.Entity.Id, language.Code, payload));
    Assert.Equal(language.ToString(), exception.Language);

    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_recipeType.Entity, entity);
  }

  [Fact(DisplayName = "It should update an existing recipe type.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateRecipeTypePayload payload = new()
    {
      Notes = new Optional<string>("  Le plat principal constitue le cœur du repas, généralement servi après l’entrée et avant le dessert.  ")
    };

    RecipeTypeModel? recipeType = await _recipeTypeService.UpdateAsync(_recipeType.Entity.Id, payload);
    Assert.NotNull(recipeType);

    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
    Assert.Equal(_recipeType.Version + 1, recipeType.Version);
    Assert.Equal(_recipeType.CreatedBy, recipeType.CreatedBy.ToActorId());
    Assert.Equal(_recipeType.CreatedOn.AsUniversalTime(), recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_recipeType.Name.Value, recipeType.Name);
    Assert.Equal(payload.Notes.Value?.Trim(), recipeType.Notes);
  }

  [Fact(DisplayName = "It should update an recipe type locale.")]
  public async Task Given_LocaleFound_When_UpdateLocale_Then_Updated()
  {
    Language language = Faker.Language();
    _recipeType.SetLocale(language, new RecipeTypeLocale(_recipeType.Name, null, null, null, null), Actor.ToActorId());
    await _recipeTypeRepository.SaveAsync(_recipeType);

    UpdateRecipeTypeLocalePayload payload = new()
    {
      Slug = new Optional<string>("plat-principal"),
      MetaDescription = new Optional<string>("   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   "),
      HtmlContent = new Optional<string>("  Le plat principal constitue le cœur du repas, généralement servi après l’entrée et avant le dessert.  "),
      Notes = new Optional<string>("    ")
    };

    RecipeTypeModel? recipeType = await _recipeTypeService.UpdateLocaleAsync(_recipeType.Entity.Id, language.Code, payload);
    Assert.NotNull(recipeType);

    Assert.Equal(_recipeType.Entity.Id, recipeType.Id);
    Assert.Equal(_recipeType.Version + 1, recipeType.Version);
    Assert.Equal(_recipeType.CreatedOn.AsUniversalTime(), recipeType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_recipeType.CreatedBy, recipeType.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, recipeType.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeType.UpdatedBy);

    RecipeTypeLocaleModel locale = Assert.Single(recipeType.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(_recipeType.Name.Value, locale.Name);
    Assert.Equal(payload.Slug.Value, locale.Slug);
    Assert.Equal(payload.MetaDescription.Value?.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Value?.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(recipeType.Version, locale.Version);
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
