using Cooxboox.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure;

public class CooxbooxContext : DbContext
{
  public CooxbooxContext(DbContextOptions<CooxbooxContext> options) : base(options)
  {
  }

  internal DbSet<IngredientCategoryEntity> IngredientCategories => Set<IngredientCategoryEntity>();
  internal DbSet<IngredientCategoryLocaleEntity> IngredientCategoryLocales => Set<IngredientCategoryLocaleEntity>();
  internal DbSet<IngredientTypeEntity> IngredientTypes => Set<IngredientTypeEntity>();
  internal DbSet<IngredientTypeLocaleEntity> IngredientTypeLocales => Set<IngredientTypeLocaleEntity>();
  internal DbSet<KitchenEntity> Kitchens => Set<KitchenEntity>();
  internal DbSet<RecipeCategoryEntity> RecipeCategories => Set<RecipeCategoryEntity>();
  internal DbSet<RecipeCategoryLocaleEntity> RecipeCategoryLocales => Set<RecipeCategoryLocaleEntity>();
  internal DbSet<RecipeTypeEntity> RecipeTypes => Set<RecipeTypeEntity>();
  internal DbSet<RecipeTypeLocaleEntity> RecipeTypeLocales => Set<RecipeTypeLocaleEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
