using OpenAQAir.Application.Country.Queries;
using OpenAQAir.Domain.Entities;

namespace OpenAQAir.Application.Country.Interfaces
{
  public interface ICountrySearchReader
  {
    Task<CountryResponse> GetCountriesAsync(SearchCountryQuery query, CancellationToken cancellationToken = default);
  }
}
