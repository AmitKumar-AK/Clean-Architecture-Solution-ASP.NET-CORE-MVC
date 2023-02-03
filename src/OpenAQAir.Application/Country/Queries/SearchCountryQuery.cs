using MediatR;
using OpenAQAir.Domain.Entities;

namespace OpenAQAir.Application.Country.Queries
{
  public class SearchCountryQuery : CountryQuery,IRequest<CountryResponse>
  {
  }
}
