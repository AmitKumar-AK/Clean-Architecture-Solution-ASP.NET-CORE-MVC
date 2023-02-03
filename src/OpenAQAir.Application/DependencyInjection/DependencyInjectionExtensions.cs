using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OpenAQAir.Application.Common.Behaviours;

namespace OpenAQAir.Application.DependencyInjection
{
  public static class DependencyInjectionExtensions
  {
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
      services.AddAutoMapper(Assembly.GetExecutingAssembly());
      services.AddMediatR(Assembly.GetExecutingAssembly());
      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
      return services;
    }
  }
}
