using Cooxboox.Core.Identity;
using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.Ingredients;
using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.Recipes;
using Cooxboox.Core.RecipeTypes;
using Logitar.CQRS;
using Logitar.EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxCore(this IServiceCollection services)
  {
    return services
      .AddCoreServices()
      .AddLogitarEventSourcing()
      .AddSingleton(serviceProvider => RetrySettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddTransient<ICommandBus, CommandBus>()
      .AddTransient<IQueryBus, QueryBus>();
  }

  private static IServiceCollection AddCoreServices(this IServiceCollection services)
  {
    IdentityService.Register(services);
    IngredientCategoryService.Register(services);
    IngredientService.Register(services);
    IngredientTypeService.Register(services);
    KitchenService.Register(services);
    PermissionService.Register(services);
    RecipeCategoryService.Register(services);
    RecipeService.Register(services);
    RecipeTypeService.Register(services);
    return services;
  }
}
