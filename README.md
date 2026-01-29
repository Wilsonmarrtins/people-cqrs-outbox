# people-cqrs-outbox

Exemplo de aplicação .NET demonstrando **CQRS (Command Query Responsibility Segregation)** com **bancos de dados separados para escrita e leitura**, utilizando **Entity Framework Core**, **Outbox Pattern transacional**, **SQL Server**, **MySQL**, **Docker Compose** e **Worker Service** para garantir **consistência eventual**.

---

## 🎯 Objetivo do Projeto

Demonstrar uma arquitetura moderna, robusta e escalável, aplicável a cenários reais de sistemas distribuídos, abordando:

- Separação clara entre leitura e escrita
- Consistência eventual
- Publicação confiável de eventos
- Processamento assíncrono
- Escalabilidade e desacoplamento

---

## 🧱 Arquitetura Geral

A aplicação segue:

- **CQS (Command Query Separation)**
- **CQRS (Command Query Responsibility Segregation)**
- **Outbox Pattern**
- **Clean Architecture**
- **DDD Tático (Entities, Commands, Handlers)**

### Visão macro

```
┌──────────────┐
│   API HTTP   │
│ ASP.NET Core │
└──────┬───────┘
       │ Commands (Create / Update / Delete)
       ▼
┌────────────────────┐
│ Write DB           │
│ SQL Server         │
│ - people           │
│ - outbox_messages  │
└────────┬───────────┘
         │ Outbox
         ▼
┌────────────────────┐
│ Worker Service     │
│ People.Worker      │
└────────┬───────────┘
         │ Projeção
         ▼
┌────────────────────┐
│ Read DB            │
│ MySQL              │
│ - people_read      │
└────────────────────┘
```

---

## ✍️ CQS vs CQRS

### CQS
- **Commands**: alteram estado (Create, Update, Delete)
- **Queries**: apenas leitura
- Um método nunca faz os dois

### CQRS
- Bancos separados
- Modelos independentes
- Escala e performance melhores

Neste projeto:
- **SQL Server** → Escrita
- **MySQL** → Leitura

---

## 🗄️ Bancos de Dados

### 🟦 Write Database (SQL Server)

Banco: `PeopleWrite`

#### Tabela `people`
| Campo | Tipo |
|-----|-----|
| Id | uniqueidentifier |
| Name | nvarchar |
| Age | int |
| Sex | nvarchar |
| Rg | nvarchar |
| Cpf | nvarchar |
| CreatedAtUtc | datetime |

#### Tabela `outbox_messages`
| Campo | Tipo |
|-----|-----|
| Id | uniqueidentifier |
| Type | nvarchar |
| PayloadJson | nvarchar(max) |
| OccurredAtUtc | datetime |
| ProcessedAtUtc | datetime (nullable) |

> Usa **EF Core Migrations**

---

### 🟩 Read Database (MySQL)

Banco: `PeopleRead`

#### Tabela `people_read`
| Campo | Tipo |
|-----|-----|
| id | char(36) |
| name | varchar |
| age | int |
| sex | varchar |
| rg | varchar |
| cpf | varchar |
| created_at_utc | datetime |

> Não utiliza migrations  
> É mantido exclusivamente pelo Worker

---

## 📦 Outbox Pattern

### Por que Outbox?

Sem Outbox:
- Dados gravados sem evento
- Eventos publicados sem dados
- Falhas difíceis de recuperar

Com Outbox:
- Escrita + evento na mesma transação
- Garantia de entrega
- Retry automático

### Fluxo

1. Command grava entidade
2. Command grava Outbox
3. Commit único
4. Worker processa

---

## 🔁 Worker Service (People.Worker)

Responsável por:

- Ler eventos não processados da Outbox
- Desserializar Payload JSON
- Atualizar o ReadDb (MySQL)
- Marcar eventos como processados

### Eventos suportados
- `PersonCreated`
- `PersonUpdated`
- `PersonDeleted`

### Garantias
- Processamento idempotente
- Retry automático
- Consistência eventual

---

## 🌐 Endpoints da API

### Criar pessoa
`POST /api/people`

### Atualizar pessoa
`PUT /api/people/{id}`

### Remover pessoa
`DELETE /api/people/{id}`

> A API **nunca acessa o ReadDb**

---

## 🐳 Docker Compose

### Subir os bancos
```bash
docker compose up -d
```

Serviços:
- SQL Server (WriteDb)
- MySQL (ReadDb)

---

## 🧪 Migrations (WriteDb)

### Criar migration
```bash
dotnet ef migrations add InitialWrite \
  --project People.Infrastructure \
  --startup-project People.Api \
  --context WriteDbContext
```

### Aplicar migration
```bash
dotnet ef database update \
  --project People.Infrastructure \
  --startup-project People.Api \
  --context WriteDbContext
```

---

## 🚀 Tecnologias

- .NET 9
- ASP.NET Core
- EF Core
- SQL Server
- MySQL
- Docker
- CQRS / CQS
- Outbox Pattern
- Worker Service

---

## 📌 Considerações Finais

Este projeto demonstra uma arquitetura **enterprise-ready**, focada em:

- Clareza
- Robustez
- Evolução
- Observabilidade
- Escalabilidade

Ideal como **portfólio profissional** ou base para sistemas distribuídos reais.

---

👤 **Autor:** Wilson Martins  
📅 Projeto educacional e arquitetural
