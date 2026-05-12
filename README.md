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
> 📡 **[API](https://sua-api.azurewebsites.net/scalar/v1)**

---

## 🖼️ Screenshots

| Listagem de Livros | Detalhes do Livro |
-----
![image alt](https://github.com/mikaelkaina/BookStore/blob/master/images/Captura%20de%20tela%202026-05-12%20161958.png?raw=true)
-----
![image alt](https://github.com/mikaelkaina/BookStore/blob/master/images/Captura%20de%20tela%202026-05-12%20162614.png?raw=true)

| Carrinho | Pedido |
|---|---|
| ![cart](./docs/screenshots/cart.png) | ![order](./docs/screenshots/order.png) |

---

## 🏗️ Arquitetura

O projeto segue **Clean Architecture** com separação em quatro camadas,
onde as dependências sempre fluem de fora para dentro:

```
BookStore.Domain          ← Entidades, Value Objects, Domain Events
BookStore.Application     ← Use Cases, CQRS, DTOs, Validators
BookStore.Infrastructure  ← EF Core, Repositórios, Identity, JWT
BookStore.API             ← Controllers, Middlewares, Program.cs
BookStore.UnitTests       ← Testes unitários do Domain
```
