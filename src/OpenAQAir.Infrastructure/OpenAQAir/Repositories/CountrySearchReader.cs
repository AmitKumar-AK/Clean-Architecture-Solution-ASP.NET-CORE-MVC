using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAQAir.Application.Country.Interfaces;
using OpenAQAir.Application.Country.Queries;
using OpenAQAir.Domain.Entities;
using OpenAQAir.Infrastructure.OpenAQAir.Extensions;

namespace OpenAQAir.Infrastructure.OpenAQAir.Repositories
{
  public class CountrySearchReader: ICountrySearchReader
  {

    private readonly OpenAQAirSettings _openAQAirSettings;
    private readonly HtmlEncoder _htmlEncoder;
    private readonly ILogger<ICountrySearchReader> _logger;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// This is constructor of Country Repository in Infrastructor Layer.
    /// </summary>
    /// <param name="openAQAirSettings"></param>
    /// <param name="htmlEncoder"></param>
    /// <param name="logger"></param>
    /// <param name="cache"></param>
    public CountrySearchReader(IOptions<OpenAQAirSettings> openAQAirSettings, HtmlEncoder htmlEncoder,
      ILogger<ICountrySearchReader> logger, IMemoryCache cache)
    {
      _openAQAirSettings = openAQAirSettings?.Value ?? throw new ArgumentNullException(nameof(openAQAirSettings));
      _htmlEncoder = htmlEncoder;
      _logger = logger;
      _cache = cache;
    }

    public Task<CountryResponse> GetCountriesAsync(SearchCountryQuery query, CancellationToken cancellationToken = default)
    {
      GetAPIParameter(ref query);

      var results = _cache.GetOrCreate(
        CacheHelpers.GenerateItemCacheKey("country", query.PageSize, query.PageNumber, query.Keyword, query.SortOrder),
        cacheEntry =>
        {
          cacheEntry.SlidingExpiration = CacheHelpers.DefaultCacheDuration;
          return GetCountriesFromAPIAsync(query);
        });

      return results;
    }

    private void GetAPIParameter(ref SearchCountryQuery request)
    {
      request.Keyword = string.IsNullOrEmpty(request.Keyword) ? string.Empty : _htmlEncoder.Encode(request.Keyword.Trim());
      request.PageNumber = (request.PageNumber == 0) ? 1 : request.PageNumber;
      request.PageSize = (request.PageSize == 0) ? _openAQAirSettings.DefaultPageSize : request.PageSize;
      request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : _htmlEncoder.Encode(request.SortOrder);
    }

    /// <summary>
    /// This function from Infrastructure layer interact with OpenAQ AIR API endpoints
    /// to fetch data related to the Country.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    private async Task<CountryResponse> GetCountriesFromAPIAsync(CountryQuery query)
    {
      CountryResponse? response = null;
      try
      {
        using (var client = new HttpClient())
        {
          //HTTP GET
          var responseTask = client.GetAsync(new Uri(this.BuildUrl(query)));
          responseTask.Wait();

          var result = responseTask.Result;
          if (result.IsSuccessStatusCode)
          {

            var task = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            response = JsonSerializer.Deserialize<CountryResponse>(
                            task, new JsonSerializerOptions
                            {
                              PropertyNameCaseInsensitive = true
                            });
            if (response != null)
            {
              response.CurrentPage = query.PageNumber;
              response.PageSize = query.PageSize;
              response.TotalCount = response.Meta!.Found;
              response.TotalPages = (int)Math.Ceiling(response.Meta.Found / (double)query.PageSize);
              response.PreviousPageNumber = (query.PageNumber - 1);
              response.NextPageNumber = (query.PageNumber + 1);
            }
          }
        }
      }
      catch (WebException ex)
      {
        Stream? stream = null;
        using (stream = ex?.Response?.GetResponseStream())
        using (var reader = new StreamReader(stream!))
        {
          Console.WriteLine(reader.ReadToEnd());
          _logger.LogError("Server error. Please contact administrator. Error:" + reader.ReadToEnd());
        }
      }
      catch (Exception ex)
      {
        var line = Environment.NewLine + Environment.NewLine;

        string? ErrorlineNo = ex?.StackTrace;
        string? Errormsg = ex?.Message;
        string? extype = ex.GetType().ToString();
        string? ErrorLocation = ex.Message.ToString();
        string error = "Log Written Date:" + " " + DateTime.Now.ToString()
                            + line + "Error Line No :" + " " + ErrorlineNo + line
                            + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " "
                            + extype + line + "Error Location :" + " " + ErrorLocation + line
                            + line + line;
        _logger.LogError("Server error. Please contact administrator. Error:" + error);

      }

      return response!;
    }

    /// <summary>
    /// This function from Infrastructure layer Build the API url for the Country.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    private string BuildUrl(CountryQuery query)
    {
      string uri;
      int offSet = ((query.PageNumber - 1) * query.PageSize);
      //--url 'https://api.openaq.org/v2/countries?limit=200&page=1&offset=0&sort=asc&order_by=country' \
      //--url 'https://api.openaq.org/v2/countries?limit=200&page=1&offset=0&sort=asc&country=IN&country=GB&order_by=country' \
      if (string.IsNullOrEmpty(query.Keyword))
      {
        #region If keyword is empty
        uri = _openAQAirSettings.OpenAQAirEndPoint + "/countries?" + "limit=" + query.PageSize + "&page=" + query.PageNumber + "&offset=" + offSet
          + "&" + string.Format(Constants.OpenAQAirSearch.Parameters.SortByFieldsClause, query.SortOrder)
        + "&order_by=country";
        #endregion
      }
      else
      {
        #region If keyword is not empty
        if (query.Keyword.IndexOf(",") == -1)
        {
          #region If not more than one country
          uri = _openAQAirSettings.OpenAQAirEndPoint + "/countries?" + "limit=" + query.PageSize + "&page=" + query.PageNumber + "&offset=" + offSet
            + "&" + string.Format(Constants.OpenAQAirSearch.Parameters.SortByFieldsClause, query.SortOrder)
          + "&" + string.Format(Constants.OpenAQAirSearch.Parameters.Country.CountryFieldsClause, query.Keyword)
          + "&order_by=country";
          #endregion
        }
        else
        {
          #region If more than one country
          var arrKeywords = query.Keyword.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
          StringBuilder sb = new StringBuilder("");
          if (arrKeywords != null && arrKeywords.Length > 0)
          {
            foreach (string str in arrKeywords)
            {
              sb.Append("&" + string.Format(Constants.OpenAQAirSearch.Parameters.Country.CountryFieldsClause, str.Trim()));
            }
          }
          uri = _openAQAirSettings.OpenAQAirEndPoint + "/countries?" + "limit=" + query.PageSize + "&page=" + query.PageNumber + "&offset=" + offSet
            + "&" + string.Format(Constants.OpenAQAirSearch.Parameters.SortByFieldsClause, query.SortOrder)
          + Convert.ToString(sb)
          + "&order_by=country";
          #endregion
        }
        #endregion
      }
      return uri;
    }
  }
}
