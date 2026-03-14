using BlazorApp.Components;
using BlazorApp.Repositories.Implementations;
using BlazorApp.Repositories.Interfaces;
using BlazorApp.Services.Implementations;
using BlazorApp.Services.Interfaces;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddSingleton<IFormDefinitionService, FormDefinitionService>();
builder.Services.AddSingleton<IFormValidationService, FormValidationService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
// These two are better as Scoped (per-request)
builder.Services.AddScoped<IFormSubmissionService, FormSubmissionService>();
builder.Services.AddScoped<IFormSubmissionRepository, FormSubmissionCosmosRepository>();

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

builder.Services.AddSingleton<IFormSubmissionRepository, FormSubmissionCosmosRepository>();

var app = builder.Build();

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
