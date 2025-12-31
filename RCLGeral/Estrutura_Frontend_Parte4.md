# Estrutura Detalhada do Frontend MyCOLL - Parte 4

## Índice desta Parte
12. [Sistema de Estilos CSS](#sistema-de-estilos-css)
13. [Tratamento de Erros](#tratamento-de-erros)
14. [Configuração CORS na API](#configuracao-cors-na-api)
15. [Ciclo de Vida dos Componentes Blazor](#ciclo-de-vida-dos-componentes)
16. [Boas Práticas Implementadas](#boas-praticas-implementadas)

---

## Sistema de Estilos CSS

### Arquitetura de CSS no Projeto

```
+------------------------------------------------------------------+
|                    HIERARQUIA DE CSS                              |
+------------------------------------------------------------------+
|                                                                   |
|   RCLGeral/wwwroot/css/                                        |
|   +---------------------------+                                   |
|   |   app.css                 |  <- Estilos GLOBAIS              |
|   |   - CSS Variables         |     (cores, tipografia, layout)  |
|   |   - Reset/Base            |                                   |
|   |   - Components (buttons,  |                                   |
|   |     forms, alerts, etc.)  |                                   |
|   +---------------------------+                                   |
|              |                                                    |
|              v                                                    |
|   +---------------------------+                                   |
|   |   pages.css               |  <- Estilos de PÁGINAS           |
|   |   - Home page             |     (específicos por página)     |
|   |   - Produto detalhe       |                                   |
|   |   - Carrinho              |                                   |
|   |   - Auth pages            |                                   |
|   |   - Fornecedor pages      |                                   |
|   +---------------------------+                                   |
|                                                                   |
|   Como são carregados:                                           |
|   +---------------------------+                                   |
|   | MyMedia_Web/App.razor      |                                   |
|   | <link href="_content/     |                                   |
|   |   RCLGeral/css/app.css"/>                                  |
|   | <link href="_content/     |                                   |
|   |   RCLGeral/css/pages.css"/>                                |
|   +---------------------------+                                   |
|                                                                   |
|   MyCOLL_MAUI/wwwroot/index.html                                 |
|   +---------------------------+                                   |
|   | <link href="_content/     |  <- Mesmo caminho!               |
|   |   RCLGeral/css/..."/>   |     RCL expõe wwwroot            |
|   +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### CSS Variables (Design Tokens)

O ficheiro `app.css` começa com **CSS Variables** que definem todo o sistema visual:

```css
:root {
    /* ========================================
       CORES PRINCIPAIS
       ======================================== */
    --primary-color: #6366f1;      /* Roxo/Índigo - cor principal */
    --primary-hover: #4f46e5;      /* Cor ao fazer hover */
    --primary-light: #e0e7ff;      /* Versão clara para backgrounds */
    
    --secondary-color: #64748b;    /* Cinza - cor secundária */
    
    /* Cores semânticas */
    --success-color: #10b981;      /* Verde - sucesso */
    --warning-color: #f59e0b;      /* Amarelo - aviso */
    --danger-color: #ef4444;       /* Vermelho - erro/perigo */
    --info-color: #3b82f6;         /* Azul - informação */
    
    /* ========================================
       CORES DE FUNDO E TEXTO
       ======================================== */
    --bg-color: #f8fafc;           /* Fundo da página */
    --bg-card: #ffffff;            /* Fundo de cards */
    --bg-dark: #1e293b;            /* Fundo escuro (footer) */
    
    --text-primary: #1e293b;       /* Texto principal */
    --text-secondary: #64748b;     /* Texto secundário */
    --text-muted: #94a3b8;         /* Texto desativado */
    --text-light: #ffffff;         /* Texto em fundos escuros */
    
    /* ========================================
       BORDAS E SOMBRAS
       ======================================== */
    --border-color: #e2e8f0;
    --border-radius: 8px;          /* Cantos arredondados padrão */
    --border-radius-lg: 12px;      /* Cantos maiores para cards */
    
    --shadow-sm: 0 1px 2px rgba(0, 0, 0, 0.05);
    --shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
    
    /* ========================================
       ESPAÇAMENTO (Sistema 4px)
       ======================================== */
    --spacing-xs: 4px;
    --spacing-sm: 8px;
    --spacing-md: 16px;
    --spacing-lg: 24px;
    --spacing-xl: 32px;
    
    /* ========================================
       TRANSIÇÕES
       ======================================== */
    --transition: all 0.2s ease;
}
```

**Porque usar CSS Variables?**

1. **Consistência** - Mesmas cores em todo o projeto
2. **Manutenção** - Mudar uma cor muda em todo o lado
3. **Tema** - Fácil criar modo escuro no futuro
4. **Legibilidade** - `var(--primary-color)` é mais claro que `#6366f1`

### Componentes CSS Reutilizáveis

#### Botões
```css
/* Botão Principal */
.btn-primary {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: var(--spacing-sm);
    padding: var(--spacing-sm) var(--spacing-lg);
    background: var(--primary-color);
    color: white;
    border: none;
    border-radius: var(--border-radius);
    font-weight: 500;
    cursor: pointer;
    transition: var(--transition);
}

.btn-primary:hover {
    background: var(--primary-hover);
}

.btn-primary:disabled {
    background: var(--text-muted);
    cursor: not-allowed;
}

/* Variantes */
.btn-secondary { /* ... */ }
.btn-danger { /* ... */ }
.btn-full { width: 100%; }
.btn-large { padding: var(--spacing-md) var(--spacing-xl); }
```

#### Formulários
```css
.form-group {
    margin-bottom: var(--spacing-md);
}

.form-group label {
    display: block;
    margin-bottom: var(--spacing-xs);
    font-weight: 500;
}

.form-control {
    width: 100%;
    padding: var(--spacing-sm) var(--spacing-md);
    border: 1px solid var(--border-color);
    border-radius: var(--border-radius);
    transition: var(--transition);
}

/* Estado de foco - feedback visual importante! */
.form-control:focus {
    outline: none;
    border-color: var(--primary-color);
    box-shadow: 0 0 0 3px var(--primary-light);  /* Ring effect */
}
```

#### Alertas
```css
.alert {
    padding: var(--spacing-md);
    border-radius: var(--border-radius);
    margin-bottom: var(--spacing-md);
}

.alert-error {
    background: #fef2f2;
    color: #991b1b;
    border: 1px solid #fecaca;
}

.alert-success {
    background: #ecfdf5;
    color: #065f46;
    border: 1px solid #a7f3d0;
}
```

#### Loading States
```css
.loading {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 64px;
}

/* Spinner animado */
.spinner {
    width: 40px;
    height: 40px;
    border: 3px solid var(--border-color);
    border-top-color: var(--primary-color);
    border-radius: 50%;
    animation: spin 1s linear infinite;
}

@keyframes spin {
    to { transform: rotate(360deg); }
}
```

### Estilos de Páginas Específicas

#### Home Page - Product Grid
```css
.produtos-grid {
    display: grid;
    /* Auto-fill: cria colunas automaticamente */
    /* minmax: mínimo 250px, máximo 1fr */
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: var(--spacing-lg);
}

.produto-card {
    background: var(--bg-color);
    border-radius: var(--border-radius);
    overflow: hidden;
    transition: var(--transition);
    border: 1px solid var(--border-color);
}

/* Efeito hover - feedback visual */
.produto-card:hover {
    transform: translateY(-4px);
    box-shadow: var(--shadow-md);
}
```

#### Categoria Slider (Horizontal Scroll)
```css
.categoria-slider {
    display: flex;
    gap: var(--spacing-md);
    overflow-x: auto;  /* Scroll horizontal */
    padding-bottom: var(--spacing-md);
    
    /* Scrollbar customizada */
    scrollbar-width: thin;
    scrollbar-color: var(--border-color) transparent;
}

/* Webkit (Chrome, Safari) */
.categoria-slider::-webkit-scrollbar {
    height: 6px;
}

.categoria-slider::-webkit-scrollbar-thumb {
    background: var(--border-color);
    border-radius: 3px;
}
```

### Design Responsivo

```css
/* Tablet e abaixo */
@media (max-width: 992px) {
    .hero-content {
        grid-template-columns: 1fr;  /* Stack vertical */
    }
    
    .produto-detalhe {
        grid-template-columns: 1fr;
    }
    
    .carrinho-content {
        grid-template-columns: 1fr;
    }
}

/* Mobile */
@media (max-width: 768px) {
    .header-content {
        flex-wrap: wrap;
    }
    
    .search-box {
        order: 3;  /* Mover para baixo */
        width: 100%;
    }
    
    .form-row {
        grid-template-columns: 1fr;  /* Uma coluna */
    }
    
    .main-content {
        padding: var(--spacing-md);
    }
}
```

---

## Tratamento de Erros

### Estratégia de Tratamento de Erros

```
+------------------------------------------------------------------+
|                    NÍVEIS DE TRATAMENTO                           |
+------------------------------------------------------------------+
|                                                                   |
|   1. SERVIÇO (try/catch)                                         |
|   +---------------------------+                                   |
|   | ProdutoService.cs         |                                   |
|   | - Captura exceções HTTP   |                                   |
|   | - Retorna valores padrão  |                                   |
|   | - Nunca lança exceções    |                                   |
|   +---------------------------+                                   |
|              |                                                    |
|              v                                                    |
|   2. COMPONENTE (UI feedback)                                    |
|   +---------------------------+                                   |
|   | Home.razor                |                                   |
|   | - Verifica retorno        |                                   |
|   | - Mostra mensagem erro    |                                   |
|   | - Estado de loading       |                                   |
|   +---------------------------+                                   |
|              |                                                    |
|              v                                                    |
|   3. GLOBAL (ErrorBoundary)                                      |
|   +---------------------------+                                   |
|   | App.razor / Routes.razor  |                                   |
|   | - Captura erros não       |                                   |
|   |   tratados                |                                   |
|   | - Mostra página de erro   |                                   |
|   +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### 1. Tratamento nos Serviços

```csharp
// RCLGeral/Services/ProdutoService.cs
public class ProdutoService : IProdutoService
{
    private readonly HttpClient _httpClient;

    public async Task<List<ProdutoModel>> GetProdutosAsync(...)
    {
        try
        {
            var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoModel>>(url);
            return produtos ?? new List<ProdutoModel>();
        }
        catch (HttpRequestException ex)
        {
            // Erro de rede (sem internet, servidor indisponível)
            Console.WriteLine($"Erro de rede: {ex.Message}");
            return new List<ProdutoModel>();
        }
        catch (TaskCanceledException ex)
        {
            // Timeout
            Console.WriteLine($"Timeout: {ex.Message}");
            return new List<ProdutoModel>();
        }
        catch (JsonException ex)
        {
            // Resposta inválida da API
            Console.WriteLine($"Erro JSON: {ex.Message}");
            return new List<ProdutoModel>();
        }
        catch (Exception ex)
        {
            // Qualquer outro erro
            Console.WriteLine($"Erro inesperado: {ex.Message}");
            return new List<ProdutoModel>();
        }
    }

    public async Task<ProdutoModel?> GetProdutoAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProdutoModel>($"api/produtos/{id}");
        }
        catch
        {
            return null;  // Retorna null em caso de erro
        }
    }
}
```

### 2. Tratamento com Resultado Estruturado

```csharp
// RCLGeral/Services/EncomendaService.cs
public class EncomendaService : IEncomendaService
{
    // Retorno estruturado: (Success, Data, Error)
    public async Task<(bool Success, EncomendaModel? Encomenda, string? Error)> 
        CriarEncomendaAsync(CriarEncomendaModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/encomendas", model);
            
            if (response.IsSuccessStatusCode)
            {
                var encomenda = await response.Content.ReadFromJsonAsync<EncomendaModel>();
                return (true, encomenda, null);
            }
            
            // Erro da API (400, 401, 404, 500, etc.)
            var errorContent = await response.Content.ReadAsStringAsync();
            
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest 
                    => (false, null, "Dados inválidos. Verifique o carrinho."),
                System.Net.HttpStatusCode.Unauthorized 
                    => (false, null, "Sessão expirada. Faça login novamente."),
                System.Net.HttpStatusCode.Forbidden 
                    => (false, null, "Não tem permissão para esta operação."),
                System.Net.HttpStatusCode.NotFound 
                    => (false, null, "Produto não encontrado ou indisponível."),
                _ => (false, null, $"Erro do servidor: {response.StatusCode}")
            };
        }
        catch (HttpRequestException)
        {
            return (false, null, "Sem ligação ao servidor. Verifique a sua internet.");
        }
        catch (TaskCanceledException)
        {
            return (false, null, "O pedido demorou demasiado. Tente novamente.");
        }
        catch (Exception ex)
        {
            return (false, null, $"Erro inesperado: {ex.Message}");
        }
    }
}
```

### 3. Tratamento nos Componentes

```razor
@* RCLGeral/Components/Pages/Carrinho.razor *@

@if (!string.IsNullOrEmpty(errorMessage))
{
    <div class="alert alert-error">
        <span>??</span>
        <span>@errorMessage</span>
        <button class="alert-close" @onclick="() => errorMessage = null">×</button>
    </div>
}

@code {
    private string? errorMessage;
    private bool isProcessing;

    private async Task FinalizarCompra()
    {
        isProcessing = true;
        errorMessage = null;
        StateHasChanged();

        try
        {
            var model = new CriarEncomendaModel { /* ... */ };
            
            // Usar resultado estruturado
            var (success, encomenda, error) = await EncomendaService.CriarEncomendaAsync(model);

            if (success)
            {
                CarrinhoService.Limpar();
                Navigation.NavigateTo($"/encomenda/{encomenda!.Id}?nova=true");
            }
            else
            {
                errorMessage = error;
            }
        }
        catch (Exception ex)
        {
            // Último recurso - erro não previsto
            errorMessage = "Ocorreu um erro inesperado. Tente novamente.";
            Console.WriteLine($"Erro não tratado: {ex}");
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }
}
```

### 4. Estados de Loading e Empty

```razor
@* Padrão comum em todas as páginas *@

@if (isLoading)
{
    <div class="loading">
        <div class="spinner"></div>
        <p>A carregar...</p>
    </div>
}
else if (errorMessage != null)
{
    <div class="error-state">
        <span class="empty-icon">?</span>
        <h2>Ocorreu um erro</h2>
        <p>@errorMessage</p>
        <button class="btn-primary" @onclick="CarregarDados">
            Tentar Novamente
        </button>
    </div>
}
else if (!produtos.Any())
{
    <div class="empty-state">
        <span class="empty-icon">??</span>
        <h2>Nenhum produto encontrado</h2>
        <p>Tente ajustar os filtros de pesquisa.</p>
    </div>
}
else
{
    @* Conteúdo normal *@
    <div class="produtos-grid">
        @foreach (var produto in produtos)
        {
            @* ... *@
        }
    </div>
}
```

### 5. AuthService - Tratamento Especial

```csharp
// RCLGeral/Services/AuthService.cs
public async Task<AuthResponse> LoginAsync(LoginModel model)
{
    try
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            
            if (result?.Success == true && result.Token != null)
            {
                await _tokenStorage.SetTokenAsync(result.Token);
                await _tokenStorage.SetUserAsync(result.User!);
                _cachedToken = result.Token;
                _cachedUser = result.User;
                ConfigureHttpClient(result.Token);
                OnAuthStateChanged?.Invoke();
            }
            
            return result ?? new AuthResponse { Success = false, Message = "Resposta inválida" };
        }
        
        // Tratar códigos de erro específicos
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return new AuthResponse 
            { 
                Success = false, 
                Message = "Email ou password incorretos" 
            };
        }
        
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            // Tentar ler mensagem da API (conta pendente, inativa, etc.)
            try
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return errorResponse ?? new AuthResponse 
                { 
                    Success = false, 
                    Message = "Acesso negado" 
                };
            }
            catch
            {
                return new AuthResponse { Success = false, Message = "Acesso negado" };
            }
        }
        
        return new AuthResponse 
        { 
            Success = false, 
            Message = $"Erro do servidor ({(int)response.StatusCode})" 
        };
    }
    catch (HttpRequestException)
    {
        return new AuthResponse 
        { 
            Success = false, 
            Message = "Sem ligação ao servidor. Verifique a sua internet." 
        };
    }
    catch (Exception ex)
    {
        return new AuthResponse 
        { 
            Success = false, 
            Message = $"Erro: {ex.Message}" 
        };
    }
}
```

---

## Configuração CORS na API

### O que é CORS?

**CORS (Cross-Origin Resource Sharing)** é um mecanismo de segurança do browser:

```
+------------------------------------------------------------------+
|                    PROBLEMA SEM CORS                              |
+------------------------------------------------------------------+
|                                                                   |
|   Browser (MyMedia_Web)                                           |
|   URL: https://localhost:5001                                    |
|                                                                   |
|   JavaScript faz pedido para:                                    |
|   https://localhost:7196/api/produtos                            |
|                                                                   |
|   ? BLOQUEADO!                                                   |
|   "Different origin" (porta diferente = origem diferente)        |
|                                                                   |
+------------------------------------------------------------------+
|                                                                   |
|   SOLUÇÃO: Configurar CORS na API                                |
|                                                                   |
|   A API diz ao browser:                                          |
|   "Aceito pedidos de https://localhost:5001"                     |
|                                                                   |
|   ? PERMITIDO!                                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### Configuração no REST/Program.cs

```csharp
// REST/Program.cs

var builder = WebApplication.CreateBuilder(args);

// ... outras configurações ...

// =============================================
// CONFIGURAR CORS
// =============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            // Origens permitidas
            .AllowAnyOrigin()           // Em produção, especificar URLs exatas!
            // .WithOrigins(
            //     "https://localhost:5001",  // MyMedia_Web
            //     "https://mycoll.pt"        // Produção
            // )
            
            // Métodos HTTP permitidos
            .AllowAnyMethod()           // GET, POST, PUT, DELETE, etc.
            // .WithMethods("GET", "POST", "PUT", "DELETE")
            
            // Headers permitidos
            .AllowAnyHeader()           // Authorization, Content-Type, etc.
            // .WithHeaders("Authorization", "Content-Type")
            
            // Headers expostos na resposta
            .WithExposedHeaders(
                "X-Total-Count",        // Para paginação
                "X-Page",
                "X-Per-Page"
            );
    });
});

var app = builder.Build();

// ... outras configurações ...

// =============================================
// USAR CORS (ANTES de Authentication!)
// =============================================
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Porque AllowAnyOrigin em Desenvolvimento?

```csharp
// Em DESENVOLVIMENTO:
policy.AllowAnyOrigin()  // Mais fácil para testar

// Em PRODUÇÃO (mais seguro):
policy.WithOrigins(
    "https://mycoll.pt",
    "https://www.mycoll.pt",
    "https://app.mycoll.pt"
)
```

**Riscos de AllowAnyOrigin em produção:**
- Qualquer site pode fazer pedidos à tua API
- Vulnerável a ataques CSRF
- Dados podem ser roubados

### Headers Expostos

```csharp
.WithExposedHeaders("X-Total-Count", "X-Page", "X-Per-Page")
```

Por padrão, o browser só permite acesso a alguns headers. Se a API enviar headers customizados (como para paginação), é preciso "expô-los".

### Ordem do Middleware

A ordem **importa**! CORS deve vir **antes** de Authentication:

```csharp
// CORRETO:
app.UseCors("AllowFrontend");    // 1º
app.UseAuthentication();          // 2º
app.UseAuthorization();           // 3º

// ERRADO (pode não funcionar):
app.UseAuthentication();
app.UseCors("AllowFrontend");    // Tarde demais!
app.UseAuthorization();
```

---

## Ciclo de Vida dos Componentes Blazor

### Métodos do Ciclo de Vida

```
+------------------------------------------------------------------+
|              CICLO DE VIDA DE UM COMPONENTE                       |
+------------------------------------------------------------------+
|                                                                   |
|   1. CRIAÇÃO                                                     |
|   +---------------------------+                                   |
|   | SetParametersAsync        |  <- Parâmetros são definidos     |
|   +---------------------------+                                   |
|              |                                                    |
|              v                                                    |
|   +---------------------------+                                   |
|   | OnInitialized             |  <- Inicialização síncrona       |
|   | OnInitializedAsync        |  <- Inicialização assíncrona     |
|   +---------------------------+     (carregar dados da API)      |
|              |                                                    |
|              v                                                    |
|   2. RENDERIZAÇÃO                                                |
|   +---------------------------+                                   |
|   | OnParametersSet           |  <- Após parâmetros mudarem      |
|   | OnParametersSetAsync      |                                   |
|   +---------------------------+                                   |
|              |                                                    |
|              v                                                    |
|   +---------------------------+                                   |
|   | BuildRenderTree           |  <- Constrói o HTML              |
|   | (automático)              |                                   |
|   +---------------------------+                                   |
|              |                                                    |
|              v                                                    |
|   +---------------------------+                                   |
|   | OnAfterRender             |  <- Após renderizar              |
|   | OnAfterRenderAsync        |     (JS interop aqui!)           |
|   +---------------------------+                                   |
|              |                                                    |
|              v                                                    |
|   3. ATUALIZAÇÕES (loop)                                         |
|   +---------------------------+                                   |
|   | StateHasChanged()         |  <- Força re-render              |
|   +---------------------------+                                   |
|              |                                                    |
|              v                                                    |
|   4. DESTRUIÇÃO                                                  |
|   +---------------------------+                                   |
|   | Dispose                   |  <- Limpar recursos              |
|   | (se IDisposable)          |     (event handlers, etc.)       |
|   +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### Exemplo Prático: MainLayout.razor

```razor
@inherits LayoutComponentBase
@implements IDisposable  @* Importante! Permite limpar recursos *@

@inject IAuthService AuthService
@inject ICarrinhoService CarrinhoService

<div class="page">
    @* ... UI ... *@
</div>

@code {
    private bool isAuthenticated;
    private UserInfo? userInfo;

    // =============================================
    // INICIALIZAÇÃO - Corre UMA vez ao criar
    // =============================================
    protected override async Task OnInitializedAsync()
    {
        // Carregar estado inicial
        isAuthenticated = await AuthService.IsAuthenticatedAsync();
        if (isAuthenticated)
        {
            userInfo = await AuthService.GetUserInfoAsync();
        }

        // SUBSCREVER a eventos
        // Quando auth ou carrinho mudar, atualizar UI
        AuthService.OnAuthStateChanged += OnAuthStateChanged;
        CarrinhoService.OnCarrinhoChanged += OnCarrinhoChanged;
    }

    // =============================================
    // HANDLERS DE EVENTOS
    // =============================================
    private async void OnAuthStateChanged()
    {
        // async void é necessário para event handlers
        isAuthenticated = await AuthService.IsAuthenticatedAsync();
        userInfo = isAuthenticated ? await AuthService.GetUserInfoAsync() : null;
        
        // InvokeAsync garante que corre na thread correta
        await InvokeAsync(StateHasChanged);
    }

    private void OnCarrinhoChanged()
    {
        // Sem await porque não é async
        InvokeAsync(StateHasChanged);
    }

    // =============================================
    // DESTRUIÇÃO - Limpar subscrições!
    // =============================================
    public void Dispose()
    {
        // MUITO IMPORTANTE! Evita memory leaks
        AuthService.OnAuthStateChanged -= OnAuthStateChanged;
        CarrinhoService.OnCarrinhoChanged -= OnCarrinhoChanged;
    }
}
```

### Quando usar cada método?

| Método | Quando usar |
|--------|-------------|
| `OnInitializedAsync` | Carregar dados da API, inicialização |
| `OnParametersSetAsync` | Reagir a mudanças de parâmetros (ex: `@page "/produto/{Id}"`) |
| `OnAfterRenderAsync` | JavaScript Interop, focus em elementos |
| `Dispose` | Limpar event handlers, cancelar timers |

### Exemplo: Página com Parâmetro

```razor
@page "/produto/{Id:int}"

@code {
    [Parameter]
    public int Id { get; set; }

    private ProdutoModel? produto;
    private int previousId;

    // Corre quando Id muda (navegação entre produtos)
    protected override async Task OnParametersSetAsync()
    {
        // Só recarregar se Id mudou
        if (Id != previousId)
        {
            previousId = Id;
            await CarregarProduto();
        }
    }

    private async Task CarregarProduto()
    {
        produto = await ProdutoService.GetProdutoAsync(Id);
    }
}
```

---

## Boas Práticas Implementadas

### 1. Dependency Injection

```csharp
// CERTO: Injetar interfaces, não implementações
@inject IAuthService AuthService
@inject IProdutoService ProdutoService

// ERRADO: Criar instâncias diretamente
var service = new AuthService(new HttpClient());  // Não fazer isto!
```

### 2. Padrão de Serviços com Interfaces

```csharp
// Interface define o contrato
public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginModel model);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    event Action? OnAuthStateChanged;
}

// Implementação pode variar
public class AuthService : IAuthService
{
    // Implementação real
}

public class MockAuthService : IAuthService
{
    // Para testes
}
```

### 3. Async/Await Corretamente

```csharp
// CERTO: Usar async/await para operações I/O
protected override async Task OnInitializedAsync()
{
    produtos = await ProdutoService.GetProdutosAsync();
}

// ERRADO: Bloquear com .Result ou .Wait()
protected override void OnInitialized()
{
    produtos = ProdutoService.GetProdutosAsync().Result;  // Pode causar deadlock!
}
```

### 4. StateHasChanged com Cuidado

```csharp
// CERTO: Usar InvokeAsync em event handlers
private async void OnSomeEvent()
{
    await InvokeAsync(StateHasChanged);
}

// ERRADO: Chamar StateHasChanged diretamente em qualquer thread
private void OnSomeEvent()
{
    StateHasChanged();  // Pode falhar se não estiver na thread correta
}
```

### 5. Dispose para Limpar Recursos

```csharp
// SEMPRE limpar subscrições de eventos
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        SomeService.OnChanged += HandleChanged;
    }

    public void Dispose()
    {
        SomeService.OnChanged -= HandleChanged;  // Evita memory leak!
    }
}
```

### 6. Validação no Cliente E no Servidor

```razor
@* Cliente: Blazor validation *@
<EditForm Model="model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />
    
    <InputText @bind-Value="model.Nome" />
    <ValidationMessage For="() => model.Nome" />
</EditForm>
```

```csharp
// Servidor: API também valida
[HttpPost]
public async Task<ActionResult> CriarProduto([FromBody] CriarProdutoDTO dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    // ... resto
}
```

### 7. Loading States Sempre

```razor
@* Sempre mostrar estado de carregamento *@
<button @onclick="Submeter" disabled="@isLoading">
    @if (isLoading)
    {
        <span class="spinner-small"></span>
        <text>A processar...</text>
    }
    else
    {
        <text>Submeter</text>
    }
</button>
```

---

## Resumo das Secções Adicionadas

| Secção | Conteúdo |
|--------|----------|
| **CSS** | CSS Variables, componentes reutilizáveis, responsivo |
| **Erros** | Try/catch, resultados estruturados, UI feedback |
| **CORS** | Configuração, segurança, ordem do middleware |
| **Ciclo de Vida** | Métodos, quando usar, exemplos práticos |
| **Boas Práticas** | DI, async/await, Dispose, validação |

---

*Documentação criada em: Janeiro 2025*
*Projeto: MyCOLL - Plataforma de Colecionáveis*
*Parte 4 de 4 - Conteúdo Complementar*
