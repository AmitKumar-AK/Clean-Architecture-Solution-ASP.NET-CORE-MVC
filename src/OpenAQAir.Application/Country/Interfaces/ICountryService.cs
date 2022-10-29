using OpenAQAir.Domain.Entities;

namespace OpenAQAir.Application.Country.Interfaces
{
  public interface ICountryService
  {
    public Output GetCountries(CountryQuery query);
  }
}
