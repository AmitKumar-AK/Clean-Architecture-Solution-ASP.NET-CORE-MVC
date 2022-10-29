namespace OpenAQAir.Domain.Entities
{
  public class CityDetails : BaseEntity
  {
    public string? Country { get;  set; }
    public string? City { get; set; }
    public string[]? Parameters { get; set; }
  }
}
