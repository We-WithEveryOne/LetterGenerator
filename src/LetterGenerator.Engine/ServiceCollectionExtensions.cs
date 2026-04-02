using LetterGenerator.Core.Interfaces;
using LetterGenerator.Engine.PdfGeneration;
using LetterGenerator.Engine.Rendering;
using LetterGenerator.Engine.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LetterGenerator.Engine;

/// <summary>
/// Extension methods to wire up all letter generation services via DI.
/// Usage in Program.cs: builder.Services.AddLetterGenerator("Templates");
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLetterGenerator(
        this IServiceCollection services,
        string templateRootPath)
    {
        // Razor template renderer (singleton — caches compiled templates)
        services.AddSingleton<IRazorTemplateRenderer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RazorTemplateRenderer>>();
            return new RazorTemplateRenderer(templateRootPath, logger);
        });

        // Playwright PDF generator (singleton — reuses browser instance)
        services.AddSingleton<IPdfGenerator, PlaywrightPdfGenerator>();

        // Letter service (transient — lightweight orchestrator)
        services.AddTransient<ILetterService, LetterService>();

        return services;
    }
}
