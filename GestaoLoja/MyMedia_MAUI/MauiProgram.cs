using Microsoft.Extensions.Logging;
using MyMedia_MAUI.Services;
using RCLGeral.Services;

namespace MyMedia_MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Configurar URL base da API
        // Android Emulator usa 10.0.2.2 para aceder ao localhost do host
        // Windows/iOS usam localhost diretamente
        string apiBaseUrl;
        
#if ANDROID
        apiBaseUrl = "https://10.0.2.2:7000";
#else
        apiBaseUrl = "https://localhost:7000";
#endif

        // Registar HttpClient configurado
        builder.Services.AddScoped(sp => 
        {
            var handler = new HttpClientHandler();
            // Em desenvolvimento, aceitar certificados self-signed
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            
            return new HttpClient(handler)
            {
                BaseAddress = new Uri(apiBaseUrl)
            };
        });

        // Registar serviços - Token Storage específico para MAUI
        builder.Services.AddSingleton<ITokenStorageService, MauiTokenStorageService>();

        // Registar serviços do RCLGeral
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IProdutoService, ProdutoService>();
        builder.Services.AddScoped<ICategoriaService, CategoriaService>();
        builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();
        builder.Services.AddScoped<IEncomendaService, EncomendaService>();
        builder.Services.AddScoped<IFornecedorService, FornecedorService>();

        return builder.Build();
    }
}
