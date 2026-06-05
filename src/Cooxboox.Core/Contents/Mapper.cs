using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Contents;

namespace Cooxboox.Core.Contents;

internal static class Mapper
{
  public static KitchenModel ToKitchen(Content content)
  {
    KitchenModel kitchen = new()
    {
      Id = Guid.Parse(content.Invariant.UniqueName),
      Version = content.Version,
      CreatedBy = content.CreatedBy,
      CreatedOn = content.CreatedOn,
      UpdatedBy = content.UpdatedBy,
      UpdatedOn = content.UpdatedOn,
      Name = content.Invariant.DisplayName ?? content.Invariant.UniqueName
    };

    Dictionary<Guid, string> fields = content.Invariant.FieldValues.ToDictionary(x => x.Id, x => x.Value);
    if (fields.TryGetValue(KitchenDefinition.Owner, out string? owner))
    {
      // TODO(fpion): implement
      kitchen.Owner.Type = ActorType.User;
      kitchen.Owner.Id = Guid.Parse(owner);
    }
    if (fields.TryGetValue(KitchenDefinition.Confidentiality, out string? confidentiality))
    {
      kitchen.Confidentiality = Enum.Parse<Confidentiality>(confidentiality[2..^2]);
    }
    if (fields.TryGetValue(KitchenDefinition.Slug, out string? slug))
    {
      kitchen.Slug = slug;
    }

    foreach (ContentLocale locale in content.Locales)
    {
      kitchen.Locales.Add(ToKitchenLocale(locale));
    }

    return kitchen;
  }

  public static KitchenLocaleModel ToKitchenLocale(ContentLocale content)
  {
    KitchenLocaleModel locale = new();

    if (content.Language is not null)
    {
      locale.Language = content.Language.Locale;
    }

    Dictionary<Guid, string> fields = content.FieldValues.ToDictionary(x => x.Id, x => x.Value);
    if (fields.TryGetValue(KitchenDefinition.MetaDescription, out string? metaDescription))
    {
      locale.MetaDescription = metaDescription;
    }
    if (fields.TryGetValue(KitchenDefinition.HtmlContent, out string? htmlContent))
    {
      locale.HtmlContent = htmlContent;
    }

    return locale;
  }
}
