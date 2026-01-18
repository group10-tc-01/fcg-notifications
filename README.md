# 📧 FCG.Notifications - Sistema de Notificações por Email

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Kafka](https://img.shields.io/badge/Apache_Kafka-2.12-black.svg)](https://kafka.apache.org/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Responsabilidade](#-responsabilidade)
- [Arquitetura](#-arquitetura)
- [Tecnologias e Bibliotecas](#-tecnologias-e-bibliotecas)
- [Regras de Negócio](#-regras-de-negócio)
- [Eventos Consumidos](#-eventos-consumidos)
- [Configuração e Execução](#-configuração-e-execução)

---

## 🎯 Sobre o Projeto

**FCG.Notifications** é um serviço de **Background Worker** desenvolvido em .NET 8 responsável pelo envio de notificações por email aos usuários da plataforma FCG. O serviço implementa uma arquitetura orientada a eventos (**Event-Driven Architecture**), consumindo eventos do **Apache Kafka** e enviando emails personalizados através do **Azure Communication Services**.

### 🚀 Responsabilidade

O serviço é responsável por:

- ✅ **Consumir eventos de negócio** do Kafka (UserCreatedEvent, PaymentProcessedEvent)
- 📨 **Enviar emails transacionais** de boas-vindas e confirmação de compras
- 🎨 **Gerar templates HTML personalizados** para cada tipo de notificação
- 🔄 **Processar mensagens de forma assíncrona** e resiliente
- 📊 **Registrar logs estruturados** com Serilog e Seq
- ⚡ **Garantir entrega confiável** com commit manual de offsets no Kafka

---

## 🏛️ Arquitetura

A aplicação segue os princípios da **Vertical Slice Architecture** e **Event-Driven Architecture**, onde cada funcionalidade (feature) é organizada como uma fatia vertical independente, contendo toda a lógica necessária para processar um tipo específico de evento.

### Estrutura por Features (Vertical Slices)

```
┌──────────────────────────────────────────────────────────────┐
│             FCG.Notifications.Worker                         │
│                 (Host Application)                           │
└──────────────────────────────┬───────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────┐
│         FCG.Notifications.Application                        │
├──────────────────────────────────────────────────────────────┤
│  ┌────────────────────────┐  ┌────────────────────────┐    │
│  │  Feature: UserCreated  │  │Feature: PaymentProc.   │    │
│  ├────────────────────────┤  ├────────────────────────┤    │
│  │ • UserCreatedEvent     │  │ • PaymentProcessedEvent│    │
│  │ • UserCreatedConsumer  │  │ • PaymentProcConsumer  │    │
│  │ • WelcomeEmailTemplate │  │ • PaymentEmailTemplate │    │
│  └────────────────────────┘  └────────────────────────┘    │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐    │
│  │           Common (Shared Infrastructure)           │    │
│  ├──────────────────────────────────────────────────────┤    │
│  │ • BaseKafkaConsumer  • EmailService                │    │
│  │ • IEmailService      • EmailSettings               │    │
│  │ • EmailTemplateBase  • KafkaSettings               │    │
│  └──────────────────────────────────────────────────────┘    │
└────────────────┬───────────────────────┬─────────────────────┘
                 │                       │
                 ▼                       ▼
        ┌────────────────┐      ┌────────────────┐
        │  Apache Kafka  │      │ Azure Comm.    │
        │   (Events)     │      │ Services       │
        └────────────────┘      └────────────────┘
```

### Fluxo de Processamento

```
┌─────────────┐     ┌──────────────┐     ┌────────────────┐     ┌──────────┐
│   Kafka     │────▶│   Consumer   │────▶│  Email Service │────▶│  Azure   │
│   Topic     │     │  (Worker)    │     │   + Template   │     │   Email  │
└─────────────┘     └──────────────┘     └────────────────┘     └──────────┘
     ▲                    │                       │
     │                    │                       │
     │                    ▼                       ▼
     │              ┌──────────┐           ┌────────────┐
     │              │   Logs   │           │   Commit   │
     │              │ (Serilog)│           │  (Offset)  │
     │              └──────────┘           └────────────┘
     │                                            │
     └────────────────────────────────────────────┘
```

### Vertical Slices (Features)

Cada feature representa uma **fatia vertical completa** do sistema, encapsulando toda a lógica necessária para processar um tipo específico de evento.

#### 🎯 **Feature: UserCreated** (`Features/UserCreated/`)
**Responsabilidade**: Processar criação de usuários e enviar email de boas-vindas

- 📋 `UserCreatedEvent.cs` - Contrato do evento Kafka
- 🔄 `UserCreatedConsumer.cs` - Consumer Kafka (herda de `BaseKafkaConsumer`)
- 📧 `WelcomeEmailTemplate.cs` - Template HTML personalizado

**Fluxo**: Kafka → Consumer → EmailService → Template → Azure Email

#### 💳 **Feature: PaymentProcessed** (`Features/PaymentProcessed/`)
**Responsabilidade**: Processar pagamentos e enviar emails de confirmação/falha

- 📋 `PaymentProcessedEvent.cs` - Contrato do evento Kafka
- 🔄 `PaymentProcessedConsumer.cs` - Consumer Kafka (herda de `BaseKafkaConsumer`)
- 📧 `PaymentProcessedEmailTemplate.cs` - Template HTML condicional (success/failure)

**Fluxo**: Kafka → Consumer → EmailService → Template → Azure Email

---

### Infraestrutura Compartilhada (Common)

Componentes reutilizáveis por todas as features:

#### 🔧 **Common/Abstractions**
- `BaseKafkaConsumer<TEvent>` - Classe base abstrata para todos os consumers
- `IEmailService` - Interface para serviço de email
- `IEmailTemplate` - Interface para templates de email
- `IKafkaEventHandler` - Interface para handlers de eventos

#### 📨 **Common/Services**
- `EmailService` - Implementação do serviço de envio de emails via Azure

#### ⚙️ **Common/Settings**
- `EmailSettings` - Configurações do Azure Communication Services
- `KafkaSettings` - Configurações do Apache Kafka

#### 🎨 **Common/EmailTemplates**
- `EmailTemplateBase` - Template HTML base com estilos e estrutura padrão

#### ⚠️ **Common/Exceptions**
- `EmailServiceException` - Exceção específica para falhas no envio de email
- `NotificationException` - Exceção geral de notificações

#### 🛡️ **Common/ExceptionHandlers**
- `GlobalExceptionHandler` - Tratamento centralizado de exceções

---

### Host Application

#### 🖥️ **Worker** (`FCG.Notifications.Worker`)
- `Program.cs` - Ponto de entrada e configuração do Host
- `appsettings.json` - Configurações de ambiente
- `Dockerfile` - Containerização

#### 🔌 **DependencyInjection**
- Registro de todos os consumers como Hosted Services
- Configuração de settings (Email, Kafka)
- Setup do Serilog

---

## 🛠️ Tecnologias e Bibliotecas

### Core Framework
- **.NET 8** - Framework principal
- **C# 12** - Linguagem de programação
- **Hosted Services** - Background Workers

### Mensageria
- **Apache Kafka** (`Confluent.Kafka 2.12.0`) - Event Streaming Platform
- **Event-Driven Architecture** - Arquitetura baseada em eventos

### Email
- **Azure Communication Services** (`Azure.Communication.Email 1.1.0`) - Envio de emails
- **Azure Core** (`1.50.0`) - Biblioteca base do Azure SDK

### Observabilidade
- **Serilog** (`4.3.0`) - Logging estruturado
- **Serilog.Sinks.Console** (`6.0.0`) - Output de logs no console
- **Serilog.Sinks.Seq** (`9.0.0`) - Centralização de logs no Seq
- **Serilog.Enrichers.Environment** (`3.0.1`) - Enriquecimento de logs com dados do ambiente
- **Serilog.Enrichers.Thread** (`4.0.0`) - Enriquecimento com informações de thread

### Configuração
- **Microsoft.Extensions.Configuration** (`9.0.10`)
- **Microsoft.Extensions.DependencyInjection** (`9.0.10`)
- **Microsoft.Extensions.Hosting** (`9.0.10`)

### Containerização
- **Docker** - Containerização da aplicação
- **Docker Compose** - Orquestração multi-container

---

## 📐 Regras de Negócio

### RN-NOT-001: Envio de Email de Boas-Vindas
✅ **Quando**: Ao consumir `UserCreatedEvent` do tópico `user-created`

📋 **Ações**:
- Gerar email com template "Welcome"
- Extrair dados do usuário do evento (Name, Email)
- Enviar email personalizado

📧 **Conteúdo do Email**:
- **Subject**: "Bem-vindo à Nossa Plataforma!"

### RN-NOT-002: Envio de Email de Confirmação de Compra
✅ **Quando**: Ao consumir `PaymentProcessedEvent` do tópico `payment-processed`

#### ✅ Cenário 1: Pagamento Aprovado (`Status = 'Approved'`)

📋 **Ações**:
- Gerar email com template "PurchaseConfirmation"
- Enviar email de confirmação

📧 **Conteúdo do Email**:
- **Subject**: "Pagamento Confirmado - Transação Bem-sucedida"

#### ❌ Cenário 2: Pagamento Rejeitado (`Status = 'Rejected'`)

📋 **Ações**:
- Gerar email com template "PurchaseRejected"
- Enviar email de notificação de falha

📧 **Conteúdo do Email**:
- **Subject**: "Pagamento Falhou - Ação Necessária"
---

## 📥 Eventos Consumidos

### UserCreatedEvent

**Tópico Kafka**: `user-created`

**Estrutura do Evento**:
```json
{
  "userId": "7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f",
  "name": "João Silva",
  "email": "joao@example.com",
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "createdAt": "2026-01-18T10:30:00Z"
}
```

**Ação**: 
- ✅ Enviar email de boas-vindas para o usuário
- ✅ Utilizar template `WelcomeEmailTemplate`
- ✅ Registrar log de processamento

---

### PaymentProcessedEvent

**Tópico Kafka**: `payment-processed`

**Estrutura do Evento**:
```json
{
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "paymentId": "8c1f3d4a-9e2b-4f6c-8d7e-5a6b7c8d9e0f",
  "userId": "7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f",
  "gameId": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
  "amount": 59.90,
  "status": "Approved",
  "processedAt": "2026-01-18T11:45:00Z"
}
```

**Ações**:
- ✅ Se `status == "Approved"`: Enviar email de confirmação de compra
- ✅ Se `status == "Rejected"`: Enviar email de falha no pagamento
- ✅ Utilizar template `PaymentProcessedEmailTemplate` (condicional)
- ✅ Registrar log de processamento

---

## ⚙️ Configuração e Execução

### Pré-requisitos

- ✅ **.NET 8 SDK** instalado
- ✅ **Docker** e **Docker Compose** instalados
- ✅ **Apache Kafka** (via Docker Compose)
- ✅ **Conta Azure Communication Services** com connectionString válida

### Configuração de Ambiente

#### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.Hosting.Lifetime": "Information",
      "FCG.Notifications": "Debug"
    }
  },
  "KafkaSettings": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "fcg-notifications-consumer-group",
    "Topics": {
      "UserCreated": "user-created",
      "PaymentProcessed": "payment-processed"
    }
  },
  "Email": {
    "ConnectionString": "endpoint=https://your-resource.communication.azure.com/;accesskey=YOUR_ACCESS_KEY",
    "SenderAddress": "DoNotReply@your-domain.com"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      }
    ]
  }
}
```

#### appsettings.Docker.json

```json
{
  "KafkaSettings": {
    "BootstrapServers": "kafka:29092"
  },
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://seq:5341"
        }
      }
    ]
  }
}
```

---

### 🐳 Executar com Docker Compose

#### 1. Configurar Azure Communication Services

Antes de executar, configure as credenciais do Azure no arquivo `appsettings.Docker.json`:

```json
"Email": {
  "ConnectionString": "endpoint=https://your-resource.communication.azure.com/;accesskey=YOUR_KEY",
  "SenderAddress": "DoNotReply@your-domain.com"
}
```

#### 2. Iniciar todos os serviços

```cmd
docker-compose up -d
```

Isso irá iniciar:
- 🐳 **Zookeeper**: Coordenação do Kafka (`localhost:2181`)
- 📨 **Kafka**: Message Broker (`localhost:9092`)
- 🎛️ **Kafka UI**: Interface web para Kafka (`http://localhost:8081`)
- 📊 **Seq**: Visualização de logs (`http://localhost:5341`)
- 📧 **FCG.Notifications**: Worker de notificações

#### 3. Verificar logs

**Visualizar logs do container**:
```cmd
docker logs fcg-notifications -f
```

**Acessar Seq (Logs Estruturados)**:
```
URL: http://localhost:5341
Usuário: admin
Senha: YourPassword123
```

#### 4. Monitorar Kafka

**Acessar Kafka UI**:
```
URL: http://localhost:8081
```

Você poderá:
- ✅ Visualizar tópicos (`user-created`, `payment-processed`)
- ✅ Verificar mensagens nos tópicos
- ✅ Monitorar consumer groups
- ✅ Ver offsets commitados

#### 5. Testar envio de eventos

**Publicar evento de teste no Kafka (via Kafka UI)**:

Acesse `http://localhost:8081` → Tópico `user-created` → Produce Message:

```json
{
  "userId": "test-123",
  "name": "Usuário Teste",
  "email": "teste@example.com",
  "correlationId": "corr-123",
  "createdAt": "2026-01-18T12:00:00Z"
}
```

Você verá nos logs do Seq o processamento do evento e o envio do email.

---