using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Models.Kitchen;

public record SearchKitchensParameters : SearchParameters
{
  [FromQuery(Name = "confidentiality")]
  public Confidentiality? Confidentiality { get; set; }

  public virtual SearchKitchensPayload ToPayload()
  {
    SearchKitchensPayload payload = new()
    {
      Confidentiality = Confidentiality
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out KitchenSort field))
      {
        payload.Sort.Add(new KitchenSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
