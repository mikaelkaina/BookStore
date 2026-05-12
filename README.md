# 📚 BookStore — Loja Virtual de Livros Fullstack

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)
![React](https://img.shields.io/badge/React-19-61DAFB?style=flat&logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-5-3178C6?style=flat&logo=typescript)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=flat&logo=microsoftsqlserver)
![Azure](https://img.shields.io/badge/Azure-Deploy-0078D4?style=flat&logo=microsoftazure)

Aplicação fullstack de e-commerce de livros construída com foco em
arquitetura limpa, domínio rico e boas práticas de mercado.
O backend segue Clean Architecture com DDD e o frontend é uma
SPA em React com TypeScript.

> 🌐 **[Acesse a aplicação](https://jolly-flower-02868870f.7.azurestaticapps.net/)**
> 📡 **[API](bookstore-f4btctchfmcvgzcu.brazilsouth-01.azurewebsites.net/scalar)**

---

##  Screenshots

| Listagem de Livros | Detalhes do Livro |
-----
![image alt](https://github.com/mikaelkaina/BookStore/blob/master/images/Captura%20de%20tela%202026-05-12%20161958.png?raw=true)
-----
![image alt](https://github.com/mikaelkaina/BookStore/blob/master/images/Captura%20de%20tela%202026-05-12%20162614.png?raw=true)

------

| Carrinho | Pedido |
-----
![image alt](https://github.com/mikaelkaina/BookStore/blob/master/images/Captura%20de%20tela%202026-05-12%20164800.png?raw=true)
-----
![image alt](https://github.com/mikaelkaina/BookStore/blob/master/images/Captura%20de%20tela%202026-05-12%20164733.png?raw=true)

---

##  Arquitetura

O projeto segue **Clean Architecture** com separação em quatro camadas,
onde as dependências sempre fluem de fora para dentro:

```
BookStore.Domain          ← Entidades, Value Objects, Domain Events
BookStore.Application     ← Use Cases, CQRS, DTOs, Validators
BookStore.Infrastructure  ← EF Core, Repositórios, Identity, JWT
BookStore.API             ← Controllers, Middlewares, Program.cs
BookStore.UnitTests       ← Testes unitários do Domain
```

┌──────────────────────────────────────────┐
│                  API                     │
│  ┌────────────────────────────────────┐  │
│  │           Application              │  │
│  │  ┌──────────────────────────────┐  │  │
│  │  │         Infrastructure       │  │  │
│  │  │  ┌────────────────────────┐  │  │  │
│  │  │  │        Domain          │  │  │  │
│  │  │  └────────────────────────┘  │  │  │
│  │  └──────────────────────────────┘  │  │
│  └────────────────────────────────────┘  │
└──────────────────────────────────────────┘

---

##  Tecnologias

### Backend
| Tecnologia | Versão | Uso |
|---|---|---|
| .NET | 10 | Framework principal |
| ASP.NET Core | 10 | Web API |
| Entity Framework Core | 10 | ORM |
| SQL Server | 2022 | Banco de dados |
| MediatR | 12 | CQRS + Pipeline Behaviors |
| FluentValidation | 11 | Validação de comandos |
| ASP.NET Identity | 10 | Gerenciamento de usuários |
| JWT Bearer | 10 | Autenticação stateless |
| Scalar | - | Documentação da API |

### Frontend
| Tecnologia | Versão | Uso |
|---|---|---|
| React | 19 | UI Framework |
| TypeScript | 5 | Tipagem estática |
| Vite | 6 | Build tool |
| Tailwind CSS | 4 | Estilização |
| React Router DOM | 6 | Roteamento |
| TanStack React Query | 5 | Cache e estado servidor |
| Axios | 1 | Cliente HTTP |
| Lucide React | - | Ícones |

### DevOps
| Tecnologia | Uso |
|---|---|
| Azure App Service | Hospedagem da API |
| Azure SQL Database | Banco de dados em produção |
| Azure Static Web Apps | Hospedagem do frontend |
| GitHub Actions | CI/CD |

---

##  Padrões e Práticas

### Domain-Driven Design (DDD)
- **Entidades ricas** com comportamento encapsulado — sem anemia
- **Value Objects** imutáveis: `Money`, `Email`, `Isbn`, `Address`
- **Domain Events**: `BookCreatedEvent`, `OrderShippedEvent`, etc
- **Aggregate Roots**: `Book`, `Order`, `Cart`, `Customer`, `Category`
- **Erros de domínio** tipados por aggregate: `BookErrors`, `OrderErrors`

### Clean Architecture
- Dependências sempre apontam para o Domain
- Domain não conhece nenhum framework externo
- Interfaces definidas no Domain, implementadas na Infrastructure

### CQRS com MediatR
- **Commands** para operações de escrita com efeito colateral
- **Queries** para leitura — sem alterar estado
- **Pipeline Behaviors**: `LoggingBehavior` → `ValidationBehavior` → Handler

### Result Pattern
```csharp
// Sem exceptions no fluxo de negócio
public static Result<Book> Create(string title, ...)
{
    if (string.IsNullOrWhiteSpace(title))
        return Result.Failure<Book>(Error.Validation(nameof(Title), "Title is required."));

    return Result.Success(new Book(...));
}
```

### Repository Pattern + Unit of Work
- Interfaces de repositório no Domain
- Implementações com EF Core na Infrastructure
- `IUnitOfWork` desacoplado dos repositórios

### Domain Events com Interceptor
SavingChangesAsync → coleta eventos → salva em Context.Items
↓ commit no banco ↓
SavedChangesAsync  → publica via MediatR → handlers executam

## Autenticação e Autorização

### Fluxo JWT + Refresh Token
Login → Access Token (30 min) + Refresh Token (7 dias)
↓
Token expira → Interceptor detecta 401
↓
Chama /auth/refresh automaticamente
↓
Novos tokens → request original reenviada

### Roles
| Role | Permissões |
|---|---|
| **Guest** | Ver livros, categorias e detalhes |
| **Customer** | Carrinho, checkout, pedidos próprios, pagamento |
| **Admin** | Gerenciar livros, categorias, todos os pedidos, ciclo de vida dos pedidos |

---

##  Fluxo Principal

Usuário navega pelos livros sem login
Tenta adicionar ao carrinho → redirecionado para login
Faz login ou se cadastra
Adiciona livros ao carrinho
Finaliza compra informando endereço de entrega
Preenche dados do cartão na tela de pagamento
Pedido criado com status Pending
Admin confirma pagamento → Processing → Enviado → Entregue
Customer acompanha status em tempo real com linha do tempo
