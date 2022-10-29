using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAQAir.Application.City.Interfaces;
using OpenAQAir.Application.City.Services;
using OpenAQAir.Application.Country.Interfaces;
using OpenAQAir.Application.Country.Services;
using OpenAQAir.Domain.Interfaces;
using OpenAQAir.Infrastructure.OpenAQAir.Repositories;

namespace OpenAQAir.Infrastructure.OpenAQAir
{
  /// <summary>
  /// This function from Infrastructure layer use to inject the application/Infra functions.
  /// </summary>
  public static class DependencyInjectionExtensions
  {
    public static void AddOpenAQAirSearch(this IServiceCollection collection,
    IConfiguration configuration, IServiceCollection services)
    {
      var openAQAirSearchApiConfiguration =
          configuration.GetSection(Constants.OpenAQAirSearch.Parameters.OpenAQAirSearchConfigKey);

      collection.AddOptions<OpenAQAirSettings>().BindConfiguration(Constants.OpenAQAirSearch.Parameters.OpenAQAirSearchConfigKey)
          .ValidateDataAnnotations();

      var openAQAirAPISettings = openAQAirSearchApiConfiguration.Get<OpenAQAirSettings>();

      //Application
      services.AddScoped<ICityService, CityService>();
      services.AddScoped<ICountryService, CountryService>();

      //Core Interfaces | Infrastructure Repositories
      services.AddScoped<ICityRepository, CityRepository>();
      services.AddScoped<ICountryRepository, CountryRepository>();

    }
  }


}
