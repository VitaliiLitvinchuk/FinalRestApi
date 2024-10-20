using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Tests.Common;

public static class TestsExtensions
{
    public static async Task<T> ToResponseModel<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content)!;
    }
}

public static class TestFactoryExtensions
{
    public static void RemoveServiceByType(this IServiceCollection services, Type serviceType)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == serviceType);

        if (descriptor is not null)
            services.Remove(descriptor);
    }
}