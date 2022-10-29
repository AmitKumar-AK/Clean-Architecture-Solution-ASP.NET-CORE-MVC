
namespace OpenAQAir.Domain.Entities
{
  public class Output
  {
    public Meta? Meta { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public int PreviousPageNumber { get; set; }
    public int NextPageNumber { get; set; }

  }


  public class Meta
  {
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Found { get; set; }
  }

}
