using MyMedia_Web.Components;
using RCLGeral.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configurar URL base da API
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7196";

// Registar HttpClient para comunicação com a API
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(apiBaseUrl) 
});

// Registar serviços da RCL
builder.Services.AddScoped<ITokenStorageService, WebTokenStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IEncomendaService, EncomendaService>();
builder.Services.AddScoped<IFornecedorService, FornecedorService>();
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();

// Blazor Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(RCLGeral._Imports).Assembly);

app.Run();
