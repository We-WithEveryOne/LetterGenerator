using LetterGenerator.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LetterGenerator.Engine.PdfGeneration;

/// <summary>
/// Generates PDFs by rendering HTML in a headless Chromium browser via Playwright.
/// 
/// Why Playwright?
/// - Microsoft-maintained, MIT license (enterprise-friendly)
/// - Pixel-perfect rendering — same engine as Chrome
/// - Supports @media print CSS, web fonts, SVG, base64 images
/// - Headers/footers with page numbers
/// </summary>
public class PlaywrightPdfGenerator : IPdfGenerator, IAsyncDisposable
{
    private readonly ILogger<PlaywrightPdfGenerator> _logger;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public PlaywrightPdfGenerator(ILogger<PlaywrightPdfGenerator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Lazy-initialize Playwright and launch the browser once.
    /// The browser instance is reused across requests for performance.
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;

            _logger.LogInformation("Initializing Playwright and launching Chromium...");
            _playwright = await Playwright.CreateAsync();

            try
            {
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true
                });
            }
            catch (PlaywrightException ex) when (ex.Message?.Contains("Executable doesn't exist", StringComparison.OrdinalIgnoreCase) == true
                                                 || ex.Message?.Contains("Please run", StringComparison.OrdinalIgnoreCase) == true)
            {
                _logger.LogWarning(ex, "Playwright browser executable not found. Attempting to run Playwright install script from published output.");

                var baseDir = AppContext.BaseDirectory ?? Environment.CurrentDirectory;
                var scriptPath = Path.Combine(baseDir, "playwright.ps1");

                if (File.Exists(scriptPath))
                {
                    _logger.LogInformation("Found playwright install script at {ScriptPath}. Running installer...", scriptPath);
                    var installed = await RunPlaywrightInstallScriptAsync(scriptPath);
                    if (!installed)
                    {
                        _logger.LogError("Failed to run playwright install script. Please run it manually in the application output folder: pwsh playwright.ps1 install");
                        throw;
                    }

                    _logger.LogInformation("Playwright install script completed. Retrying browser launch...");
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = true
                    });
                }
                else
                {
                    _logger.LogError("playwright.ps1 not found at {ScriptPath}. Please run 'playwright install' manually in your project output folder.", scriptPath);
                    throw;
                }
            }

            _initialized = true;
            _logger.LogInformation("Playwright Chromium browser launched successfully.");
        }
        finally
        {
            _initLock.Release();
        }
    }

    private static async Task<bool> RunPlaywrightInstallScriptAsync(string scriptPath)
    {
        // Try pwsh first, then powershell
        string[] candidates = { "pwsh", "powershell" };

        foreach (var exe in candidates)
        {
            try
            {
                var args = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" install";

                var psi = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc == null) continue;

                var outputTask = proc.StandardOutput.ReadToEndAsync();
                var errorTask = proc.StandardError.ReadToEndAsync();

                await Task.WhenAll(outputTask, errorTask);
                var output = outputTask.Result;
                var error = errorTask.Result;

                proc.WaitForExit();

                // Helpful diagnostic output (console used — logger not available in static helper)
                if (!string.IsNullOrWhiteSpace(output))
                    Console.WriteLine(output);
                if (!string.IsNullOrWhiteSpace(error))
                    Console.Error.WriteLine(error);

                if (proc.ExitCode == 0)
                    return true;
            }
            catch
            {
                // ignore and try next candidate
            }
        }

        return false;
    }

    public async Task<byte[]> GeneratePdfAsync(string htmlContent, PdfOptions? options = null)
    {
        await EnsureInitializedAsync();

        options ??= new PdfOptions();

        _logger.LogDebug("Generating PDF. Format: {Format}, Landscape: {Landscape}",
            options.Format, options.Landscape);

        // Create a new page (isolated context) for each PDF
        var page = await _browser!.NewPageAsync();
        try
        {
            // Load the HTML content
            await page.SetContentAsync(htmlContent, new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Wait for any images to fully render
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Generate PDF
            var pdfBytes = await page.PdfAsync(new PagePdfOptions
            {
                Format = options.Format,
                Landscape = options.Landscape,
                PrintBackground = options.PrintBackground,
                Margin = new()
                {
                    Top = options.MarginTop,
                    Bottom = options.MarginBottom,
                    Left = options.MarginLeft,
                    Right = options.MarginRight
                },
                DisplayHeaderFooter = options.DisplayHeaderFooter,
                HeaderTemplate = options.HeaderTemplate ?? "<span></span>",
                FooterTemplate = options.FooterTemplate ?? "<span></span>"
            });

            _logger.LogInformation("PDF generated successfully. Size: {Size} bytes", pdfBytes.Length);
            return pdfBytes;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;
        _initialized = false;

        GC.SuppressFinalize(this);
    }
}