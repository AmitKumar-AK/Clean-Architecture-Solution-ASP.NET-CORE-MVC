﻿
using OpenAQAir.Domain.Entities;

namespace OpenAQAir.Domain.Interfaces
{
  public interface ICountryRepository
  {
    Output GetCountries(CountryQuery query);
  }
}
