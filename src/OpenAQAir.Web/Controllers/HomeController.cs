using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenAQAir.Application.City.Interfaces;
using OpenAQAir.Application.Country.Interfaces;
using OpenAQAir.Domain.Entities;
using OpenAQAir.Web.Models;
using System.Diagnostics;


namespace OpenAQAir.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ICityService _cityService;
        private ICountryService _countryService;

    public HomeController(ILogger<HomeController> logger, ICityService cityService, ICountryService countryService)
        {
          _logger = logger;
          _cityService = cityService;
          _countryService = countryService;
        }

    public IActionResult Index()
        {
          return View();
        }

    /// <summary>
    /// The purpose of this feature is to provide information on the City
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult PartialViewCity(string keyword, string sortOrder)
        {
        _logger.LogInformation("Web => PartialViewCity :: Start");

      var query = new CityQuery()
          {
            Keyword = keyword,
            PageNumber = 1,
            SortOrder = string.IsNullOrEmpty(sortOrder) ? "asc" : sortOrder
    };
          Output model = _cityService.GetCities(query);
      _logger.LogInformation("Web => PartialViewCity :: Start");
      return PartialView("_cityView", model);
    }

    /// <summary>
    /// The purpose of this feature is to provide information on the Country.
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="pageNumber"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult PartialViewCountry(string keyword, int pageNumber, string sortOrder)
    {
      var query = new CountryQuery()
      {
        Keyword = keyword,
        PageNumber = pageNumber,
        SortOrder = string.IsNullOrEmpty(sortOrder) ? "asc" : sortOrder
      };
      Output model = _countryService.GetCountries(query);
      return PartialView("_countryView", model);

    }

    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
  }
}
