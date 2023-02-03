using OpenAQAir.Application.Country.Queries;
using OpenAQAir.Domain.Entities;

namespace OpenAQAir.Application.Country.Interfaces
{
  public interface ICountryService
  {
    public Output GetCountries(CountryQuery query);

    public Task<CountryResponse> GetCountriesAsync(CountryQuery query);
  }
}
