# Estrutura Detalhada do Frontend MyCOLL - Parte 3

## Índice desta Parte
8. [Componentes e Páginas](#componentes-e-paginas)
9. [Dev Tunnels - Testar em Dispositivos Reais](#dev-tunnels)
10. [Diagrama de Fluxos](#diagrama-de-fluxos)
11. [Resumo para Defesa](#resumo-para-defesa)

---

## Componentes e Páginas

### MainLayout.razor - O Layout Partilhado

Este é o layout usado tanto no Web como no MAUI:

```razor
@* RCLGeral/Components/Layout/MainLayout.razor *@
@inherits LayoutComponentBase
@inject IAuthService AuthService
@inject ICarrinhoService CarrinhoService
@inject NavigationManager Navigation

<div class="page">
    <!-- HEADER -->
    <header class="top-header">
        <div class="header-content">
            <!-- Logo -->
            <a href="" class="brand">
                <span class="brand-text">MyCOLL</span>
            </a>
            
            <!-- Pesquisa -->
            <div class="search-box">
                <input type="text" placeholder="Pesquisar..." @bind="pesquisa" />
                <button @onclick="Pesquisar">??</button>
            </div>
            
            <!-- Navegação -->
            <nav class="header-nav">
                <!-- Carrinho com badge -->
                <a href="carrinho" class="nav-item">
                    ??
                    @if (CarrinhoService.GetQuantidadeTotal() > 0)
                    {
                        <span class="badge">@CarrinhoService.GetQuantidadeTotal()</span>
                    }
                </a>
                
                <!-- Menu baseado em autenticação -->
                @if (isAuthenticated)
                {
                    <div class="user-menu">
                        <span>@userInfo?.NomeCompleto</span>
                        <div class="dropdown-menu">
                            <!-- Opções diferentes por role -->
                            @if (userInfo?.IsCliente == true)
                            {
                                <a href="minhas-encomendas">?? Minhas Encomendas</a>
                            }
                            @if (userInfo?.IsFornecedor == true)
                            {
                                <a href="meus-produtos">?? Meus Produtos</a>
                                <a href="minhas-vendas">?? Minhas Vendas</a>
                            }
                            <button @onclick="Logout">?? Sair</button>
                        </div>
                    </div>
                }
                else
                {
                    <a href="login">Entrar</a>
                    <a href="registar">Registar</a>
                }
            </nav>
        </div>
    </header>

    <!-- CONTEÚDO PRINCIPAL -->
    <main class="main-content">
        @Body  <!-- Aqui aparece o conteúdo da página atual -->
    </main>

    <!-- FOOTER -->
    <footer class="footer">
        <p>&copy; @DateTime.Now.Year MyCOLL</p>
    </footer>
</div>

@code {
    private bool isAuthenticated;
    private UserInfo? userInfo;
    private string pesquisa = "";

    protected override async Task OnInitializedAsync()
    {
        // Verificar estado inicial
        isAuthenticated = await AuthService.IsAuthenticatedAsync();
        if (isAuthenticated)
        {
            userInfo = await AuthService.GetUserInfoAsync();
        }

        // Subscrever a eventos para atualizar UI
        AuthService.OnAuthStateChanged += OnAuthStateChanged;
        CarrinhoService.OnCarrinhoChanged += OnCarrinhoChanged;
    }

    private async void OnAuthStateChanged()
    {
        // Atualizar quando login/logout
        isAuthenticated = await AuthService.IsAuthenticatedAsync();
        userInfo = isAuthenticated ? await AuthService.GetUserInfoAsync() : null;
        await InvokeAsync(StateHasChanged);  // Forçar re-render
    }

    private void OnCarrinhoChanged()
    {
        // Atualizar badge do carrinho
        InvokeAsync(StateHasChanged);
    }

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        Navigation.NavigateTo("/");
    }

    public void Dispose()
    {
        // Limpar subscrições (evitar memory leaks)
        AuthService.OnAuthStateChanged -= OnAuthStateChanged;
        CarrinhoService.OnCarrinhoChanged -= OnCarrinhoChanged;
    }
}
```

### Home.razor - Página Principal

```razor
@page "/"
@inject IProdutoService ProdutoService
@inject ICategoriaService CategoriaService
@inject ICarrinhoService CarrinhoService

<div class="home-page">
    <!-- PRODUTO EM DESTAQUE -->
    @if (produtoDestaque != null)
    {
        <section class="hero-section">
            <img src="@produtoDestaque.ImagemUrl" />
            <h1>@produtoDestaque.Nome</h1>
            <span class="price">@produtoDestaque.PrecoFinal.ToString("C")</span>
            <button @onclick="() => AdicionarAoCarrinho(produtoDestaque)">
                Adicionar ao Carrinho
            </button>
        </section>
    }

    <!-- NAVEGAÇÃO POR CATEGORIAS (3 níveis) -->
    <section class="categorias-section">
        <h2>Explorar por Categoria</h2>
        
        <!-- Nível 1: Tipos (Moedas, Selos, etc.) -->
        <div class="categoria-slider">
            @foreach (var cat in categoriasRaiz)
            {
                <div class="categoria-card @(IsSelected(cat, 1) ? "selected" : "")" 
                     @onclick="() => SelecionarNivel1(cat)">
                    <img src="@cat.ImagemUrl" />
                    <span>@cat.Nome</span>
                </div>
            }
        </div>

        <!-- Nível 2: Países (aparecem após selecionar Nível 1) -->
        @if (subcategoriasNivel2.Any())
        {
            <div class="categoria-slider nivel-2">
                @foreach (var cat in subcategoriasNivel2)
                {
                    <div class="categoria-card" @onclick="() => SelecionarNivel2(cat)">
                        <span>@cat.Nome</span>
                    </div>
                }
            </div>
        }

        <!-- Nível 3: Temas -->
        @if (subcategoriasNivel3.Any())
        {
            <div class="categoria-slider nivel-3">
                <!-- ... similar ... -->
            </div>
        }
    </section>

    <!-- GRID DE PRODUTOS -->
    <section class="produtos-section">
        @if (isLoading)
        {
            <div class="spinner"></div>
        }
        else
        {
            <div class="produtos-grid">
                @foreach (var produto in produtos)
                {
                    <div class="produto-card">
                        <img src="@produto.ImagemUrl" />
                        <h3>@produto.Nome</h3>
                        <span>@produto.PrecoFinal.ToString("C")</span>
                        @if (produto.PodeComprar)
                        {
                            <button @onclick="() => AdicionarAoCarrinho(produto)">??</button>
                        }
                    </div>
                }
            </div>
        }
    </section>
</div>

@code {
    private ProdutoModel? produtoDestaque;
    private List<CategoriaModel> categoriasRaiz = new();
    private List<CategoriaModel> subcategoriasNivel2 = new();
    private List<CategoriaModel> subcategoriasNivel3 = new();
    private List<ProdutoModel> produtos = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        // Carregar tudo em paralelo (melhor performance!)
        await Task.WhenAll(
            CarregarDestaque(),
            CarregarCategorias(),
            CarregarProdutos()
        );
        isLoading = false;
    }

    private async Task SelecionarNivel1(CategoriaModel categoria)
    {
        // Carregar subcategorias
        subcategoriasNivel2 = await CategoriaService.GetSubcategoriasAsync(categoria.Id);
        subcategoriasNivel3 = new();  // Limpar nível 3
        
        // Carregar produtos desta categoria
        await CarregarProdutos(categoria.Id);
    }

    private void AdicionarAoCarrinho(ProdutoModel produto)
    {
        CarrinhoService.AdicionarProduto(produto);
        // UI atualiza automaticamente via evento OnCarrinhoChanged
    }
}
```

### CarrinhoService - Gestão Local do Carrinho

```csharp
// O carrinho é gerido LOCALMENTE (não precisa de API)
public class CarrinhoService : ICarrinhoService
{
    public Carrinho Carrinho { get; } = new();
    
    // Evento para notificar componentes
    public event Action? OnCarrinhoChanged;

    public void AdicionarProduto(ProdutoModel produto, int quantidade = 1)
    {
        // Verificar se já existe
        var existente = Carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produto.Id);
        
        if (existente != null)
        {
            // Aumentar quantidade
            existente.Quantidade += quantidade;
        }
        else
        {
            // Adicionar novo item
            Carrinho.Itens.Add(new CarrinhoItem
            {
                ProdutoId = produto.Id,
                ProdutoNome = produto.Nome,
                ProdutoImagem = produto.ImagemUrl,
                PrecoUnitario = produto.PrecoFinal,
                Quantidade = quantidade,
                StockDisponivel = produto.Stock
            });
        }
        
        // Notificar que mudou
        OnCarrinhoChanged?.Invoke();
    }

    public void Limpar()
    {
        Carrinho.Itens.Clear();
        OnCarrinhoChanged?.Invoke();
    }
}
```

### Carrinho.razor - Finalizar Compra

```razor
@page "/carrinho"
@inject ICarrinhoService CarrinhoService
@inject IAuthService AuthService
@inject IEncomendaService EncomendaService

<h1>Carrinho de Compras</h1>

@if (CarrinhoService.Carrinho.EstaVazio)
{
    <p>O seu carrinho está vazio.</p>
    <a href="/">Ver Produtos</a>
}
else
{
    <!-- Lista de Itens -->
    @foreach (var item in CarrinhoService.Carrinho.Itens)
    {
        <div class="carrinho-item">
            <img src="@item.ProdutoImagem" />
            <span>@item.ProdutoNome</span>
            <span>@item.PrecoUnitario.ToString("C")</span>
            
            <!-- Controles de quantidade -->
            <button @onclick="() => Diminuir(item)">-</button>
            <span>@item.Quantidade</span>
            <button @onclick="() => Aumentar(item)">+</button>
            
            <span>Subtotal: @item.Subtotal.ToString("C")</span>
            <button @onclick="() => Remover(item)">???</button>
        </div>
    }

    <!-- Resumo -->
    <div class="resumo">
        <p>Total: @CarrinhoService.GetTotal().ToString("C")</p>
        
        @if (isAuthenticated)
        {
            <select @bind="metodoPagamento">
                <option value="MBWay">MBWay</option>
                <option value="Multibanco">Multibanco</option>
                <option value="PayPal">PayPal</option>
            </select>
            
            <button @onclick="FinalizarCompra" disabled="@isProcessing">
                @(isProcessing ? "A processar..." : "Finalizar Compra")
            </button>
        }
        else
        {
            <p>Faça login para finalizar a compra.</p>
            <a href="/login?returnUrl=/carrinho">Entrar</a>
        }
    </div>
}

@code {
    private bool isAuthenticated;
    private bool isProcessing;
    private string metodoPagamento = "MBWay";

    protected override async Task OnInitializedAsync()
    {
        isAuthenticated = await AuthService.IsAuthenticatedAsync();
    }

    private async Task FinalizarCompra()
    {
        isProcessing = true;

        // Criar modelo de encomenda
        var model = new CriarEncomendaModel
        {
            Itens = CarrinhoService.Carrinho.Itens.Select(i => new ItemCarrinhoModel
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList(),
            MetodoPagamento = metodoPagamento
        };

        // Enviar para API
        var (success, encomenda, error) = await EncomendaService.CriarEncomendaAsync(model);

        if (success)
        {
            CarrinhoService.Limpar();  // Limpar carrinho
            Navigation.NavigateTo($"/encomenda/{encomenda!.Id}?nova=true");
        }
        else
        {
            errorMessage = error;
        }

        isProcessing = false;
    }
}
```

---

## Dev Tunnels - Testar em Dispositivos Reais

### O Problema

Quando desenvolves localmente:
- A API corre em `https://localhost:7196`
- O browser no PC consegue aceder
- **MAS** um telemóvel Android **NÃO** consegue aceder a `localhost`!

```
+------------------------------------------------------------------+
|                    O PROBLEMA                                     |
+------------------------------------------------------------------+
|                                                                   |
|   PC de Desenvolvimento                                          |
|   +---------------------------+                                   |
|   |  API: localhost:7196      |                                   |
|   +---------------------------+                                   |
|              |                                                    |
|              | ? Funciona                                        |
|              v                                                    |
|   +---------------------------+                                   |
|   |  Browser no PC            |                                   |
|   +---------------------------+                                   |
|                                                                   |
|   Telemóvel Android                                              |
|   +---------------------------+                                   |
|   |  App MAUI                 |                                   |
|   |  -> localhost:7196        |   ? "localhost" é o TELEMÓVEL!   |
|   +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### A Solução: Dev Tunnels

O **Dev Tunnel** cria um URL público temporário que redireciona para o teu localhost:

```
+------------------------------------------------------------------+
|                    A SOLUÇÃO                                      |
+------------------------------------------------------------------+
|                                                                   |
|   PC de Desenvolvimento                                          |
|   +---------------------------+                                   |
|   |  API: localhost:7196      |                                   |
|   +-------------+-------------+                                   |
|                 |                                                 |
|                 | Dev Tunnel                                      |
|                 v                                                 |
|   +---------------------------+                                   |
|   |  URL Público:             |                                   |
|   |  https://abc123.devtunnels.ms                                |
|   +-------------+-------------+                                   |
|                 |                                                 |
|        INTERNET (qualquer dispositivo)                           |
|                 |                                                 |
|   +-------------+-------------+                                   |
|   |  Telemóvel Android        |                                   |
|   |  -> abc123.devtunnels.ms  |   ? Funciona!                   |
|   +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### Como Configurar Dev Tunnel no Visual Studio

**Passo 1: Abrir Visual Studio 2022**

**Passo 2: Criar Dev Tunnel**
1. Clica no dropdown junto ao botão de "Start"
2. Seleciona **"Dev Tunnels"** ? **"Create Tunnel"**
3. Dá um nome (ex: "MyCOLL-API")
4. Seleciona **"Public"** para acesso sem autenticação
5. Clica **"Create"**

**Passo 3: Iniciar Tunnel**
1. No menu **View** ? **Output** ? **Dev Tunnels**
2. Verás um URL como: `https://abc123.devtunnels.ms`

**Passo 4: Atualizar MauiProgram.cs**

```csharp
// MyCOLL_MAUI/MauiProgram.cs

// ANTES (só funciona no PC):
// private const string API_BASE_URL = "https://localhost:7196";

// DEPOIS (funciona no telemóvel):
private const string API_BASE_URL = "https://abc123.devtunnels.ms";
```

### Testar no Android com ADB

**Passo 1: Habilitar Developer Options no Android**
1. Definições ? Sobre o Telefone
2. Toca 7x em "Número de Compilação"
3. Volta atrás ? "Opções de Programador"
4. Ativa "Depuração USB"

**Passo 2: Ligar ao PC**
1. Liga o telemóvel por USB
2. No telemóvel, aceita "Permitir Depuração USB"
3. No Visual Studio, o dispositivo aparece na lista

**Passo 3: Selecionar Target**
```
Debug ? | net8.0-android ? | Samsung Galaxy S21 ?
                              ^^^^^^^^^^^^^^^^^^^^
                              Teu dispositivo!
```

**Passo 4: Run (F5)**
- A app é compilada e instalada no telemóvel
- Abre automaticamente
- Podes ver logs no Output do Visual Studio

### Troubleshooting Comum

| Problema | Solução |
|----------|---------|
| "Connection refused" | Verificar se Dev Tunnel está ativo |
| "SSL Error" | Em debug, ignora erros SSL (já está no código) |
| Dispositivo não aparece | Reinstalar drivers USB / Aceitar no telemóvel |
| App crasha | Ver logs em View ? Output ? Device Log |

---

## Diagrama de Fluxos

### Fluxo Completo: Cliente Faz Compra

```
+------------------------------------------------------------------+
|               FLUXO: CLIENTE FAZ UMA COMPRA                       |
+------------------------------------------------------------------+
|                                                                   |
|  1. BROWSE PRODUTOS                                              |
|  +---------------------------+                                   |
|  | Home.razor                |                                   |
|  | - Ver categorias          |                                   |
|  | - Filtrar produtos        |                                   |
|  | - Adicionar ao carrinho   |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  2. ADICIONAR AO CARRINHO (Local)                                |
|  +---------------------------+                                   |
|  | CarrinhoService           |                                   |
|  | - Guarda em memória       |                                   |
|  | - Notifica UI (badge)     |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  3. VER CARRINHO                                                 |
|  +---------------------------+                                   |
|  | Carrinho.razor            |                                   |
|  | - Lista itens             |                                   |
|  | - Ajustar quantidades     |                                   |
|  | - Ver total               |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                | (Se não autenticado, redireciona)               |
|                v                                                  |
|  4. LOGIN                                                        |
|  +---------------------------+                                   |
|  | Login.razor               |                                   |
|  | -> AuthService.LoginAsync |                                   |
|  | -> API: POST /api/auth/login                                  |
|  | <- Token JWT              |                                   |
|  | -> Guardar no storage     |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  5. FINALIZAR COMPRA                                             |
|  +---------------------------+                                   |
|  | Carrinho.razor            |                                   |
|  | -> EncomendaService       |                                   |
|  | -> API: POST /api/encomendas                                  |
|  |    (com token no header)  |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  6. API PROCESSA                                                 |
|  +---------------------------+                                   |
|  | EncomendasController      |                                   |
|  | - Valida token (JWT)      |                                   |
|  | - Valida produtos/stock   |                                   |
|  | - Cria encomenda na BD    |                                   |
|  | - Reserva stock           |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  7. CONFIRMAÇÃO                                                  |
|  +---------------------------+                                   |
|  | EncomendaDetalhe.razor    |                                   |
|  | - Mostra dados encomenda  |                                   |
|  | - Estado: Pendente        |                                   |
|  | - Opções: Pagar/Cancelar  |                                   |
|  +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

### Fluxo: Fornecedor Cria Produto

```
+------------------------------------------------------------------+
|            FLUXO: FORNECEDOR CRIA PRODUTO                         |
+------------------------------------------------------------------+
|                                                                   |
|  1. LOGIN COMO FORNECEDOR                                        |
|  +---------------------------+                                   |
|  | Login.razor               |                                   |
|  | Email: fornecedor@...     |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  2. ACEDER A MEUS PRODUTOS                                       |
|  +---------------------------+                                   |
|  | MeusProdutos.razor        |                                   |
|  | [Authorize(Roles="Fornecedor")]                               |
|  | - Lista produtos próprios |                                   |
|  | - Botão "Novo Produto"    |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  3. CRIAR PRODUTO                                                |
|  +---------------------------+                                   |
|  | CriarProduto.razor        |                                   |
|  | - Nome, Descrição         |                                   |
|  | - Preço BASE              |                                   |
|  | - Categorias              |                                   |
|  | - Stock                   |                                   |
|  | -> FornecedorService      |                                   |
|  | -> API: POST /api/fornecedor/produtos                        |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  4. ESTADO: PENDENTE                                             |
|  +---------------------------+                                   |
|  | Produto fica PENDENTE     |                                   |
|  | Aguarda aprovação do      |                                   |
|  | Admin/Funcionário         |                                   |
|  +---------------------------+                                   |
|                |                                                  |
|                v (NO BACKOFFICE)                                  |
|  5. BACKOFFICE APROVA                                            |
|  +---------------------------+                                   |
|  | Pendentes.razor           |                                   |
|  | (GestaoLoja)              |                                   |
|  | - Define % da empresa     |                                   |
|  | - Calcula preço final     |                                   |
|  | - Aprova produto          |                                   |
|  +-------------+-------------+                                   |
|                |                                                  |
|                v                                                  |
|  6. PRODUTO ATIVO                                                |
|  +---------------------------+                                   |
|  | Estado: ATIVO             |                                   |
|  | Visível para clientes     |                                   |
|  | PrecoFinal = Base + %     |                                   |
|  +---------------------------+                                   |
|                                                                   |
+------------------------------------------------------------------+
```

---

## Resumo para Defesa

### Perguntas Frequentes e Respostas

**P: Porque separar em 4 projetos?**
> R: Separação de responsabilidades. O backoffice (GestaoLoja) acede diretamente à BD e é só para admin/funcionários. O frontend (Web/MAUI) usa API REST e é para clientes/fornecedores. A RCL permite partilhar código entre Web e MAUI.

**P: Porque usar RCL em vez de copiar código?**
> R: DRY - Don't Repeat Yourself. Com RCL, corrigimos um bug uma vez e aplica-se aos dois frontends. Garante consistência de UI e reduz manutenção.

**P: Qual a diferença entre Blazor Server e Blazor Hybrid?**
> R: Blazor Server (Web) renderiza no servidor e envia HTML via SignalR. Blazor Hybrid (MAUI) renderiza localmente dentro de um WebView nativo. O Hybrid tem melhor performance e acesso a APIs nativas.

**P: Como funciona a autenticação?**
> R: Usamos JWT. O utilizador faz login, a API valida credenciais e devolve um token. O frontend guarda o token (localStorage no Web, SecureStorage no MAUI) e envia-o no header de cada pedido à API.

**P: Porque o carrinho não usa API?**
> R: O carrinho é temporário e local. Só quando o cliente finaliza a compra é que criamos a encomenda na API. Isto evita pedidos desnecessários e permite funcionar parcialmente offline.

**P: O que é um Dev Tunnel?**
> R: É uma funcionalidade do Visual Studio que cria um URL público temporário para o localhost. Permite testar a app MAUI num telemóvel real que precisa de aceder à API local.

**P: Como proteges os endpoints da API?**
> R: Com atributo [Authorize(Roles = "...")]. O JWT contém a role do utilizador. A API valida o token e verifica se a role permite acesso ao endpoint.

### Tecnologias Utilizadas

| Tecnologia | Projeto | Propósito |
|------------|---------|-----------|
| Blazor Server | MyMedia_Web | Frontend web interativo |
| .NET MAUI + Blazor Hybrid | MyCOLL_MAUI | App nativa multiplataforma |
| Razor Class Library | RCLGeral | Código partilhado |
| ASP.NET Core Web API | REST | Endpoints RESTful |
| JWT Bearer | REST | Autenticação stateless |
| Entity Framework Core | REST/GestaoLoja | ORM para BD |
| SQL Server LocalDB | - | Base de dados |
| HttpClient | RCL | Comunicação com API |
| SecureStorage | MAUI | Armazenamento seguro |
| localStorage | Web | Armazenamento browser |

### Padrões de Design

1. **Dependency Injection** - Todos os serviços são injetados
2. **Interface Segregation** - ITokenStorageService permite implementações diferentes
3. **Repository Pattern** (implícito) - EF Core como abstração da BD
4. **Factory Pattern** - IDbContextFactory no Blazor Server
5. **Observer Pattern** - Eventos OnAuthStateChanged, OnCarrinhoChanged

### Endpoints da API REST

| Método | Endpoint | Autenticação | Descrição |
|--------|----------|--------------|-----------|
| POST | /api/auth/login | Não | Login |
| POST | /api/auth/registar | Não | Registo |
| GET | /api/produtos | Não | Listar produtos |
| GET | /api/produtos/{id} | Não | Detalhe produto |
| GET | /api/categorias | Não | Listar categorias |
| POST | /api/encomendas | Cliente | Criar encomenda |
| GET | /api/encomendas/historico | Cliente | Histórico |
| GET | /api/fornecedor/produtos | Fornecedor | Produtos próprios |
| POST | /api/fornecedor/produtos | Fornecedor | Criar produto |

### Rotas do Frontend

| Rota | Página | Proteção |
|------|--------|----------|
| / | Home.razor | Pública |
| /login | Login.razor | Pública |
| /registar | Registar.razor | Pública |
| /produto/{id} | ProdutoDetalhe.razor | Pública |
| /carrinho | Carrinho.razor | Pública (checkout requer login) |
| /minhas-encomendas | MinhasEncomendas.razor | Cliente |
| /encomenda/{id} | EncomendaDetalhe.razor | Cliente |
| /meus-produtos | MeusProdutos.razor | Fornecedor |
| /meus-produtos/criar | CriarProduto.razor | Fornecedor |
| /minhas-vendas | MinhasVendas.razor | Fornecedor |

---

## Credenciais de Teste (Frontend)

| Email | Password | Role | Estado |
|-------|----------|------|--------|
| cliente2@teste.com | Clie2..00 | Cliente | Ativo |
| fornecedor1@teste.com | Forn1..00 | Fornecedor | Ativo |

**Nota:** Admin e Funcionário só podem usar o Backoffice (GestaoLoja), não o Frontend.

---

*Documentação criada em: Janeiro 2025*
*Projeto: MyCOLL - Plataforma de Colecionáveis*

*Continua na Parte 4...*
