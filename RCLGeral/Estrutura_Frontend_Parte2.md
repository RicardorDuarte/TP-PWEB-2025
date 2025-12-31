# Estrutura Detalhada do Frontend MyCOLL - Parte 2

## Índice desta Parte
5. [Estrutura de Pastas Completa](#estrutura-de-pastas-completa)
6. [Serviços e Comunicação com API](#servicos-e-comunicacao-com-api)
7. [Sistema de Autenticação JWT](#sistema-de-autenticacao-jwt)

---

## Estrutura de Pastas Completa

### RCLGeral (Biblioteca Partilhada)

```
RCLGeral/
?
??? RCLGeral.csproj              <- Ficheiro do projeto
?                                     Define que é uma RCL
?
??? _Imports.razor                 <- Imports globais
?                                     Evita repetir @using em cada ficheiro
?
??? Components/                    <- COMPONENTES BLAZOR
?   ?
?   ??? Layout/                    <- LAYOUT PARTILHADO
?   ?   ??? MainLayout.razor       <- Layout principal
?   ?                                 Header + Menu + Footer
?   ?                                 Usado por Web E MAUI!
?   ?
?   ??? Pages/                     <- PÁGINAS DA APLICAÇÃO
?       ?
?       ??? Home.razor             <- Página inicial (/)
?       ?                             Produto destaque + Categorias + Grid
?       ?
?       ??? Login.razor            <- Página de login (/login)
?       ?                             Formulário + Validação
?       ?
?       ??? Registar.razor         <- Registo (/registar)
?       ?                             Cliente ou Fornecedor
?       ?
?       ??? Carrinho.razor         <- Carrinho (/carrinho)
?       ?                             Lista + Checkout
?       ?
?       ??? ProdutoDetalhe.razor   <- Detalhe produto (/produto/{id})
?       ?                             Imagem + Info + Comprar
?       ?
?       ??? MinhasEncomendas.razor <- Histórico (/minhas-encomendas)
?       ?                             Lista de encomendas do cliente
?       ?
?       ??? EncomendaDetalhe.razor <- Detalhe encomenda (/encomenda/{id})
?       ?                             Estado + Itens + Ações
?       ?
?       ??? Fornecedor/            <- PÁGINAS DO FORNECEDOR
?           ?
?           ??? MeusProdutos.razor     <- Lista produtos (/meus-produtos)
?           ??? CriarProduto.razor     <- Criar (/meus-produtos/criar)
?           ??? EditarProduto.razor    <- Editar (/meus-produtos/editar/{id})
?           ??? MinhasVendas.razor     <- Vendas (/minhas-vendas)
?
??? Services/                      <- SERVIÇOS (LÓGICA)
?   ?
?   ??? ITokenStorageService.cs    <- Interface para guardar tokens
?   ?                                 Implementação diferente em Web vs MAUI
?   ?
?   ??? AuthService.cs             <- Autenticação
?   ?                                 Login, Logout, Estado
?   ?
?   ??? ProdutoService.cs          <- Produtos
?   ?                                 Listar, Filtrar, Detalhe
?   ?
?   ??? CategoriaService.cs        <- Categorias
?   ?                                 Hierarquia, Navegação
?   ?
?   ??? CarrinhoService.cs         <- Carrinho (local)
?   ?                                 Adicionar, Remover, Total
?   ?
?   ??? EncomendaService.cs        <- Encomendas
?   ?                                 Criar, Histórico, Pagar
?   ?
?   ??? FornecedorService.cs       <- Fornecedor
?                                     CRUD produtos próprios
?
??? Models/                        <- MODELOS DE DADOS
?   ?
?   ??? AuthModels.cs              <- LoginModel, RegistoModel, UserInfo
?   ??? ProdutoModels.cs           <- ProdutoModel, CriarProdutoModel
?   ??? CategoriaModels.cs         <- CategoriaModel
?   ??? EncomendaModels.cs         <- EncomendaModel, ItemModel
?   ??? CarrinhoModels.cs          <- CarrinhoItem, Carrinho
?
??? wwwroot/                       <- FICHEIROS ESTÁTICOS
    ??? css/
        ??? app.css                <- Estilos globais
        ??? pages.css              <- Estilos das páginas
```

### MyMedia_Web (Frontend Browser)

```
MyMedia_Web/
?
??? MyMedia_Web.csproj              <- Ficheiro do projeto
?                                     Referencia RCLGeral
?
??? Program.cs                     <- PONTO DE ENTRADA
?                                     Configura serviços e HttpClient
?
??? appsettings.json               <- Configurações
?                                     URL da API
?
??? Components/                    <- COMPONENTES ESPECÍFICOS WEB
?   ?
?   ??? App.razor                  <- Componente raiz
?   ?                                 HTML base, CSS imports
?   ?
?   ??? Routes.razor               <- Configuração de rotas
?   ?                                 Inclui assemblies da RCL
?   ?
?   ??? WebTokenStorageService.cs  <- Guardar token no localStorage
?   ?                                 Usa JavaScript Interop
?   ?
?   ??? CustomAuthStateProvider.cs <- Estado de autenticação
?                                     Lê token e cria ClaimsPrincipal
?
??? wwwroot/                       <- FICHEIROS ESTÁTICOS WEB
    ??? favicon.png
    ??? app.css                    <- CSS específico Web (se houver)
```

### MyCOLL_MAUI (App Nativa)

```
MyCOLL_MAUI/
?
??? MyCOLL_MAUI.csproj             <- Ficheiro do projeto
?                                     Multi-plataforma (Android, iOS, Windows)
?
??? MauiProgram.cs                 <- PONTO DE ENTRADA MAUI
?                                     Configura serviços e HttpClient
?                                     URL da API (ou Dev Tunnel)
?
??? App.xaml                       <- Definição da App MAUI
??? App.xaml.cs                    <- Código da App
?
??? MainPage.xaml                  <- Página principal MAUI
?                                     Contém o BlazorWebView
??? MainPage.xaml.cs
?
??? Components/                    <- COMPONENTES ESPECÍFICOS MAUI
?   ?
?   ??? Routes.razor               <- Rotas (igual ao Web)
?   ??? _Imports.razor             <- Imports MAUI
?
??? MauiTokenStorageService.cs     <- Guardar token no SecureStorage
?                                     API nativa do MAUI
?
??? MauiAuthStateProvider.cs       <- Estado de autenticação
?                                     Similar ao Web
?
??? Platforms/                     <- CÓDIGO ESPECÍFICO POR PLATAFORMA
?   ??? Android/
?   ?   ??? AndroidManifest.xml
?   ??? iOS/
?   ?   ??? Info.plist
?   ??? Windows/
?       ??? Package.appxmanifest
?
??? Resources/                     <- RECURSOS DA APP
?   ??? AppIcon/                   <- Ícone da app
?   ??? Splash/                    <- Splash screen
?   ??? Fonts/                     <- Fontes
?   ??? Images/                    <- Imagens
?
??? wwwroot/                       <- FICHEIROS ESTÁTICOS MAUI
    ??? index.html                 <- HTML base para Blazor
    ??? css/
        ??? app.css
```

---

## Serviços e Comunicação com API

### Arquitetura de Comunicação

```
+------------------------------------------------------------------+
|                    FLUXO DE COMUNICAÇÃO                           |
+------------------------------------------------------------------+
|                                                                   |
|   MyMedia_Web / MyCOLL_MAUI                                       |
|   +---------------------------+                                   |
|   |    Página Blazor          |                                   |
|   |    (Home.razor)           |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 | @inject IProdutoService                         |
|                 v                                                 |
|   +---------------------------+                                   |
|   |    ProdutoService.cs      |                                   |
|   |    (da RCL)               |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 | HttpClient.GetAsync("api/produtos")             |
|                 v                                                 |
|   +---------------------------+                                   |
|   |    HttpClient             |                                   |
|   |    (configurado no        |                                   |
|   |     Program.cs)           |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 | HTTPS Request                                   |
|                 | Authorization: Bearer {token}                   |
|                 v                                                 |
+------------------------------------------------------------------+
|                                                                   |
|   REST API (Projeto REST)                                        |
|   +---------------------------+                                   |
|   |    ProdutosController     |                                   |
|   |    [Route("api/produtos")]|                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 | Entity Framework                                |
|                 v                                                 |
|   +---------------------------+                                   |
|   |    ApplicationDbContext   |                                   |
|   |    (do GestaoLoja)        |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 | SQL                                             |
|                 v                                                 |
|   +---------------------------+                                   |
|   |    SQL Server             |                                   |
|   +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### Configuração do HttpClient

**MyMedia_Web/Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// URL da API vem do appsettings.json
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7196";

// Registar HttpClient com a URL base
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
```

**MyCOLL_MAUI/MauiProgram.cs:**
```csharp
// URL da API - IMPORTANTE para Dev Tunnels!
private const string API_BASE_URL = "https://localhost:7196";
// Para testar em dispositivo real, usar Dev Tunnel:
// private const string API_BASE_URL = "https://seu-tunnel.devtunnels.ms";

builder.Services.AddScoped(sp => 
{
    var handler = new HttpClientHandler();
    
    // Em desenvolvimento, ignorar erros SSL
    #if DEBUG
    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    #endif
    
    return new HttpClient(handler)
    {
        BaseAddress = new Uri(API_BASE_URL)
    };
});

// Mesmos serviços, implementação diferente do token storage
builder.Services.AddScoped<ITokenStorageService, MauiTokenStorageService>();
// ... resto igual ao Web
```

### Interface ITokenStorageService

Esta interface permite que o **mesmo código** funcione em **plataformas diferentes**:

```csharp
// RCLGeral/Services/ITokenStorageService.cs
public interface ITokenStorageService
{
    Task<string?> GetTokenAsync();      // Obter token guardado
    Task SetTokenAsync(string token);   // Guardar token
    Task RemoveTokenAsync();            // Apagar token (logout)
    Task<UserInfo?> GetUserAsync();     // Obter info do utilizador
    Task SetUserAsync(UserInfo user);   // Guardar info
    Task RemoveUserAsync();             // Apagar info
}
```

### Implementação Web (localStorage)

```csharp
// MyMedia_Web/Components/WebTokenStorageService.cs
public class WebTokenStorageService : ITokenStorageService
{
    private readonly IJSRuntime _jsRuntime;  // JavaScript Interop!
    
    // Guardar no localStorage do browser
    public async Task SetTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "mycoll_auth_token", token);
    }
    
    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "mycoll_auth_token");
    }
    
    public async Task RemoveTokenAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "mycoll_auth_token");
    }
}
```

### Implementação MAUI (SecureStorage)

```csharp
// MyCOLL_MAUI/MauiTokenStorageService.cs
public class MauiTokenStorageService : ITokenStorageService
{
    // SecureStorage é uma API nativa do MAUI!
    // - Android: usa KeyStore
    // - iOS: usa Keychain
    // - Windows: usa Credential Manager
    
    public async Task SetTokenAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync("mycoll_auth_token", token);
        }
        catch
        {
            // Fallback para Preferences (menos seguro)
            Preferences.Set("mycoll_auth_token", token);
        }
    }
    
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync("mycoll_auth_token");
        }
        catch
        {
            return Preferences.Get("mycoll_auth_token", string.Empty);
        }
    }
}
```

### Exemplo Completo: ProdutoService

```csharp
// RCLGeral/Services/ProdutoService.cs
public class ProdutoService : IProdutoService
{
    private readonly HttpClient _httpClient;

    public ProdutoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Listar produtos com filtros
    public async Task<List<ProdutoModel>> GetProdutosAsync(
        int? categoriaId = null, 
        decimal? precoMin = null, 
        decimal? precoMax = null, 
        string? pesquisa = null,
        int pagina = 1, 
        int porPagina = 12)
    {
        try
        {
            // Construir query string
            var queryParams = new List<string>();
            
            if (categoriaId.HasValue)
                queryParams.Add($"categoriaId={categoriaId}");
            if (precoMin.HasValue)
                queryParams.Add($"precoMin={precoMin}");
            if (precoMax.HasValue)
                queryParams.Add($"precoMax={precoMax}");
            if (!string.IsNullOrEmpty(pesquisa))
                queryParams.Add($"pesquisa={Uri.EscapeDataString(pesquisa)}");
            
            queryParams.Add($"pagina={pagina}");
            queryParams.Add($"porPagina={porPagina}");

            var url = "api/produtos";
            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            // Fazer pedido HTTP
            var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoModel>>(url);
            return produtos ?? new List<ProdutoModel>();
        }
        catch (Exception)
        {
            return new List<ProdutoModel>();  // Retorna lista vazia em caso de erro
        }
    }

    // Obter produto por ID
    public async Task<ProdutoModel?> GetProdutoAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProdutoModel>($"api/produtos/{id}");
        }
        catch (Exception)
        {
            return null;
        }
    }
}
```

---

## Sistema de Autenticação JWT

### O que é JWT?

**JWT (JSON Web Token)** é um padrão para autenticação:

```
+------------------------------------------------------------------+
|                    ESTRUTURA DE UM JWT                            |
+------------------------------------------------------------------+
|                                                                   |
|  eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.                           |
|  eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4ifQ.                 |
|  SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c                     |
|                                                                   |
|  +------------------+                                             |
|  |     HEADER       |  <- Algoritmo e tipo                        |
|  +------------------+                                             |
|  | {                |                                             |
|  |   "alg": "HS256",|                                             |
|  |   "typ": "JWT"   |                                             |
|  | }                |                                             |
|  +------------------+                                             |
|           .                                                       |
|  +------------------+                                             |
|  |     PAYLOAD      |  <- Dados do utilizador (claims)            |
|  +------------------+                                             |
|  | {                |                                             |
|  |   "sub": "123",  |  <- ID do utilizador                        |
|  |   "email": "...",|                                             |
|  |   "role": "...", |  <- Role (Cliente/Fornecedor)               |
|  |   "exp": 123...  |  <- Data de expiração                       |
|  | }                |                                             |
|  +------------------+                                             |
|           .                                                       |
|  +------------------+                                             |
|  |    SIGNATURE     |  <- Assinatura (verifica autenticidade)     |
|  +------------------+                                             |
|                                                                   |
+------------------------------------------------------------------+
```

### Fluxo de Autenticação

```
+------------------------------------------------------------------+
|                    FLUXO DE LOGIN                                 |
+------------------------------------------------------------------+
|                                                                   |
|   1. UTILIZADOR FAZ LOGIN                                        |
|   +---------------------------+                                   |
|   |    Login.razor            |                                   |
|   |    - Email: xxx@xxx.com   |                                   |
|   |    - Password: ********   |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 | AuthService.LoginAsync(model)                   |
|                 v                                                 |
|   2. ENVIAR CREDENCIAIS PARA API                                 |
|   +---------------------------+                                   |
|   | POST /api/auth/login      |                                   |
|   | {                         |                                   |
|   |   "email": "xxx@xxx.com", |                                   |
|   |   "password": "********"  |                                   |
|   | }                         |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 v                                                 |
|   3. API VALIDA E GERA TOKEN                                     |
|   +---------------------------+                                   |
|   | AuthController.cs         |                                   |
|   | - Verificar credenciais   |                                   |
|   | - Verificar estado conta  |                                   |
|   | - Gerar JWT token         |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 v                                                 |
|   4. RESPOSTA COM TOKEN                                          |
|   +---------------------------+                                   |
|   | {                         |                                   |
|   |   "success": true,        |                                   |
|   |   "token": "eyJhbG...",   |                                   |
|   |   "user": {               |                                   |
|   |     "id": "...",          |                                   |
|   |     "nome": "João",       |                                   |
|   |     "role": "Cliente"     |                                   |
|   |   }                       |                                   |
|   | }                         |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 v                                                 |
|   5. GUARDAR TOKEN LOCALMENTE                                    |
|   +---------------------------+                                   |
|   | ITokenStorageService      |                                   |
|   | - Web: localStorage       |                                   |
|   | - MAUI: SecureStorage     |                                   |
|   +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### AuthService Explicado

```csharp
// RCLGeral/Services/AuthService.cs
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenStorage;
    private string? _cachedToken;      // Cache em memória
    private UserInfo? _cachedUser;

    // Evento para notificar mudanças de estado
    public event Action? OnAuthStateChanged;

    public async Task<AuthResponse> LoginAsync(LoginModel model)
    {
        try
        {
            // 1. Enviar credenciais para API
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                
                if (result?.Success == true && result.Token != null)
                {
                    // 2. Guardar token localmente
                    await _tokenStorage.SetTokenAsync(result.Token);
                    await _tokenStorage.SetUserAsync(result.User!);
                    
                    // 3. Cache em memória
                    _cachedToken = result.Token;
                    _cachedUser = result.User;
                    
                    // 4. Configurar HttpClient para usar token
                    ConfigureHttpClient(result.Token);
                    
                    // 5. Notificar que estado mudou
                    OnAuthStateChanged?.Invoke();
                }
                return result ?? new AuthResponse { Success = false };
            }
            
            return new AuthResponse 
            { 
                Success = false, 
                Message = "Email ou password incorretos" 
            };
        }
        catch (Exception ex)
        {
            return new AuthResponse { Success = false, Message = $"Erro: {ex.Message}" };
        }
    }

    public async Task LogoutAsync()
    {
        // Limpar tudo
        await _tokenStorage.RemoveTokenAsync();
        await _tokenStorage.RemoveUserAsync();
        _cachedToken = null;
        _cachedUser = null;
        
        // Remover header de autorização
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        // Notificar que estado mudou
        OnAuthStateChanged?.Invoke();
    }

    private void ConfigureHttpClient(string token)
    {
        // Adicionar token a todos os pedidos futuros
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }
}
```

### AuthStateProvider - Integração com Blazor

```csharp
// CustomAuthStateProvider.cs (Web) / MauiAuthStateProvider.cs (MAUI)
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;

    public CustomAuthStateProvider(IAuthService authService)
    {
        _authService = authService;
        
        // Quando auth muda, notifica Blazor
        _authService.OnAuthStateChanged += NotifyAuthStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _authService.GetUserInfoAsync();
        
        if (user == null)
        {
            // Não autenticado - retorna utilizador anónimo
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Criar claims a partir do UserInfo
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.NomeCompleto),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)  // IMPORTANTE para [Authorize(Roles = "...")]
        };

        var identity = new ClaimsIdentity(claims, "jwt");
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);
    }

    private void NotifyAuthStateChanged()
    {
        // Força Blazor a re-verificar autenticação
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
```

### Como Usar Autenticação nas Páginas

```razor
@* Página protegida - só Clientes podem aceder *@
@page "/minhas-encomendas"
@attribute [Authorize(Roles = "Cliente")]
@inject IEncomendaService EncomendaService

@* Se não autenticado, Routes.razor redireciona para Login *@

<h1>Minhas Encomendas</h1>

@* Usar AuthorizeView para mostrar/esconder conteúdo *@
<AuthorizeView Roles="Cliente">
    <Authorized>
        @* Só aparece se for Cliente *@
        <button>Fazer Nova Encomenda</button>
    </Authorized>
</AuthorizeView>

@code {
    protected override async Task OnInitializedAsync()
    {
        // Este código só corre se passar pelo [Authorize]
        var encomendas = await EncomendaService.GetHistoricoAsync();
    }
}
```

### API REST - Validar Token

```csharp
// REST/Controllers/EncomendasController.cs
[Route("api/[controller]")]
[ApiController]
public class EncomendasController : ControllerBase
{
    [HttpGet("historico")]
    [Authorize(Roles = "Cliente")]  // REQUER token válido com role Cliente
    public async Task<ActionResult<List<EncomendaDTO>>> GetHistoricoCliente()
    {
        // Obter ID do utilizador a partir do token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Não autenticado" });

        // Só retorna encomendas DESTE utilizador
        var encomendas = await context.Encomendas
            .Where(e => e.ClienteId == userId)
            .ToListAsync();

        return Ok(encomendas);
    }
}
```

### Configuração JWT na API

```csharp
// REST/Program.cs
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];  // Chave secreta (32+ chars)
var issuer = jwtSettings["Issuer"];        // Quem emitiu (MyCOLL_API)
var audience = jwtSettings["Audience"];    // Para quem (MyCOLL_Frontend)

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,           // Verificar emissor
        ValidateAudience = true,         // Verificar audiência
        ValidateLifetime = true,         // Verificar expiração
        ValidateIssuerSigningKey = true, // Verificar assinatura
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero,       // Sem tolerância de tempo
        RoleClaimType = ClaimTypes.Role  // Onde está a role no token
    };
});
```

---

*Continua na Parte 3...*
