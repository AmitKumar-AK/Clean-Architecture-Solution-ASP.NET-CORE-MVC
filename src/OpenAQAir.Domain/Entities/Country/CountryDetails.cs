
namespace OpenAQAir.Domain.Entities
{
  public class CountryDetails : BaseEntity
  {
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int Cities { get; set; }
    public string[]? Parameters { get; set; }
  }
}
