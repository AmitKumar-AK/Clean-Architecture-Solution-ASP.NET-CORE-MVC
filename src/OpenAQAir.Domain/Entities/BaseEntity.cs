namespace OpenAQAir.Domain.Entities
{
  public class BaseEntity
  {
    public int Locations { get; set; }
    public string? FirstUpdated { get; set; }
    public string? LastUpdated { get; set; }
    public double Count { get; set; }

  }
}
