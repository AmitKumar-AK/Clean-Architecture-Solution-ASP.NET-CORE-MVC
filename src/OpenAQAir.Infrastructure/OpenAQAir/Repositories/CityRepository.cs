using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAQAir.Domain.Entities;
using OpenAQAir.Domain.Interfaces;
using OpenAQAir.Infrastructure.OpenAQAir.Extensions;

namespace OpenAQAir.Infrastructure.OpenAQAir.Repositories
{
  public class CityRepository : ICityRepository
  {
    private readonly OpenAQAirSettings _openAQAirSettings;
    private readonly HtmlEncoder _htmlEncoder;
    private readonly ILogger<CityRepository> _logger;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// This is constructor of City Repository in Infrastructor Layer.
    /// </summary>
    /// <param name="openAQAirSettings"></param>
    /// <param name="htmlEncoder"></param>
    /// <param name="logger"></param>
    /// <param name="cache"></param>
    public CityRepository(IOptions<OpenAQAirSettings> openAQAirSettings, HtmlEncoder htmlEncoder, 
      ILogger<CityRepository> logger, IMemoryCache cache)
    {
      _openAQAirSettings = openAQAirSettings?.Value ?? throw new ArgumentNullException(nameof(openAQAirSettings));
      _htmlEncoder = htmlEncoder;
      _logger = logger;
      _cache = cache;
    }

    public Output GetCities()
    {
      //To do, We can add logic to get all the cities with pagination

      return null;
    }

    /// <summary>
    /// This function from Infrastructure layer provide information on the City.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public Output GetCities(CityQuery query)
    {
      _logger.LogInformation("Infrastructure => GetCities :: Start");
      GetAPIParameter(ref query);
      if (string.IsNullOrEmpty(query.Keyword))
      {
        _logger.LogError("Keyword is empty");
        throw new ArgumentNullException(nameof(query));
      }

      var results = _cache.GetOrCreate(
        CacheHelpers.GenerateItemCacheKey("city", query.PageSize, query.PageNumber, query.Keyword,query.SortOrder),
        cacheEntry =>
        {
          cacheEntry.SlidingExpiration = CacheHelpers.DefaultCacheDuration;
          return GetCitiesFromAPIAsync(query);
        });
      _logger.LogInformation("Infrastructure => GetCities :: End");

      return results.Result;
    }

    /// <summary>
    /// This function from Infrastructure layer Build the API url for the City.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    private string BuildUrl(CityQuery query)
    {
      string uri;
      int offSet = ((query.PageNumber - 1) * query.PageSize);
      // url 'https://api.openaq.org/v2/cities?limit=100&page=1&offset=0&sort=asc&city=Delhi&order_by=city' 

      if(query.Keyword.IndexOf(",") == -1)
      {
        #region If not more than one country
        uri = _openAQAirSettings.OpenAQAirEndPoint + "/cities?" + "limit=" + query.PageSize + "&page=" + query.PageNumber + "&offset=" + offSet
          + "&" + string.Format(Constants.OpenAQAirSearch.Parameters.SortByFieldsClause, query.SortOrder)
        + "&" + string.Format(Constants.OpenAQAirSearch.Parameters.City.CityFieldsClause, query.Keyword)
        + "&order_by=city";
        #endregion
      }
      else
      {
        #region If more than one country
        var arrKeywords = query.Keyword.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder sb = new StringBuilder("");
        if (arrKeywords != null && arrKeywords.Length >0)
        {
          foreach(string str in arrKeywords)
          {
            sb.Append("&" + string.Format(Constants.OpenAQAirSearch.Parameters.City.CityFieldsClause, str.Trim()));
          }
        }
        uri = _openAQAirSettings.OpenAQAirEndPoint + "/cities?" + "limit=" + query.PageSize + "&page=" + query.PageNumber + "&offset=" + offSet
          + "&" + string.Format(Constants.OpenAQAirSearch.Parameters.SortByFieldsClause, query.SortOrder)
        + Convert.ToString(sb)
        + "&order_by=city";
        #endregion
      }
      return uri;
    }

    /// <summary>
    /// This function from Infrastructure layer interact with OpenAQ AIR API endpoints
    /// to fetch data related to the City.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    private async Task<CityResponse> GetCitiesFromAPIAsync(CityQuery query)
    {
      CityResponse? response = null;
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
            response = JsonSerializer.Deserialize<CityResponse>(
                            task, new JsonSerializerOptions
                            {
                              PropertyNameCaseInsensitive = true
                            });
          }
        }
      }
      catch (WebException ex)
      {
        Stream? stream = null;
        using (stream = ex?.Response?.GetResponseStream())
        using (var reader = new StreamReader(stream))
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
                            +  line + line;
        _logger.LogError("Server error. Please contact administrator. Error:" + error);

      }

      return response;
    }

    /// <summary>
    /// This function will provide the required parameters for API Search
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private void GetAPIParameter(ref CityQuery request)
    {
      request.Keyword = string.IsNullOrEmpty(request.Keyword) ? string.Empty : _htmlEncoder.Encode(request.Keyword.Trim());
      request.PageNumber = (request.PageNumber == 0) ? 1 : request.PageNumber;
      request.PageSize = (request.PageSize == 0) ? _openAQAirSettings.DefaultPageSize : request.PageSize;
      request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : _htmlEncoder.Encode(request.SortOrder);
    }

  }
}

