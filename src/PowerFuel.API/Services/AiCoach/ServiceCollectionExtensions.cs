using Microsoft.Extensions.Options;

namespace PowerFuel.API.Services.AiCoach;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiCoach(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AiCoachOptions>()
            .Bind(configuration.GetSection(AiCoachOptions.SectionName))
            .Validate(o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _), "AiCoach:BaseUrl must be an absolute URL.")
            .ValidateOnStart();

        services.AddHttpClient<IAiCoachClient, AiCoachClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AiCoachOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        return services;
    }
}

