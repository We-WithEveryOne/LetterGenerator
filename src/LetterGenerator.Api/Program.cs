using LetterGenerator.Engine;

var builder = WebApplication.CreateBuilder(args);

// ── Register Letter Generation Services ──────────────────────────────
var templatePath = Path.Combine(builder.Environment.ContentRootPath, "Templates");
builder.Services.AddLetterGenerator(templatePath);

// ── Standard API Services ────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Healthcare Letter Generator API",
        Version = "v1",
        Description = "Generates healthcare authorization letters (Denial, Approval, etc.) " +
                      "using Razor templates rendered to PDF via Playwright."
    });
});

// ── CORS (for preview UI if needed) ──────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPreview", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ── Middleware Pipeline ──────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowPreview");
app.UseAuthorization();
app.MapControllers();

app.Run();
