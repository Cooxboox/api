namespace Cooxboox.Core;

public abstract class AggregateEntity // TODO(fpion): this is bad!
{
  public Guid Id { get; set; }
  public long Version { get; set; }

  public Guid? CreatedBy { get; set; }
  public DateTime CreatedOn { get; set; }

  public Guid? UpdatedBy { get; set; }
  public DateTime UpdatedOn { get; set; }
}
