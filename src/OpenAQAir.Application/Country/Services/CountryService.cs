using OpenAQAir.Application.Country.Interfaces;
using OpenAQAir.Application.Country.Queries;
using OpenAQAir.Domain.Entities;
using OpenAQAir.Domain.Interfaces;

namespace OpenAQAir.Application.Country.Services
{
  public class CountryService : ICountryService
  {
    public ICountryRepository _countryRepository;
    public CountryService(ICountryRepository countryRepository)
    {
      _countryRepository = countryRepository;
    }

    /// <summary>
    /// This function from Application layer provide information on the Country.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public Output GetCountries(CountryQuery query)
    {
      var response = _countryRepository.GetCountries(query);
      return response;
    }

    public Task<CountryResponse> GetCountriesAsync(CountryQuery query)
    {
      var response = _countryRepository.GetCountriesAsync(query);
      return response;
    }

  }
}
