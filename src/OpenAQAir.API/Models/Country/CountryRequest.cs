using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAQAir.API.Models.Country
{
  /// <summary>
  /// Used as method parameter object
  /// </summary>
  public class CountryRequest
  {
    public string Keyword { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 0;
    public string SortOrder { get; set; } = string.Empty;
  }
}
