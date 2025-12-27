using RCLGeral.API;
using RCLGeral.Services;
using RCLGeral.API.Services;
using RCLGeral.Services.Cart; 
using MyMEDIA_Blazor.Components;
using MyMEDIA_Blazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using RCLGeral.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the RCLGeral project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<IApiServices, ApiService>();
builder.Services.AddScoped<AuthStateService>();

builder.Services.AddScoped<CartService>();

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
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(RCLGeral._Imports).Assembly);

app.Run();
