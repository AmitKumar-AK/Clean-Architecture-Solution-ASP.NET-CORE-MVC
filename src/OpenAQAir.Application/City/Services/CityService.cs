
using OpenAQAir.Application.City.Interfaces;
using OpenAQAir.Domain.Entities;
using OpenAQAir.Domain.Interfaces;

namespace OpenAQAir.Application.City.Services
{
  public class CityService : ICityService
  {
    public ICityRepository _cityRepository;
    public CityService(ICityRepository cityRepository)
    {
      _cityRepository = cityRepository;
    }
    public Output GetCities()
    {
      var response = _cityRepository.GetCities();
      return response;
    }

    /// <summary>
    /// This function from Application layer provide information on the City.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public Output GetCities(CityQuery query)
    {
      var response = _cityRepository.GetCities(query);
      return response;
    }

  }
}
