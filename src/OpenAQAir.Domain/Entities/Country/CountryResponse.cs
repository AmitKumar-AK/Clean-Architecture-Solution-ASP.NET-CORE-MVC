namespace OpenAQAir.Domain.Entities
{
  public class CountryResponse : Output
  {
    public IEnumerable<CountryDetails>? Results { get; set; }
  }
}
