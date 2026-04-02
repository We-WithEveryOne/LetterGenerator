using LetterGenerator.Core.Interfaces;
using Microsoft.Extensions.Logging;
using RazorLight;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Linq;

namespace LetterGenerator.Engine.Rendering;

/// <summary>
/// Renders Razor (.cshtml) templates using RazorLight — a standalone Razor engine
/// that works outside of ASP.NET MVC. Templates are loaded from a folder on disk.
/// </summary>
public class RazorTemplateRenderer : IRazorTemplateRenderer
{
    private readonly RazorLightEngine _engine;
    private readonly ILogger<RazorTemplateRenderer> _logger;

    public RazorTemplateRenderer(string templateRootPath, ILogger<RazorTemplateRenderer> logger)
    {
        _logger = logger;

        // Resolve absolute path to the templates folder
        var absolutePath = Path.GetFullPath(templateRootPath);

        if (!Directory.Exists(absolutePath))
        {
            throw new DirectoryNotFoundException(
                $"Template directory not found: {absolutePath}");
        }

        // Build metadata references from currently loaded assemblies (excluding dynamic/in-memory ones).
        // This provides RazorLight the compilation references it needs at runtime.
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToArray();

        // Create an array of MetadataReference (not List<PortableExecutableReference>).
        var references = assemblies
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToArray();

        // Choose a sensible "operating" assembly for Razor compilation (entry or executing assembly).
        var operatingAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(absolutePath)
            .UseMemoryCachingProvider()
            .SetOperatingAssembly(operatingAssembly)
            .AddMetadataReferences(references)
            .EnableDebugMode()          // Better error messages during development
            .Build();

        _logger.LogInformation("RazorLight engine initialized. Template root: {Path}", absolutePath);
    }

    public async Task<string> RenderAsync<TModel>(string templateName, TModel model) where TModel : class
    {
        var templateFile = $"{templateName}.cshtml";
        _logger.LogDebug("Rendering template: {Template} with model type: {ModelType}",
            templateFile, typeof(TModel).Name);

        try
        {
            var html = await _engine.CompileRenderAsync(templateFile, model);
            _logger.LogDebug("Template rendered successfully. Output length: {Length} chars", html.Length);
            return html;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render template: {Template}", templateFile);
            throw;
        }
    }
}