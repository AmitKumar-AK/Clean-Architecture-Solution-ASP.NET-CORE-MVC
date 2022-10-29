
namespace OpenAQAir.Infrastructure.OpenAQAir.Extensions
{
  public static class CacheHelpers
  {
    public static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromSeconds(5*60);
    private static readonly string _itemsKeyTemplate = "items-{0}-{1}-{2}-{3}-{4}-{5}";

    /// <summary>
    /// This function from Infrastructure layer provide the Cache key.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageNumber"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public static string GenerateItemCacheKey(string type,int pageSize, int pageNumber, string keyword, string sortOrder)
    {
      int offSet = ((pageNumber - 1) * pageSize);
      return string.Format(_itemsKeyTemplate,type, keyword.Replace(",","-"), pageNumber, pageSize, offSet,sortOrder);
    }

  }
}
