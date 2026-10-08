# 🔗 URL Shortener API

API REST para encurtamento e gerenciamento de URLs, desenvolvida com **ASP.NET Core**.

Este projeto está sendo desenvolvido como um projeto de estudo para aprofundar conhecimentos em desenvolvimento backend com **.NET**, persistência de dados, cache distribuído, arquitetura de APIs e conceitos de escalabilidade.

---

## 🚀 Tecnologias

- **C#**
- **.NET / ASP.NET Core**
- **Entity Framework Core**
- **PostgreSQL**
- **Redis**
- **Tailscale** — comunicação segura com serviços hospedados no servidor de desenvolvimento
- **OpenAPI**

### Principais bibliotecas

- `Microsoft.EntityFrameworkCore`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.Extensions.Caching.StackExchangeRedis`

---

## 📋 Funcionalidades

- [x] Criação de URLs encurtadas
- [x] Geração automática de códigos para URLs
- [x] Definição de código personalizado
- [x] Validação de códigos personalizados
- [x] Persistência das URLs no PostgreSQL
- [x] Contagem de acessos
- [x] Data de criação da URL
- [x] Data de expiração
- [x] Cache de URLs utilizando Redis
- [x] Configuração através de User Secrets
- [ ] Endpoint de redirecionamento
- [ ] Incremento otimizado de acessos utilizando Redis
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Dockerização
- [ ] CI/CD

---

## 🏗️ Arquitetura

A aplicação utiliza uma arquitetura simples baseada em **Controller → Service → Data Access**.

```text
                    ┌─────────────────┐
                    │     Client      │
                    │ Postman / Web   │
                    └────────┬────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │   Controller    │
                    │                 │
                    │ URLsController  │
                    └────────┬────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │     Service     │
                    │                 │
                    │      Urls       │
                    └───────┬─────────┘
                            │
                 ┌──────────┴──────────┐
                 │                     │
                 ▼                     ▼
        ┌─────────────────┐   ┌─────────────────┐
        │      Redis      │   │   PostgreSQL    │
        │                 │   │                 │
        │      Cache      │   │  Source of Truth │
        └─────────────────┘   └─────────────────┘
```

### Fluxo de consulta

A API utiliza o padrão **Cache-Aside** para consultar URLs:

```text
Request
   │
   ▼
Redis
   │
   ├── HIT ──────────────► Retorna URL
   │
   └── MISS
        │
        ▼
    PostgreSQL
        │
        ▼
    Armazena no Redis
        │
        ▼
    Retorna URL
```

O PostgreSQL permanece como **fonte de verdade**, enquanto o Redis é utilizado para reduzir a quantidade de consultas ao banco de dados.

---

## 📁 Estrutura do projeto

```text
UrlShortener.Api/
│
├── Controllers/
│   └── URLsController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   ├── Url.model.cs
│   ├── UrlCreated.response.cs
│   └── UrlData.request.cs
│
├── Services/
│   └── Urls.service.cs
│
├── Migrations/
│
├── Program.cs
├── appsettings.json
└── UrlShortener.Api.csproj
```

### Responsabilidade das camadas

**Controllers**

Responsáveis por receber as requisições HTTP e retornar as respostas da API.

**Services**

Contêm a lógica de negócio da aplicação.

**Models**

Representam os dados utilizados pela aplicação, incluindo entidades e objetos de request/response.

**Data**

Contém o `DbContext` responsável pela comunicação entre a aplicação e o PostgreSQL através do Entity Framework Core.

---

## 🗄️ Banco de dados

O projeto utiliza **PostgreSQL** como banco de dados principal.

A entidade principal é a `Url`:

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | `long` | Identificador da URL |
| `Code` | `string` | Código utilizado para acessar a URL encurtada |
| `OriginalUrl` | `string` | URL original |
| `CreatedAt` | `DateTime` | Data de criação |
| `ExpiresAt` | `DateTime?` | Data de expiração |
| `ClickCount` | `long` | Número de acessos |

O campo `Code` possui um **índice único**, garantindo que dois registros não possam utilizar o mesmo código.

---

## ⚡ Cache

O projeto utiliza **Redis** para armazenar temporariamente URLs frequentemente acessadas.

As chaves seguem o padrão:

```text
url:{code}
```

Exemplo:

```text
url:abc123
```

Atualmente o cache utiliza um TTL de **10 minutos**.

Caso a URL não esteja no Redis, a aplicação consulta o PostgreSQL e posteriormente armazena o resultado no cache.

---

## ⚙️ Configuração

As configurações sensíveis não devem ser armazenadas diretamente no `appsettings.json`.

Durante o desenvolvimento, o projeto utiliza **.NET User Secrets**.

Inicialize os User Secrets:

```bash
dotnet user-secrets init
```

Configure a conexão com PostgreSQL:

```bash
dotnet user-secrets set "ConnectionStrings:PostgreSQLConnection" "Host=localhost;Port=5432;Database=urlshortener_db;Username=postgres;Password=SUA_SENHA"
```

Configure o Redis:

```bash
dotnet user-secrets set "ConnectionStrings:CacheConnection" "localhost:6379,password=SUA_SENHA"
```

Em ambientes de produção, essas configurações podem ser fornecidas através de **Environment Variables**.

---

## 🛠️ Executando o projeto

### Pré-requisitos

Antes de executar a API, certifique-se de possuir:

- [.NET SDK](https://dotnet.microsoft.com/)
- PostgreSQL
- Redis
- Git

### Clonar o repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd UrlShortener.Api
```

### Restaurar dependências

```bash
dotnet restore
```

### Aplicar migrations

```bash
dotnet ef database update
```

### Executar a API

```bash
dotnet run
```

A API será iniciada utilizando as configurações do ambiente de desenvolvimento.

---

## 🔄 Entity Framework Core

Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration
```

Para aplicar as migrations:

```bash
dotnet ef database update
```

---

## 📡 API

### Criar uma URL

```http
POST /URLs
```

Exemplo de request:

```json
{
  "originalUrl": "https://www.example.com",
  "customCode": "example"
}
```

Exemplo de resposta:

```json
{
  "id": 1,
  "code": "example"
}
```

> Os endpoints podem sofrer alterações conforme o desenvolvimento do projeto.

---

## 🧪 Testes

Os testes automatizados ainda estão em desenvolvimento.

Planejamento:

```text
Unit Tests
    │
    ├── Services
    └── Business Rules

Integration Tests
    │
    ├── API
    ├── PostgreSQL
    └── Redis
```

---

## 📚 Objetivos de estudo

Este projeto está sendo utilizado para praticar e consolidar conhecimentos em:

- ASP.NET Core Web API
- C#
- Dependency Injection
- Entity Framework Core
- Migrations
- PostgreSQL
- Redis
- Cache-Aside Pattern
- REST APIs
- Configuration & User Secrets
- Environment Variables
- Clean Code
- Separação de responsabilidades
- Escalabilidade
- Concorrência
- Testes automatizados
- Docker
- CI/CD

---

## 🗺️ Roadmap

- [x] Estrutura inicial da API
- [x] Entity Framework Core
- [x] SQLite durante desenvolvimento inicial
- [x] Migração para PostgreSQL
- [x] Redis
- [x] Cache-Aside
- [x] User Secrets
- [ ] Endpoint de redirect
- [ ] Melhorar gerenciamento de `ClickCount`
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Docker
- [ ] Docker Compose
- [ ] Health Checks
- [ ] Logging estruturado
- [ ] Rate Limiting
- [ ] CI/CD
- [ ] Deploy

---

## 👨‍💻 Sobre o projeto

Projeto desenvolvido para estudos e prática de desenvolvimento backend utilizando o ecossistema **.NET**.

A arquitetura e as tecnologias serão evoluídas gradualmente conforme novos conceitos forem estudados e implementados.
