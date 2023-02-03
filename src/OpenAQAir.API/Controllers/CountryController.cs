using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenAQAir.API.Models.Country;
using OpenAQAir.Application.Country.Interfaces;
using OpenAQAir.Application.Country.Queries;
using OpenAQAir.Domain.Entities;

namespace OpenAQAir.API.Controllers
{
  /// <summary>
  /// Country Controller with the use of MediatR
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  public class CountryController : ControllerBase
  {
    private readonly ILogger<CountryController> _logger;
    private ICountryService _countryService;
    private readonly IMediator _mediator;

    public CountryController(ILogger<CountryController> logger,  ICountryService countryService, IMediator mediator)
    {
      _logger = logger;
      _countryService = countryService;
      _mediator = mediator;
    }


    /// <summary>
    /// The purpose of this feature is to provide information on the Country.
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="pageNumber"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("countrylist")]
    public async Task<ActionResult> CountryList(string keyword, int pageNumber, string sortOrder)
    {
      var query = new CountryQuery()
      {
        Keyword = keyword,
        PageNumber = pageNumber,
        SortOrder = string.IsNullOrEmpty(sortOrder) ? "asc" : sortOrder
      };
      var response = await _countryService.GetCountriesAsync(query);
      return Ok(response);

    }

    [HttpGet("countrysearch")]
    [ProducesResponseType(typeof(CountryResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CountrySearchResults([FromQuery] CountryRequest request, CancellationToken cancellationToken)
    {
      //-- Call Details
      //-- API > Application.SearchCountryHandler.Handle > Infrastructure.CountrySearchReader.GetCountriesAsync
      _logger.LogInformation("API => CountrySearchResults :: Start");
      return Ok(await _mediator.Send(new SearchCountryQuery
      {
        Keyword = request.Keyword,
        PageNumber = request.PageNumber,
        SortOrder = request.SortOrder
      }, cancellationToken));
    }

  }
}
