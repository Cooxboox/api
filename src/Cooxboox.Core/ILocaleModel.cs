using Krakenar.Contracts.Actors;

namespace Cooxboox.Core;

public interface ILocaleModel
{
  Actor CreatedBy { get; set; }
  DateTime CreatedOn { get; set; }
  Actor UpdatedBy { get; set; }
  DateTime UpdatedOn { get; set; }
}
