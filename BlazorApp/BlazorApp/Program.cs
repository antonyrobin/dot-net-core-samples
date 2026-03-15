using BlazorApp.Components;
using BlazorApp.Repositories.Implementations;
using BlazorApp.Repositories.Interfaces;
using BlazorApp.Services.Implementations;
using BlazorApp.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();  // Add memory cache for reducing Cosmos hits
builder.Services.AddSingleton<IFormDefinitionService, FormDefinitionService>();
builder.Services.AddSingleton<IFormValidationService, FormValidationService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
// These two are better as Scoped (per-request)
builder.Services.AddScoped<IFormSubmissionService, FormSubmissionService>();

// === COSMOS DB REGISTRATION (updated) ===
var cosmosConnectionString = builder.Configuration["Cosmos:ConnectionString"]
    ?? builder.Configuration.GetConnectionString("Cosmos");

if (string.IsNullOrWhiteSpace(cosmosConnectionString))
{
    throw new InvalidOperationException(
        "Cosmos DB connection string is missing! " +
        "Add it to appsettings.json or User Secrets under 'Cosmos:ConnectionString'");
}

builder.Services.AddSingleton<CosmosClient>(sp =>
    new CosmosClient(cosmosConnectionString));

// Register the base repository
builder.Services.AddSingleton<IFormSubmissionRepository>(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    var baseRepository = new FormSubmissionCosmosRepository(cosmosClient);

    // Wrap with caching decorator for better performance
    var memoryCache = sp.GetRequiredService<IMemoryCache>();
    return new CachedFormSubmissionRepository(baseRepository, memoryCache);
});

var app = builder.Build();

// Warm-up Cosmos DB SDK and repository initialization on startup so the first user request
// doesn't bear the SDK connection / gateway / JIT cost. This blocks startup briefly but
// avoids a long delay (5-10s) on the first page navigation that needs the repository.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var cosmos = scope.ServiceProvider.GetRequiredService<CosmosClient>();
        // Warm-up the SDK connection
        cosmos.GetDatabase("dynamicsdb").GetContainer("dynamics_submissions").ReadContainerAsync().GetAwaiter().GetResult();

        // Also eagerly initialize the repository to populate the partition key path
        // This ensures the first query doesn't pay the overhead
        var repository = scope.ServiceProvider.GetRequiredService<IFormSubmissionRepository>();
        _ = repository.GetPagedAsync(null, 1, null).GetAwaiter().GetResult();
    }
    catch
    {
        // Ignore warm-up failures here; real calls will surface errors as usual.
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
