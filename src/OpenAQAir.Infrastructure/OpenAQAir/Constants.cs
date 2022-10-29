
namespace OpenAQAir.Infrastructure.OpenAQAir
{
  public struct Constants
  {
    public struct OpenAQAirSearch
    {
      public struct Parameters
      {

        public const string SortByFieldsClause = "sort={0}";
        public const string defaultStringContent = "string";
        public const string OpenAQAirSearchConfigKey = "OpenAQAirSearch";

        public struct City
        {
          public const string CityFieldsClause = "city={0}";
        }
        public struct Country
        {
          public const string CountryFieldsClause = "country={0}";
        }
      }
    }
  }
}
