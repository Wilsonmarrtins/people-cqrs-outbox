
# people-cqrs-outbox

Exemplo de aplicação .NET demonstrando **CQRS (Command Query Responsibility Segregation)** com **bancos de dados separados para escrita e leitura**, utilizando **Entity Framework Core**, **Outbox Pattern transacional**, **SQL Server**, **MySQL**, **Docker Compose** e **processos em segundo plano (Worker)** para garantir **consistência eventual**.

---

## 🎯 Objetivo do Projeto

Este projeto tem como objetivo demonstrar, de forma prática e organizada, uma arquitetura **enterprise-ready**, abordando problemas reais de sistemas distribuídos, como:

- Separação de responsabilidades entre escrita e leitura
- Consistência eventual
- Confiabilidade na publicação de eventos
- Escalabilidade e desacoplamento
- Integração entre múltiplos bancos de dados

---

## 🧱 Arquitetura Geral

A aplicação segue os seguintes princípios:

- **CQS (Command Query Separation)**  
- **CQRS (Command Query Responsibility Segregation)**  
- **Outbox Pattern**
- **Clean Architecture**
- **Single Responsibility Principle**

### Visão macro

```
API (ASP.NET Core)
 ├── Commands (Write)
 │    └── SQL Server (WriteDb)
 │         ├── People
 │         └── OutboxMessages
 │
 ├── Queries (Read)
 │    └── MySQL (ReadDb)
 │         └── People_Read
 │
 └── Worker (Background Service)
      └── Lê Outbox → Atualiza ReadDb
```

---

## ✍️ CQS vs CQRS

### CQS – Command Query Separation

- **Commands**: executam ações que alteram estado (INSERT, UPDATE, DELETE)
- **Queries**: apenas consultam dados (SELECT)
- Um método **nunca faz as duas coisas ao mesmo tempo**

### CQRS – Command Query Responsibility Segregation

CQRS leva o CQS além:

- Bancos de dados **separados**
- Modelos de dados **otimizados para cada uso**
- Escalabilidade independente
- Maior clareza arquitetural

👉 Neste projeto:
- **WriteDb (SQL Server)** → Commands
- **ReadDb (MySQL)** → Queries

---

## 🗄️ Bancos de Dados

### 🟦 Write Database (SQL Server)

Responsável por **todas as escritas** do sistema.

**Banco:** `PeopleWrite`

**Tabelas:**

#### `people`
| Campo | Tipo |
|------|------|
| Id | uniqueidentifier |
| Name | nvarchar |
| Age | int |
| Sex | nvarchar |
| Rg | nvarchar |
| Cpf | nvarchar |
| CreatedAtUtc | datetime |

#### `outbox_messages`
| Campo | Tipo |
|------|------|
| Id | uniqueidentifier |
| Type | nvarchar |
| PayloadJson | nvarchar(max) |
| OccurredAtUtc | datetime |
| ProcessedAtUtc | datetime (nullable) |

> Este banco utiliza **EF Core Migrations**.

---

### 🟩 Read Database (MySQL)

Responsável **somente por leitura**, otimizado para consultas.

**Banco:** `PeopleRead`

**Tabela:**

#### `people_read`
| Campo | Tipo |
|------|------|
| id | char(36) |
| name | varchar |
| age | int |
| sex | varchar |
| rg | varchar |
| cpf | varchar |
| created_at_utc | datetime |

> ❗ Não utiliza migrations.  
> O modelo é mantido via **eventos processados pelo Worker**.

---

## 📦 Outbox Pattern

### Por que usar Outbox?

Sem Outbox:
- ❌ Dados podem ser salvos sem evento
- ❌ Eventos podem ser publicados sem dados
- ❌ Inconsistência entre sistemas

Com Outbox:
- ✅ Escrita e evento na **mesma transação**
- ✅ Garantia de entrega
- ✅ Resiliência a falhas

### Como funciona neste projeto

1. Command grava `Person`
2. Command grava `OutboxMessage`
3. Tudo é confirmado na **mesma transação**
4. Worker processa eventos pendentes

---

## 🔁 Worker (Em breve)

O **People.Worker** será responsável por:

- Ler mensagens não processadas da `outbox_messages`
- Desserializar o evento
- Atualizar o **ReadDb (MySQL)**
- Marcar a mensagem como processada

### Responsabilidades do Worker

- Retry com backoff
- Idempotência
- Processamento seguro
- Consistência eventual

---

## 🐳 Docker Compose

### Subir os bancos de dados

```bash
docker compose up -d
```

Serviços:
- `people_sqlserver` → SQL Server (WriteDb)
- `people_mysql` → MySQL (ReadDb)

### Ver containers

```bash
docker compose ps
```

---

## 🧪 Migrations (WriteDb)

### Criar migration

```bash
dotnet ef migrations add InitialWrite   --project People.Infrastructure   --startup-project People.Api   --context WriteDbContext   --output-dir WriteDb/Migrations
```

### Aplicar migration

```bash
dotnet ef database update   --project People.Infrastructure   --startup-project People.Api   --context WriteDbContext
```

---

## 🚀 Tecnologias Utilizadas

- .NET 9
- ASP.NET Core
- Entity Framework Core
- SQL Server
- MySQL
- Docker / Docker Compose
- CQRS / CQS
- Outbox Pattern
- Clean Architecture

---

## 📌 Considerações Finais

Este projeto não é um CRUD simples.  
Ele demonstra **decisões arquiteturais reais**, utilizadas em sistemas distribuídos modernos, com foco em:

- Robustez
- Escalabilidade
- Clareza
- Evolução futura

Ideal para estudo avançado, portfólio profissional e base para sistemas enterprise.

---

👤 **Autor:** Wilson Martins  
📅 **Projeto educacional / arquitetural**
