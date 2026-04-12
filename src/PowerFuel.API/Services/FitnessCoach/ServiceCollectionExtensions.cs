using Microsoft.Extensions.Options;

namespace PowerFuel.API.Services.FitnessCoach;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFitnessCoach(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<FitnessCoachOptions>()
            .Bind(configuration.GetSection(FitnessCoachOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), $"{FitnessCoachOptions.SectionName}:{nameof(FitnessCoachOptions.BaseUrl)} is required.")
            .ValidateOnStart();

        services.AddHttpClient<IFitnessCoachClient, FitnessCoachClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<FitnessCoachOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/", UriKind.Absolute);
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-API-Key", options.ApiKey);
        });

        return services;
    }
}
