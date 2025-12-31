# Estrutura Detalhada do Frontend MyCOLL - Parte 1

## Índice Geral (dividido em 3 partes)

### Parte 1 (este ficheiro)
1. [Introdução - Visão Geral do Sistema](#introducao---visao-geral-do-sistema)
2. [Arquitetura Multi-Projeto](#arquitetura-multi-projeto)
3. [O que é uma RCL (Razor Class Library)](#o-que-e-uma-rcl)
4. [Blazor Hybrid vs Blazor Web](#blazor-hybrid-vs-blazor-web)

### Parte 2
5. Estrutura de Pastas Completa
6. Serviços e Comunicação com API
7. Sistema de Autenticação JWT

### Parte 3
8. Componentes e Páginas
9. Dev Tunnels - Testar em Dispositivos Reais
10. Diagrama de Fluxos
11. Resumo para Defesa

### Parte 4 (novo!)
12. Sistema de Estilos CSS
13. Tratamento de Erros
14. Configuração CORS na API
15. Ciclo de Vida dos Componentes Blazor
16. Boas Práticas Implementadas

---

## Introdução - Visão Geral do Sistema

### O Que é o MyCOLL?

O **MyCOLL** é uma plataforma de **colecionáveis** (moedas, selos, etc.) composta por **4 projetos** que trabalham em conjunto:

```
+------------------------------------------------------------------+
|                     SISTEMA MyCOLL COMPLETO                       |
+------------------------------------------------------------------+
|                                                                   |
|  +------------------+        +------------------+                 |
|  |   GestaoLoja     |        |      REST        |                 |
|  |   (Backoffice)   |        |    (API REST)    |                 |
|  |                  |        |                  |                 |
|  | - Blazor Server  |        | - ASP.NET Core   |                 |
|  | - Admin/Func     |        | - JWT Auth       |                 |
|  | - Acesso BD      |        | - Endpoints      |                 |
|  +--------+---------+        +--------+---------+                 |
|           |                           |                           |
|           |    Entity Framework       |                           |
|           +----------+----------------+                           |
|                      |                                            |
|                      v                                            |
|           +------------------+                                    |
|           |   SQL Server     |                                    |
|           |   (LocalDB)      |                                    |
|           +------------------+                                    |
|                      ^                                            |
|                      | HTTP/HTTPS (JSON)                          |
|                      |                                            |
|  +-------------------+--------------------+                       |
|  |              RCLGeral                |                       |
|  |        (Razor Class Library)          |                       |
|  |                                        |                       |
|  | - Componentes Blazor Partilhados      |                       |
|  | - Serviços (Auth, Produto, etc.)      |                       |
|  | - Modelos de Dados                    |                       |
|  | - CSS/Estilos                         |                       |
|  +-------------------+--------------------+                       |
|           |                   |                                   |
|           v                   v                                   |
|  +----------------+   +------------------+                        |
|  |  MyMedia_Web    |   |   MyCOLL_MAUI    |                        |
|  |  (Browser)     |   |   (Nativo)       |                        |
|  |                |   |                  |                        |
|  | - Blazor Server|   | - .NET MAUI      |                        |
|  | - Chrome/Edge  |   | - Android/iOS    |                        |
|  | - localStorage |   | - Windows/Mac    |                        |
|  +----------------+   | - SecureStorage  |                        |
|                       +------------------+                        |
+------------------------------------------------------------------+
```

### Porque 4 Projetos Separados?

| Projeto | Propósito | Utilizadores |
|---------|-----------|--------------|
| **GestaoLoja** | Backoffice interno | Admin, Funcionários |
| **REST** | API RESTful | Frontends (Web/MAUI) |
| **RCLGeral** | Código partilhado | - (biblioteca) |
| **MyMedia_Web** | Frontend browser | Clientes, Fornecedores |
| **MyCOLL_MAUI** | App nativa | Clientes, Fornecedores |

### Analogia do Mundo Real

Imagina uma loja física:

```
+----------------------------------+
|          LOJA FÍSICA             |
+----------------------------------+
|                                  |
|  FRENTE DA LOJA (Frontend)       |
|  +-----------+  +-----------+    |
|  | Loja Web  |  | App Mobile|    |  <-- Clientes entram aqui
|  | (browser) |  | (Android) |    |
|  +-----------+  +-----------+    |
|        |              |          |
|        v              v          |
|  +-------------------------+     |
|  |   BALCÃO DE ATENDIMENTO |     |  <-- API REST
|  |   (intermediário)       |     |      (traduz pedidos)
|  +-------------------------+     |
|              |                   |
|              v                   |
|  +-------------------------+     |
|  |   BASTIDORES            |     |  <-- Backoffice
|  |   (só funcionários)     |     |      (gestão interna)
|  |                         |     |
|  |   - Gerir stock         |     |
|  |   - Aprovar produtos    |     |
|  |   - Processar vendas    |     |
|  +-------------------------+     |
|              |                   |
|              v                   |
|  +-------------------------+     |
|  |   ARMAZÉM (Base Dados)  |     |  <-- SQL Server
|  +-------------------------+     |
+----------------------------------+
```

---

## Arquitetura Multi-Projeto

### Porque Separar Frontend do Backoffice?

1. **Segurança**
   - O backoffice NUNCA é exposto ao público
   - Clientes não conseguem aceder a funcionalidades admin
   - Superfície de ataque reduzida

2. **Escalabilidade**
   - Cada parte pode escalar independentemente
   - API pode servir múltiplos frontends

3. **Manutenção**
   - Equipas diferentes podem trabalhar em paralelo
   - Mudanças no frontend não afetam backoffice

### Como os Projetos se Relacionam

```
MyMedia_Web.csproj                    MyCOLL_MAUI.csproj
       |                                    |
       |  <ProjectReference>                |  <ProjectReference>
       v                                    v
+----------------------------------------------------------+
|                    RCLGeral.csproj                      |
|                                                          |
|  - Components/Pages/*.razor    (Páginas partilhadas)     |
|  - Components/Layout/*.razor   (Layout partilhado)       |
|  - Services/*.cs               (Lógica de negócio)       |
|  - Models/*.cs                 (DTOs/Modelos)            |
|  - wwwroot/css/*.css           (Estilos partilhados)     |
+----------------------------------------------------------+

REST.csproj
       |
       |  <ProjectReference>
       v
+----------------------------------------------------------+
|                   GestaoLoja.csproj                       |
|                                                          |
|  - Data/ApplicationDbContext.cs  (Acesso a dados)        |
|  - Data/ApplicationUser.cs       (Modelo utilizador)     |
|  - Entities/*.cs                 (Entidades BD)          |
+----------------------------------------------------------+
```

### Ficheiros .csproj - As Referências

**MyMedia_Web.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  
  <!-- Referência à RCL -->
  <ItemGroup>
    <ProjectReference Include="..\RCLGeral\RCLGeral.csproj" />
  </ItemGroup>
</Project>
```

**MyCOLL_MAUI.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <!-- Múltiplas plataformas! -->
    <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-windows10.0.19041.0</TargetFrameworks>
    <UseMaui>true</UseMaui>
  </PropertyGroup>
  
  <!-- Mesma referência à RCL -->
  <ItemGroup>
    <ProjectReference Include="..\RCLGeral\RCLGeral.csproj" />
  </ItemGroup>
</Project>
```

**REST.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  
  <!-- Referência ao Backoffice para usar entidades e DbContext -->
  <ItemGroup>
    <ProjectReference Include="..\GestaoLoja\GestaoLoja.csproj" />
  </ItemGroup>
  
  <!-- Packages para JWT -->
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.22" />
  </ItemGroup>
</Project>
```

---

## O que é uma RCL (Razor Class Library)?

### Definição Simples

Uma **Razor Class Library (RCL)** é uma biblioteca que contém:
- Componentes Razor (.razor)
- Páginas Blazor
- CSS/JavaScript
- Serviços C#
- Modelos de dados

Que podem ser **partilhados** entre múltiplos projetos Blazor.

### Porque Usar RCL neste Projeto?

**Sem RCL (código duplicado):**
```
MyMedia_Web/
??? Components/Pages/Home.razor      <-- Código A
??? Components/Pages/Login.razor     <-- Código B
??? Services/AuthService.cs          <-- Código C
??? Models/ProdutoModel.cs           <-- Código D

MyCOLL_MAUI/
??? Components/Pages/Home.razor      <-- Código A (DUPLICADO!)
??? Components/Pages/Login.razor     <-- Código B (DUPLICADO!)
??? Services/AuthService.cs          <-- Código C (DUPLICADO!)
??? Models/ProdutoModel.cs           <-- Código D (DUPLICADO!)
```

**Com RCL (código partilhado):**
```
RCLGeral/                          <-- ÚNICA FONTE!
??? Components/Pages/Home.razor
??? Components/Pages/Login.razor
??? Services/AuthService.cs
??? Models/ProdutoModel.cs

MyMedia_Web/
??? Program.cs                       <-- Só configuração
??? Components/
    ??? App.razor                    <-- Só estrutura base
    ??? WebTokenStorageService.cs    <-- Específico Web

MyCOLL_MAUI/
??? MauiProgram.cs                   <-- Só configuração
??? MauiTokenStorageService.cs       <-- Específico MAUI
```

### Vantagens da RCL

| Vantagem | Descrição |
|----------|-----------|
| **DRY** | Don't Repeat Yourself - código único |
| **Consistência** | UI igual em Web e MAUI |
| **Manutenção** | Corrigir bug uma vez, aplica-se a todos |
| **Testes** | Testar componentes isoladamente |

### Como a RCL é Consumida

**No Routes.razor do MyMedia_Web:**
```razor
<Router AppAssembly="typeof(Program).Assembly" 
        AdditionalAssemblies="new[] { typeof(RCLGeral._Imports).Assembly }">
    <!-- Router encontra páginas na RCL automaticamente! -->
</Router>
```

**No Routes.razor do MyCOLL_MAUI:**
```razor
<Router AppAssembly="@typeof(MauiProgram).Assembly" 
        AdditionalAssemblies="new[] { typeof(RCLGeral._Imports).Assembly }">
    <!-- Mesmo código, mesmas páginas! -->
</Router>
```

---

## Blazor Hybrid vs Blazor Web

### O que é Blazor?

**Blazor** é um framework da Microsoft para criar interfaces web interativas usando **C#** em vez de JavaScript.

### Modos de Renderização

```
+------------------------------------------------------------------+
|                    MODOS DE BLAZOR                                |
+------------------------------------------------------------------+
|                                                                   |
|  BLAZOR SERVER (MyMedia_Web usa isto)                             |
|  +------------------------------------------------------------+  |
|  |                                                            |  |
|  |   Browser                         Servidor                 |  |
|  |  +--------+                      +--------+                |  |
|  |  |  HTML  |<---- SignalR ------->|  C#    |                |  |
|  |  |  DOM   |     (WebSocket)      | Lógica |                |  |
|  |  +--------+                      +--------+                |  |
|  |                                                            |  |
|  |  - UI renderizada no servidor                              |  |
|  |  - Eventos enviados via SignalR                            |  |
|  |  - Requer conexão constante                                |  |
|  +------------------------------------------------------------+  |
|                                                                   |
|  BLAZOR WEBASSEMBLY                                              |
|  +------------------------------------------------------------+  |
|  |                                                            |  |
|  |   Browser (tudo aqui!)                                     |  |
|  |  +------------------+                                      |  |
|  |  |  .NET Runtime    |  <-- Descarregado para o browser     |  |
|  |  |  (WebAssembly)   |                                      |  |
|  |  +------------------+                                      |  |
|  |  |  Aplicação C#    |                                      |  |
|  |  +------------------+                                      |  |
|  |                                                            |  |
|  |  - Tudo corre no browser                                   |  |
|  |  - Não precisa de servidor (depois de carregar)            |  |
|  |  - Download inicial maior                                  |  |
|  +------------------------------------------------------------+  |
|                                                                   |
|  BLAZOR HYBRID (MyCOLL_MAUI usa isto)                            |
|  +------------------------------------------------------------+  |
|  |                                                            |  |
|  |   App Nativa (Android/iOS/Windows)                         |  |
|  |  +------------------+                                      |  |
|  |  |   WebView        |  <-- Componente nativo               |  |
|  |  |  +------------+  |                                      |  |
|  |  |  | Blazor UI  |  |  <-- Renderiza HTML/CSS              |  |
|  |  |  +------------+  |                                      |  |
|  |  +------------------+                                      |  |
|  |  |   .NET MAUI      |  <-- Acesso a APIs nativas           |  |
|  |  |   (C# nativo)    |      (câmara, GPS, ficheiros...)     |  |
|  |  +------------------+                                      |  |
|  |                                                            |  |
|  |  - Blazor dentro de app nativa                             |  |
|  |  - Melhor performance                                      |  |
|  |  - Acesso a funcionalidades do dispositivo                 |  |
|  +------------------------------------------------------------+  |
+------------------------------------------------------------------+
```

### Comparação Prática

| Aspeto | Blazor Server (Web) | Blazor Hybrid (MAUI) |
|--------|---------------------|----------------------|
| **Onde corre** | Servidor | Dispositivo |
| **Conexão** | Precisa de internet constante | Pode funcionar offline |
| **Performance** | Depende da latência | Muito rápida |
| **Instalação** | Não precisa | Instala como app |
| **Atualizações** | Automáticas | Precisa atualizar app |
| **Acesso nativo** | Não | Sim (câmara, GPS, etc.) |

### Porque Escolhemos Esta Combinação?

1. **MyMedia_Web (Blazor Server)**
   - Clientes podem usar sem instalar nada
   - Funciona em qualquer browser
   - Fácil de atualizar (só no servidor)

2. **MyCOLL_MAUI (Blazor Hybrid)**
   - Experiência de app nativa
   - Pode funcionar offline (parcialmente)
   - Notificações push
   - Presença nas app stores

3. **RCLGeral (código partilhado)**
   - Mesma UI nos dois
   - Manutenção simplificada
   - Consistência garantida

---

*Continua na Parte 2...*
