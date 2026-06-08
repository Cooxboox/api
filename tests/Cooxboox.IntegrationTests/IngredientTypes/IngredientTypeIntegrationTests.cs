using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.IngredientTypes.Models;
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
  public async Task Given_NotExists_When_CreateOrReplace_ThenCreated(bool withId)
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
    Assert.Equal(_ingredientType.Version + 1, ingredientType.Version);
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

  [Fact(DisplayName = "It should return null when the ingredient type does not exist.")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    UpdateIngredientTypePayload payload = new();
    Assert.Null(await _ingredientTypeService.UpdateAsync(Guid.Empty, payload));
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

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new kitchen.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceIngredientTypePayload payload = new("Fruits et légumes");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.CreateOrReplaceAsync(payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.CreateIngredientType, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing kitchen.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceIngredientTypePayload payload = new("Fruits et légumes");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.CreateOrReplaceAsync(payload, _ingredientType.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientType.EntityKind, _ingredientType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing kitchen.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateIngredientTypePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientTypeService.UpdateAsync(_ingredientType.Entity.Id, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(IngredientType.EntityKind, _ingredientType.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should update an existing ingredient type.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateIngredientTypePayload payload = new()
    {
      Notes = new Optional<string>("  Dans le langage courant, il désigne généralement une plante cultivée au jardin ou dans les champs. Un fruit peut donc bien être un légume.  ")
    };

    IngredientTypeModel? ingredientType = await _ingredientTypeService.UpdateAsync(_ingredientType.Entity.Id, payload);
    Assert.NotNull(ingredientType);

    Assert.Equal(_ingredientType.Entity.Id, ingredientType.Id);
    Assert.Equal(_ingredientType.Version + 1, ingredientType.Version);
    Assert.Equal(_ingredientType.CreatedBy, ingredientType.CreatedBy.ToActorId());
    Assert.Equal(_ingredientType.CreatedOn.AsUniversalTime(), ingredientType.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredientType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredientType.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_ingredientType.Name.Value, ingredientType.Name);
    Assert.Equal(payload.Notes.Value?.Trim(), ingredientType.Notes);
  }
}
