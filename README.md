# Healthcare Letter Generator

A modern .NET 9 Web API that replaces legacy Java/HTML letter generation pipelines with **Razor templates + Playwright PDF rendering**.



## Solution Structure

```
LetterGenerator.sln
├── LetterGenerator.Core           # Models, interfaces, DTOs (zero dependencies)
│   ├── Models/
│   │   ├── LetterBase.cs          # Common fields: member, provider, auth, org
│   │   ├── DenialLetterModel.cs   # Denial-specific: rationale, appeal rights
│   │   └── ApprovalLetterModel.cs # Approval-specific: conditions, limitations
│   ├── Interfaces/
│   │   └── ILetterServices.cs     # IRazorTemplateRenderer, IPdfGenerator, ILetterService
│   └── Dtos/
│       └── LetterDtos.cs          # Request/response DTOs, batch support
│
├── LetterGenerator.Engine         # Rendering pipeline
│   ├── Rendering/
│   │   └── RazorTemplateRenderer.cs   # RazorLight-based .cshtml renderer
│   ├── PdfGeneration/
│   │   └── PlaywrightPdfGenerator.cs  # Headless Chromium → PDF
│   ├── Services/
│   │   └── LetterService.cs           # Orchestrator: Razor → HTML → PDF
│   └── ServiceCollectionExtensions.cs # DI registration
│
└── LetterGenerator.Api            # ASP.NET Core Web API
    ├── Controllers/
    │   └── LettersController.cs   # REST endpoints for generate/preview
    ├── Templates/                 # Razor letter templates
    │   ├── DenialLetter.cshtml
    │   └── ApprovalLetter.cshtml
    ├── SampleDataFactory.cs       # Realistic test data
    └── Program.cs                 # Startup & DI wiring
```

## Prerequisites

- .NET 9 SDK
- Node.js (required by Playwright for browser download)

## Setup

```bash
# 1. Clone and restore
cd LetterGenerator
dotnet restore

# 2. Install Playwright browsers (one-time)
cd src/LetterGenerator.Api
dotnet build
pwsh bin/Debug/net9.0/playwright.ps1 install chromium

# On Linux/Mac without PowerShell:
# npx playwright install chromium

# 3. Run the API
dotnet run --project src/LetterGenerator.Api
```

The API launches at `https://localhost:7100` with Swagger UI.

## API Endpoints

### Denial Letters
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/letters/denial/pdf` | Generate denial PDF from JSON body |
| `POST` | `/api/letters/denial/preview` | Preview denial as HTML |
| `GET`  | `/api/letters/denial/sample` | Generate PDF with sample data |
| `GET`  | `/api/letters/denial/sample/preview` | Preview HTML with sample data |

### Approval Letters
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/letters/approval/pdf` | Generate approval PDF from JSON body |
| `POST` | `/api/letters/approval/preview` | Preview approval as HTML |
| `GET`  | `/api/letters/approval/sample` | Generate PDF with sample data |
| `GET`  | `/api/letters/approval/sample/preview` | Preview HTML with sample data |

### Utility
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET`  | `/api/letters/templates` | List available templates |

## Quick Test

After starting the API, open your browser:

```
https://localhost:7100/api/letters/denial/sample/preview
```

This renders a sample denial letter as HTML — no POST body needed.

## Adding a New Letter Type

1. **Create the model** in `LetterGenerator.Core/Models/`:
   ```csharp
   public class PeerToPeerLetterModel : LetterBase
   {
       public string ScheduledDateTime { get; set; }
       public string ReviewPhysicianName { get; set; }
       // ...
   }
   ```

2. **Create the template** in `LetterGenerator.Api/Templates/PeerToPeerLetter.cshtml`:
   ```html
   @model PeerToPeerLetterModel
   <!-- Your HTML/CSS layout here with @Model.PropertyName bindings -->
   ```

3. **Add controller endpoint** in `LettersController.cs`:
   ```csharp
   [HttpPost("peer-to-peer/pdf")]
   public async Task<IActionResult> GeneratePeerToPeerPdf([FromBody] PeerToPeerLetterModel model)
   {
       var pdf = await _letterService.GenerateLetterPdfAsync("PeerToPeerLetter", model);
       return File(pdf, "application/pdf", $"P2P_{model.LetterReferenceId}.pdf");
   }
   ```

That's it — no DB changes, no driver tables, no JAR rebuilds.

## Adding Logo Images

Pass a base64-encoded image in the `LogoBase64` field:

```csharp
model.LogoBase64 = Convert.ToBase64String(File.ReadAllBytes("logo.png"));
model.LogoContentType = "image/png";
```

The template renders it inline: `<img src="data:image/png;base64,..." />`

## Batch Job Integration

Your existing batch job becomes simple:

```csharp
// In your Hangfire/hosted service:
var letterService = serviceProvider.GetRequiredService<ILetterService>();

foreach (var authDecision in pendingDecisions)
{
    var model = MapToLetterModel(authDecision);  // Your existing data mapping
    var pdf = await letterService.GenerateLetterPdfAsync("DenialLetter", model);
    await SaveToStorage(pdf, model.LetterReferenceId);
}
```

## Key Design Decisions

- **RazorLight** (not ASP.NET MVC Razor) — works standalone without MVC pipeline overhead, MIT licensed
- **Playwright** (not Puppeteer) — Microsoft-maintained, MIT license, easier enterprise approval
- **Templates as files** (not DB) — version controlled, diffable, reviewable in PRs
- **Base64 images** — no file path dependencies, works in any environment
- **Singleton browser** — Playwright browser instance reused across requests for performance
- **Strongly-typed models** — compile-time safety, IntelliSense, no magic strings

## License & Dependencies

| Dependency | License | Notes |
|---|---|---|
| ASP.NET Core | MIT | Microsoft |
| RazorLight | MIT | Standalone Razor engine |
| Microsoft.Playwright | MIT | Microsoft |
| Swashbuckle | MIT | Swagger/OpenAPI |
