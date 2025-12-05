using Azure.Storage.Blobs;
using DocumentSearchPrototype.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Azure Blob Service Client
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetValue<string>("AzureStorage:ConnectionString")));

// Add HttpClient for Azure AI Search
builder.Services.AddHttpClient("AzureSearchClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AzureSearch:Endpoint"] ?? throw new InvalidOperationException("Azure Search Endpoint is not configured."));
    client.DefaultRequestHeaders.Add("api-key", builder.Configuration["AzureSearch:ApiKey"] ?? throw new InvalidOperationException("Azure Search ApiKey is not configured."));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
