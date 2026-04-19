# I mille modi di creare una applicazione reattiva su Azure

> **Global Azure 2026** — Sessione e demo su come costruire applicazioni reattive moderne con **.NET 10**, **.NET Aspire**, **Azure Service Bus**, **Azure SignalR** e **Blazor**.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-13.x-5C2D91)](https://learn.microsoft.com/dotnet/aspire/)
[![Azure](https://img.shields.io/badge/Azure-Service%20Bus%20%7C%20SignalR-0078D4?logo=microsoftazure)](https://azure.microsoft.com/)

---

## 📖 Di cosa parla la sessione

Cosa significa davvero "applicazione reattiva"? Non solo una UI che si aggiorna in tempo reale, ma un intero sistema che reagisce agli **eventi**: utenti, servizi, comandi, code di messaggi.

Questa sessione mostra **due demo progressive** che fanno vedere come un'app reattiva evolve passo dopo passo:

1. Da un semplice **push real-time** con SignalR…
2. …fino ad un'architettura **CQRS event-driven completa** con Azure Service Bus, read model proiettati dagli eventi e aggiornamenti live verso il client.

Come bonus si vede anche come questa architettura **scala su Azure** sotto carico, usando **Azure Load Testing** e l'**autoscale di App Service**.

| # | Applicazione | Complessità | Concetti chiave |
|---|--------------|-------------|-----------------|
| 1 | **ReactiveBoard** | ⭐ Semplice | SignalR + Background Service + Blazor |
| 2 | **ReactiveOrders** | ⭐⭐⭐ Avanzata | CQRS (Wolverine) + Service Bus queue/topic + SignalR + Read Model + Blazor + Azure Load Testing + App Service Autoscale |

> 💡 Tutte le risorse Azure usate nelle demo girano in locale tramite gli **emulatori orchestrati da Aspire**: zero costi, zero dipendenze cloud per provarle.

---

## 🗂️ Struttura del repository

```
GlobalAzure2026/
├── README.md                       # Questo file
├── PIANO.md                        # Piano di dettaglio della sessione e delle demo
├── GlobalAzure2026.sln             # Solution principale
├── slides/                         # Slide della sessione
└── src/
    ├── 01-ReactiveBoard/           # Demo 1 — push real-time semplice
    │   ├── ReactiveBoard.sln
    │   ├── ReactiveBoard.AppHost/          # Aspire host (orchestrazione)
    │   ├── ReactiveBoard.ServiceDefaults/  # Defaults Aspire (logging, health, telemetry)
    │   ├── ReactiveBoard.Web/              # Blazor Server (dashboard UI)
    │   └── ReactiveBoard.Simulator/        # Worker che genera eventi
    └── 02-ReactiveOrders/          # Demo 2 — CQRS event-driven completa
        ├── ReactiveOrders.sln
        ├── ReactiveOrders.AppHost/
        ├── ReactiveOrders.ServiceDefaults/
        ├── ReactiveOrders.Web/             # Blazor Server Interactive
        ├── ReactiveOrders.CommandApi/      # Minimal API + Wolverine (write side)
        ├── ReactiveOrders.ReadApi/         # Minimal API (read side)
        ├── ReactiveOrders.EventProcessor/  # Worker + Wolverine (proiezioni)
        ├── ReactiveOrders.Domain/          # Aggregati, comandi, eventi
        ├── ReactiveOrders.ReadModel/       # Entità read model + proiezioni
        ├── ReactiveOrders.Contracts/       # Contratti condivisi
        └── ReactiveOrders.Simulator/       # Worker che genera comandi simulati
```

Per il dettaglio completo di scenario, architettura, comandi/eventi e read model di ogni demo vedi **[`PIANO.md`](./PIANO.md)**.

---

## 🎯 Demo 1 — ReactiveBoard

> *"Il modo più semplice per fare reattività: push diretto con SignalR."*

Una **dashboard real-time** per la conferenza che mostra il numero di partecipanti in sala, i talk in corso e i voti live del pubblico. Un background service simula gli eventi (check-in, voti, cambi sessione) e li spinge in tempo reale verso la UI Blazor tramite Azure SignalR.

```
┌─────────────────┐     SignalR Hub      ┌──────────────────┐
│  Background     │ ──────────────────▶  │  Blazor Server   │
│  Service        │                      │  (Dashboard UI)  │
│  (Simulatore)   │                      └──────────────────┘
└─────────────────┘           ▲
                              │
                     Azure SignalR Service
                       (emulatore locale)
```

**Cosa si impara**

- Azure SignalR Service con **emulatore locale via Aspire**
- **Blazor Server** con componenti reattivi (streaming rendering)
- **Background Service** come generatore di eventi
- **Aspire** come orchestratore e service discovery

---

## 🎯 Demo 2 — ReactiveOrders

> *"Il pattern completo: CQRS + Service Bus + eventi + read model + push live."*

Un **sistema di ordini** per il catering della conferenza. I partecipanti ordinano da Blazor, il comando viene gestito da **Wolverine** e pubblicato su **Azure Service Bus**. Gli eventi alimentano read model ottimizzati (lista ordini, dashboard cucina, statistiche, classifica piatti più ordinati). La UI Blazor legge dai read model e riceve aggiornamenti push via **SignalR**. Un simulatore genera ordini per popolare il sistema durante la demo.

```
                            COMMAND SIDE                              READ SIDE

┌──────────────┐  Command   ┌──────────────────┐  Service Bus   ┌──────────────────┐
│  Blazor SSR  │ ─────────▶ │  Command API     │ ─────────────▶ │  Event Processor │
│  (UI Ordini) │            │  (Wolverine)     │   Topic/Sub    │  (Wolverine)     │
└──────┬───────┘            └──────────────────┘                └────────┬─────────┘
       │                                                                 │ Update
       │                                                                 ▼
       │                                                        ┌──────────────────┐
       │                                                        │  Read Models     │
       │  SignalR                                               │  (EF Core + SQL) │
       ◀────────────────────────────────────────────────────────┤                  │
                                                                └──────────────────┘
```

**Cosa si impara**

- **CQRS** con separazione netta command/read side
- **Wolverine** come message handler + command bus
- **Azure Service Bus** con emulatore locale (queue + topic/subscription)
- Pattern **Producer/Consumer** e **Pub/Sub** con topic per fan-out
- **Event-driven projections** per costruire i read model
- **Outbox pattern** per la consistenza tra write e publish
- **SignalR** per il push dei read model aggiornati al client
- **Aspire** per orchestrare un sistema distribuito complesso

---

## 🚀 Bonus — Azure Load Testing + App Service Autoscale

Dopo aver mostrato l'architettura CQRS funzionante in locale, la Demo 2 viene deployata su **Azure App Service** e **Azure Load Testing** genera un carico crescente di ordini. Si osserva:

- come App Service **scala automaticamente** in risposta al traffico,
- come il sistema reattivo (Service Bus + EventProcessor) **assorbe il picco senza perdere messaggi**,
- la differenza tra **scalare l'API** (stateless, semplice) e **scalare il consumer** (competing consumers su Service Bus).

> ℹ️ Questa parte richiede una subscription Azure attiva. Costi dell'esercizio: pochi centesimi per test.

---

## 🧰 Stack tecnologico

| Tecnologia | Versione | Uso |
|------------|----------|-----|
| .NET | 10 | Runtime |
| .NET Aspire | 9.x+ | Orchestrazione, service discovery, telemetria |
| Blazor | .NET 10 | UI (Server + Interactive) |
| Azure Service Bus | Emulatore | Messaging (queue + topic) |
| Azure SignalR | Emulatore | Real-time push |
| Wolverine | 3.x | CQRS + messaging (Demo 2) |
| EF Core | 10 | Persistenza read model (Demo 2) |
| Azure Load Testing | Servizio gestito | Test di carico (Bonus) |
| Azure App Service | Standard S1+ | Deploy + autoscale (Bonus) |
| Apache JMeter | 5.x | Script di test usati da Azure Load Testing |

---

## ▶️ Come eseguire le demo in locale

### Prerequisiti

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [.NET Aspire workload](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (richiesto dagli emulatori di Service Bus / SignalR e da SQL Server in container)

### Demo 1 — ReactiveBoard

```bash
cd src/01-ReactiveBoard
dotnet run --project ReactiveBoard.AppHost
```

Apri la **Aspire Dashboard** (URL stampato in console) per vedere log, traces e metriche, e seguire il link al progetto `ReactiveBoard.Web` per la dashboard live.

### Demo 2 — ReactiveOrders

```bash
cd src/02-ReactiveOrders
dotnet run --project ReactiveOrders.AppHost
```

Aspire avvia: emulatore Service Bus, emulatore SignalR, SQL Server, CommandApi, ReadApi, EventProcessor, Web e Simulator. Dalla dashboard apri `ReactiveOrders.Web` per vedere ordini e dashboard cucina aggiornarsi in tempo reale mentre il simulatore inietta comandi.

---

## 🎤 Flow della presentazione

1. **Intro** (5 min) — Cos'è davvero un'app reattiva? (Spoiler: non solo real-time UI.)
2. **Demo 1 — ReactiveBoard** (10 min) — *Il modo più semplice: push diretto con SignalR.*
3. **Demo 2 — ReactiveOrders** (20 min) — *Il pattern completo: CQRS + Service Bus + eventi + read model + push live.*
4. **Bonus — Load Testing + Autoscale** (10 min) — *Quanto regge? Scaliamo su Azure sotto carico.*
5. **Confronto e recap** (5 min) — Quando usare cosa, trade-off, prossimi passi.

---

## 📚 Risorse

- [`PIANO.md`](./PIANO.md) — Piano di dettaglio (scenari, architettura, comandi/eventi, read model, configurazioni)
- [`slides/`](./slides/) — Slide della sessione
- [Global Azure](https://globalazure.net/) — Community event globale
- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/)
- [Wolverine](https://wolverine.netlify.app/)
- [Azure Service Bus](https://learn.microsoft.com/azure/service-bus-messaging/)
- [Azure SignalR Service](https://learn.microsoft.com/azure/azure-signalr/)

---

## 👤 Autore

Sessione preparata per **Global Azure 2026** da [@andreatosato](https://github.com/andreatosato).
