using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.IngredientCategories;

[Trait(Traits.Category, Categories.Integration)]
public class IngredientCategoryIntegrationTests : IntegrationTests
{
  private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
  private readonly IIngredientCategoryService _ingredientCategoryService;

  private IngredientCategory _ingredientCategory = null!;

  public IngredientCategoryIntegrationTests() : base()
  {
    _ingredientCategoryRepository = ServiceProvider.GetRequiredService<IIngredientCategoryRepository>();
    _ingredientCategoryService = ServiceProvider.GetRequiredService<IIngredientCategoryService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _ingredientCategory = new IngredientCategoryBuilder(Faker).WithKitchen(Context.Kitchen).Build();
    await _ingredientCategoryRepository.SaveAsync(_ingredientCategory);
  }

  [Theory(DisplayName = "It should create a new ingredient category.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExists_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceIngredientCategoryPayload payload = new()
    {
      Name = " Agrumes ",
      Notes = "  Les agrumes regroupent les fruits des plantes du genre Citrus, comme l’orange, le citron et le pamplemousse.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceIngredientCategoryResult result = await _ingredientCategoryService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    IngredientCategoryModel ingredientCategory = result.IngredientCategory;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, ingredientCategory.Id);
    }
    Assert.Equal(2, ingredientCategory.Version);
    Assert.Equal(Actor, ingredientCategory.CreatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(ingredientCategory.CreatedBy, ingredientCategory.UpdatedBy);
    Assert.True(ingredientCategory.CreatedOn < ingredientCategory.UpdatedOn);

    Assert.Equal(payload.Name.Trim(), ingredientCategory.Name);
    Assert.Equal(payload.Notes.Trim(), ingredientCategory.Notes);
  }

  [Fact(DisplayName = "It should publish an ingredient category.")]
  public async Task Given_IngredientCategory_When_PublishAll_Then_AllPublished()
  {
    Language language = Faker.Language();
    _ingredientCategory.SetLocale(language, new IngredientCategoryLocale(_ingredientCategory.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientCategoryRepository.SaveAsync(_ingredientCategory);

    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.PublishAllAsync(_ingredientCategory.Entity.Id);
    Assert.NotNull(ingredientCategory);

    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
    Assert.Equal(_ingredientCategory.Version + 2, ingredientCategory.Version);
    Assert.Equal(_ingredientCategory.CreatedBy, ingredientCategory.CreatedBy.ToActorId());
    Assert.Equal(_ingredientCategory.CreatedOn.AsUniversalTime(), ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredientCategory.Status);
    Assert.Equal(ingredientCategory.Version - 1, ingredientCategory.PublishedVersion);
    Assert.Equal(Actor, ingredientCategory.PublishedBy);
    Assert.True(ingredientCategory.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientCategoryLocaleModel locale = Assert.Single(ingredientCategory.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should publish an ingredient category invariant.")]
  public async Task Given_IngredientCategory_When_PublishInvariant_Then_InvariantPublished()
  {
    Language language = Faker.Language();
    _ingredientCategory.SetLocale(language, new IngredientCategoryLocale(_ingredientCategory.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientCategoryRepository.SaveAsync(_ingredientCategory);

    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.PublishAsync(_ingredientCategory.Entity.Id);
    Assert.NotNull(ingredientCategory);

    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
    Assert.Equal(_ingredientCategory.Version + 1, ingredientCategory.Version);
    Assert.Equal(_ingredientCategory.CreatedBy, ingredientCategory.CreatedBy.ToActorId());
    Assert.Equal(_ingredientCategory.CreatedOn.AsUniversalTime(), ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredientCategory.Status);
    Assert.Equal(ingredientCategory.Version, ingredientCategory.PublishedVersion);
    Assert.Equal(Actor, ingredientCategory.PublishedBy);
    Assert.True(ingredientCategory.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientCategoryLocaleModel locale = Assert.Single(ingredientCategory.Locales);
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }

  [Fact(DisplayName = "It should publish an ingredient category locale.")]
  public async Task Given_IngredientCategory_When_PublishLocale_Then_LocalePublished()
  {
    Language language = Faker.Language();
    _ingredientCategory.PublishInvariant(Actor.ToActorId());
    _ingredientCategory.SetLocale(language, new IngredientCategoryLocale(_ingredientCategory.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientCategoryRepository.SaveAsync(_ingredientCategory);

    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.PublishAsync(_ingredientCategory.Entity.Id, language.Code);
    Assert.NotNull(ingredientCategory);

    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
    Assert.Equal(_ingredientCategory.Version + 1, ingredientCategory.Version);
    Assert.Equal(_ingredientCategory.CreatedBy, ingredientCategory.CreatedBy.ToActorId());
    Assert.Equal(_ingredientCategory.CreatedOn.AsUniversalTime(), ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredientCategory.Status);
    Assert.Equal(ingredientCategory.Version - 2, ingredientCategory.PublishedVersion);
    Assert.Equal(Actor, ingredientCategory.PublishedBy);
    Assert.True(ingredientCategory.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientCategoryLocaleModel locale = Assert.Single(ingredientCategory.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Theory(DisplayName = "It should unpublish an ingredient category.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_IngredientCategory_When_Unpublish_Then_Unpublished(bool unpublishInvariant, bool unpublishLocale)
  {
    Language language = Faker.Language();
    _ingredientCategory.SetLocale(language, new IngredientCategoryLocale(_ingredientCategory.Name, null, null, null, null), Actor.ToActorId());
    _ingredientCategory.Publish(Actor.ToActorId());
    await _ingredientCategoryRepository.SaveAsync(_ingredientCategory);

    IngredientCategoryModel? ingredientCategory = null;
    long version = _ingredientCategory.Version;
    if (unpublishInvariant && unpublishLocale)
    {
      ingredientCategory = await _ingredientCategoryService.UnpublishAllAsync(_ingredientCategory.Entity.Id);
      version += 2;
    }
    else if (unpublishInvariant)
    {
      ingredientCategory = await _ingredientCategoryService.UnpublishAsync(_ingredientCategory.Entity.Id);
      version++;
    }
    else if (unpublishLocale)
    {
      ingredientCategory = await _ingredientCategoryService.UnpublishAsync(_ingredientCategory.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(ingredientCategory);

    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
    Assert.Equal(version, ingredientCategory.Version);
    Assert.Equal(_ingredientCategory.CreatedBy, ingredientCategory.CreatedBy.ToActorId());
    Assert.Equal(_ingredientCategory.CreatedOn.AsUniversalTime(), ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    if (unpublishInvariant)
    {
      Assert.Equal(ContentStatus.Unpublished, ingredientCategory.Status);
      Assert.Null(ingredientCategory.PublishedVersion);
      Assert.Null(ingredientCategory.PublishedBy);
      Assert.Null(ingredientCategory.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, ingredientCategory.Status);
      Assert.Equal(Actor, ingredientCategory.PublishedBy);
      Assert.True(ingredientCategory.PublishedOn.HasValue);
    }

    IngredientCategoryLocaleModel locale = Assert.Single(ingredientCategory.Locales);
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

  [Fact(DisplayName = "It should read an ingredient category by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.ReadAsync(_ingredientCategory.Entity.Id);
    Assert.NotNull(ingredientCategory);
    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
  }

  [Fact(DisplayName = "It should replace an existing ingredient category.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceIngredientCategoryPayload payload = new()
    {
      Name = " Agrumes ",
      Notes = "  Les agrumes regroupent les fruits des plantes du genre Citrus, comme l’orange, le citron et le pamplemousse.  "
    };

    CreateOrReplaceIngredientCategoryResult result = await _ingredientCategoryService.CreateOrReplaceAsync(payload, _ingredientCategory.Entity.Id);
    Assert.False(result.Created);
    IngredientCategoryModel ingredientCategory = result.IngredientCategory;

    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
    Assert.Equal(_ingredientCategory.Version + 1, ingredientCategory.Version);
    Assert.Equal(_ingredientCategory.CreatedBy, ingredientCategory.CreatedBy.ToActorId());
    Assert.Equal(_ingredientCategory.CreatedOn.AsUniversalTime(), ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), ingredientCategory.Name);
    Assert.Equal(payload.Notes.Trim(), ingredientCategory.Notes);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NotMatching_When_Search_Then_EmptyResults()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    SearchIngredientCategoriesPayload payload = new();
    SearchResults<IngredientCategoryModel> results = await _ingredientCategoryService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when the ingredient category does not exist (Publish).")]
  public async Task Given_NotExist_When_Publish_Then_NullReturned()
  {
    Assert.Null(await _ingredientCategoryService.PublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the ingredient category does not exist (SaveLocale).")]
  public async Task Given_NotExist_When_SaveLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    SaveIngredientCategoryLocalePayload payload = new("Agrumes");
    Assert.Null(await _ingredientCategoryService.SaveLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the ingredient category does not exist (Unpublish).")]
  public async Task Given_NotExist_When_Unpublish_Then_NullReturned()
  {
    Assert.Null(await _ingredientCategoryService.UnpublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the ingredient category does not exist (Update).")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    UpdateIngredientCategoryPayload payload = new();
    Assert.Null(await _ingredientCategoryService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "It should return null when the ingredient category does not exist (UpdateLocale).")]
  public async Task Given_NotExist_When_UpdateLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    UpdateIngredientCategoryLocalePayload payload = new();
    Assert.Null(await _ingredientCategoryService.UpdateLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the user does not own the kitchen.")]
  public async Task Given_NotOwner_When_Read_Then_NullReturned()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    Assert.Null(await _ingredientCategoryService.ReadAsync(_ingredientCategory.Entity.Id));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matching_When_Search_Then_CorrectResults()
  {
    SearchIngredientCategoriesPayload payload = new()
    {
      Skip = 0,
      Limit = 1
    };
    payload.Ids.Add(_ingredientCategory.Entity.Id);
    payload.Search.Terms.Add(new SearchTerm($"%{_ingredientCategory.Name.Value[1..^1]}%"));

    SearchResults<IngredientCategoryModel> results = await _ingredientCategoryService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    IngredientCategoryModel ingredientCategory = Assert.Single(results.Items);
    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
  }

  [Theory(DisplayName = "It should save an ingredient category locale.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_IngredientCategory_When_SaveLocale_Then_Saved(bool exists)
  {
    Language language = Faker.Language();
    if (exists)
    {
      _ingredientCategory.SetLocale(language, new IngredientCategoryLocale(_ingredientCategory.Name, null, null, null, null), Actor.ToActorId());
      await _ingredientCategoryRepository.SaveAsync(_ingredientCategory);
    }

    SaveIngredientCategoryLocalePayload payload = new()
    {
      Name = " Agrumes ",
      Slug = "agrumes",
      MetaDescription = "   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   ",
      HtmlContent = "  Les agrumes regroupent les fruits des plantes du genre Citrus, comme l’orange, le citron et le pamplemousse.  ",
      Notes = "    "
    };

    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.SaveLocaleAsync(_ingredientCategory.Entity.Id, language.Code, payload);
    Assert.NotNull(ingredientCategory);

    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
    Assert.Equal(_ingredientCategory.Version + 1, ingredientCategory.Version);
    Assert.Equal(_ingredientCategory.CreatedOn.AsUniversalTime(), ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_ingredientCategory.CreatedBy, ingredientCategory.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, ingredientCategory.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientCategory.UpdatedBy);

    IngredientCategoryLocaleModel locale = Assert.Single(ingredientCategory.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(payload.Name.Trim(), locale.Name);
    Assert.Equal(payload.Slug, locale.Slug);
    Assert.Equal(payload.MetaDescription.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(ingredientCategory.Version, locale.Version);
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
    _ingredientCategory.SetLocale(language, new IngredientCategoryLocale(_ingredientCategory.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientCategoryRepository.SaveAsync(_ingredientCategory);

    var exception = await Assert.ThrowsAsync<InvariantNotPublishedException>(async () => await _ingredientCategoryService.PublishAsync(_ingredientCategory.Entity.Id, language.Code));
    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_ingredientCategory.Entity, entity);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new ingredient category.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceIngredientCategoryPayload payload = new("Agrumes");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientCategoryService.CreateOrReplaceAsync(payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.CreateIngredientCategory, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when publishing an ingredient category.")]
  public async Task Given_Exists_When_Publishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientCategoryService.PublishAsync(_ingredientCategory.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Publish, exception.Action);
    Assert.Equal(new Entity(IngredientCategory.EntityKind, _ingredientCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing ingredient category.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceIngredientCategoryPayload payload = new("Agrumes");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientCategoryService.CreateOrReplaceAsync(payload, _ingredientCategory.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientCategory.EntityKind, _ingredientCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when saving an ingredient category locale.")]
  public async Task Given_Exists_When_SaveLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    SaveIngredientCategoryLocalePayload payload = new("Agrumes");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientCategoryService.SaveLocaleAsync(_ingredientCategory.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientCategory.EntityKind, _ingredientCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when unpublishing an ingredient category.")]
  public async Task Given_Exists_When_Unpublishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientCategoryService.UnpublishAsync(_ingredientCategory.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Unpublish, exception.Action);
    Assert.Equal(new Entity(IngredientCategory.EntityKind, _ingredientCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing ingredient category.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateIngredientCategoryPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientCategoryService.UpdateAsync(_ingredientCategory.Entity.Id, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientCategory.EntityKind, _ingredientCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an ingredient category locale.")]
  public async Task Given_Exists_When_UpdateLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    UpdateIngredientCategoryLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientCategoryService.UpdateLocaleAsync(_ingredientCategory.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientCategory.EntityKind, _ingredientCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw LocaleNotFoundException when the ingredient category locale was not found.")]
  public async Task Given_LocaleNotFound_When_UpdateLocale_Then_LocaleNotFoundException()
  {
    Language language = Faker.Language();
    UpdateIngredientCategoryLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<LocaleNotFoundException>(async () => await _ingredientCategoryService.UpdateLocaleAsync(_ingredientCategory.Entity.Id, language.Code, payload));
    Assert.Equal(language.ToString(), exception.Language);

    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_ingredientCategory.Entity, entity);
  }

  [Fact(DisplayName = "It should update an existing ingredient category.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateIngredientCategoryPayload payload = new()
    {
      Notes = new Optional<string>("  Les agrumes regroupent les fruits des plantes du genre Citrus, comme l’orange, le citron et le pamplemousse.  ")
    };

    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.UpdateAsync(_ingredientCategory.Entity.Id, payload);
    Assert.NotNull(ingredientCategory);

    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
    Assert.Equal(_ingredientCategory.Version + 1, ingredientCategory.Version);
    Assert.Equal(_ingredientCategory.CreatedBy, ingredientCategory.CreatedBy.ToActorId());
    Assert.Equal(_ingredientCategory.CreatedOn.AsUniversalTime(), ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_ingredientCategory.Name.Value, ingredientCategory.Name);
    Assert.Equal(payload.Notes.Value?.Trim(), ingredientCategory.Notes);
  }

  [Fact(DisplayName = "It should update an ingredient category locale.")]
  public async Task Given_LocaleFound_When_UpdateLocale_Then_Updated()
  {
    Language language = Faker.Language();
    _ingredientCategory.SetLocale(language, new IngredientCategoryLocale(_ingredientCategory.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientCategoryRepository.SaveAsync(_ingredientCategory);

    UpdateIngredientCategoryLocalePayload payload = new()
    {
      Slug = new Optional<string>("agrumes"),
      MetaDescription = new Optional<string>("   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   "),
      HtmlContent = new Optional<string>("  Les agrumes regroupent les fruits des plantes du genre Citrus, comme l’orange, le citron et le pamplemousse.  "),
      Notes = new Optional<string>("    ")
    };

    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.UpdateLocaleAsync(_ingredientCategory.Entity.Id, language.Code, payload);
    Assert.NotNull(ingredientCategory);

    Assert.Equal(_ingredientCategory.Entity.Id, ingredientCategory.Id);
    Assert.Equal(_ingredientCategory.Version + 1, ingredientCategory.Version);
    Assert.Equal(_ingredientCategory.CreatedOn.AsUniversalTime(), ingredientCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_ingredientCategory.CreatedBy, ingredientCategory.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, ingredientCategory.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientCategory.UpdatedBy);

    IngredientCategoryLocaleModel locale = Assert.Single(ingredientCategory.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(_ingredientCategory.Name.Value, locale.Name);
    Assert.Equal(payload.Slug.Value, locale.Slug);
    Assert.Equal(payload.MetaDescription.Value?.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Value?.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(ingredientCategory.Version, locale.Version);
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
