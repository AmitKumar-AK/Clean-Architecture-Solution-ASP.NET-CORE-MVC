
namespace OpenAQAir.Domain.Entities
{
  public class CityResponse: Output
  {
    public IEnumerable<CityDetails>? Results { get; set; }
  }
}
