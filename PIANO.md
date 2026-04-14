# I mille modi di creare una applicazione reattiva su Azure

> **Global Azure 2026** — .NET 10 + Aspire + Azure Service Bus + Azure SignalR + Blazor

---

## Panoramica

Due demo progressive che mostrano come un'app reattiva evolve: da un semplice push real-time a un'architettura CQRS event-driven completa con Service Bus, read model e aggiornamenti live.

| # | App | Complessità | Concetti chiave |
|---|-----|-------------|-----------------|
| 1 | **ReactiveBoard** | ⭐ Semplice | SignalR + Background Service + Blazor |
| 2 | **ReactiveOrders** | ⭐⭐⭐ Avanzata | CQRS (Wolverine) + Service Bus queues/topics + SignalR + Read Model + Blazor + Azure Load Testing + App Service Autoscale |

---

## App 1 — ReactiveBoard (Semplice)

### Scenario
Una dashboard real-time per la conferenza: mostra il numero di partecipanti in sala, i talk in corso e i voti live del pubblico. Un background service simula gli eventi (check-in, voti, cambi sessione).

### Architettura

```
┌─────────────────┐     SignalR Hub      ┌──────────────────┐
│  Background      │ ──────────────────▶  │  Blazor Server   │
│  Service         │                      │  (Dashboard UI)  │
│  (Simulatore)    │                      └──────────────────┘
└─────────────────┘           ▲
                              │
                     Azure SignalR Service
                       (emulatore locale)
```

### Progetti nella solution

| Progetto | Tipo | Descrizione |
|----------|------|-------------|
| `ReactiveBoard.AppHost` | Aspire Host | Orchestrazione: SignalR emulator |
| `ReactiveBoard.ServiceDefaults` | Class Library | Defaults Aspire (logging, health, telemetry) |
| `ReactiveBoard.Web` | Blazor Server | Dashboard UI con grafici real-time |
| `ReactiveBoard.Simulator` | Worker Service | Background service che genera eventi simulati |

### Cosa si impara
- **Azure SignalR Service** con emulatore locale via Aspire
- **Blazor Server** con componenti reattivi (streaming rendering)
- **Background Service** come generatore di eventi
- **Aspire** come orchestratore e service discovery

### Dettagli implementativi
- Il `Simulator` invia messaggi all'Hub SignalR ogni 2-3 secondi (check-in random, voti random)
- La UI Blazor si aggiorna in tempo reale con `HubConnection`
- Usare `IHostedService` / `BackgroundService` per il simulatore
- Aspire gestisce il riferimento a Azure SignalR con `AddAzureSignalR()` (emulatore)

### Risorse Azure (tutte emulate localmente)
- **Azure SignalR Service** → emulatore via Aspire

---

## App 2 — ReactiveOrders (Avanzata)

### Scenario
Un sistema di ordini per il catering della conferenza con architettura CQRS completa. I partecipanti ordinano da Blazor, il comando viene gestito da **Wolverine** e pubblicato su **Azure Service Bus**. Gli eventi risultanti aggiornano read model ottimizzati (lista ordini, statistiche cucina, classifica piatti più ordinati). La UI Blazor legge dai read model e riceve aggiornamenti push via **SignalR**. Un background service simula ordini per popolare il sistema durante la demo.

### Framework CQRS: Wolverine

**Wolverine** è il framework scelto perché:
- Gestisce nativamente Command/Event handling
- Ha integrazione diretta con Azure Service Bus (`WolverineFx.AzureServiceBus`)
- Supporta outbox pattern per garanzia di consegna
- Si integra con Aspire
- Supporta saga/workflow per processi multi-step

### Architettura

```
                            COMMAND SIDE                              READ SIDE
                                                                        
┌──────────────┐  Command   ┌──────────────────┐  Service Bus   ┌──────────────────┐
│  Blazor SSR  │ ─────────▶ │  Command API     │ ─────────────▶ │  Event Processor │
│  (UI Ordini) │            │  (Wolverine)     │   Topic/Sub     │  (Wolverine)     │
└──────┬───────┘            │                  │                └────────┬─────────┘
       │                    │  Command Handler │                         │
       │                    │  → Validate      │                         │ Update
       │                    │  → Execute       │                         ▼
       │                    │  → Publish Event │                ┌──────────────────┐
       │                    └──────────────────┘                │  Read Models     │
       │                                                        │  (In-Memory /    │
       │  SignalR                                               │   EF Core + SQL) │
       ◀────────────────────────────────────────────────────────┤                  │
                                                                └──────────────────┘
       
       ┌─────────────────┐
       │  Background      │  Simula: ordini, conferme, annullamenti
       │  Simulator       │  → Invia comandi via Wolverine/ServiceBus
       └─────────────────┘
```

### Progetti nella solution

| Progetto | Tipo | Descrizione |
|----------|------|-------------|
| `ReactiveOrders.AppHost` | Aspire Host | Orchestrazione: Service Bus emulator + SignalR emulator + SQL |
| `ReactiveOrders.ServiceDefaults` | Class Library | Defaults Aspire |
| `ReactiveOrders.Web` | Blazor Server Interactive | UI: form ordini, lista ordini live, dashboard statistiche |
| `ReactiveOrders.CommandApi` | Minimal API + Wolverine | Riceve comandi HTTP, li gestisce, pubblica eventi su Service Bus |
| `ReactiveOrders.ReadApi` | Minimal API | Serve i read model via REST |
| `ReactiveOrders.EventProcessor` | Worker Service + Wolverine | Consuma eventi da Service Bus, aggiorna read model, notifica via SignalR |
| `ReactiveOrders.Domain` | Class Library | Aggregati, comandi, eventi |
| `ReactiveOrders.ReadModel` | Class Library | Read model entities + proiezioni |
| `ReactiveOrders.Contracts` | Class Library | Comandi ed eventi condivisi |
| `ReactiveOrders.Simulator` | Worker Service | Genera comandi simulati per la demo |

### Comandi ed Eventi

```
Commands:
  ├── PlaceOrder { Items[], AttendeeId, Notes }
  ├── ConfirmOrder { OrderId }
  ├── RejectOrder { OrderId, Reason }
  └── CancelOrder { OrderId }

Events:
  ├── OrderPlaced { OrderId, Items[], AttendeeId, Timestamp }
  ├── OrderConfirmed { OrderId, EstimatedReady, Timestamp }
  ├── OrderRejected { OrderId, Reason, Timestamp }
  └── OrderCancelled { OrderId, Timestamp }
```

### Read Models

| Read Model | Fonte eventi | Scopo |
|------------|-------------|-------|
| `OrderListView` | OrderPlaced, OrderConfirmed, OrderRejected, OrderCancelled | Lista ordini con stato corrente |
| `KitchenDashboardView` | OrderPlaced, OrderConfirmed | Ordini in coda per la cucina |
| `OrderStatsView` | Tutti | Dashboard: totale ordini, tempi medi, tasso conferma |
| `PopularItemsView` | OrderPlaced | Classifica piatti più ordinati |

### Cosa si impara
- **CQRS** con separazione netta command/read side
- **Wolverine** come message handler + command bus
- **Azure Service Bus** con emulatore locale (queue + topic/subscription)
- **Pattern Producer/Consumer** e **Pub/Sub** con topic per fan-out
- **Event-driven projections** per costruire read model
- **Outbox pattern** per consistenza tra write e publish
- **SignalR** per push dei read model aggiornati al client
- **Aspire** per orchestrare un sistema distribuito complesso

### Dettagli implementativi
- Wolverine gestisce `ICommandHandler<T>` e `IEventHandler<T>`
- I comandi arrivano via HTTP → Wolverine li esegue → pubblica eventi su Service Bus topic `order-events`
- L'`EventProcessor` consuma da Service Bus (subscriptions per read model), aggiorna i read model (EF Core + SQLite o in-memory), e push via SignalR
- La Blazor UI legge dai read model via `ReadApi` e si aggiorna in real-time via SignalR
- Il `Simulator` invia comandi casuali (`PlaceOrder` con item random, poi `ConfirmOrder`/`RejectOrder`) per popolare il sistema durante la demo
- Wolverine outbox con `AddAzureServiceBusTransport()` + `UseConventionalRouting()`
- Blazor UI mostra: form ordine → stato "in elaborazione" → conferma/rifiuto real-time + dashboard statistiche live

### Risorse Azure
- **Azure Service Bus** → emulatore via Aspire (`AddAzureServiceBus().RunAsEmulator()`) per dev locale
- **Azure SignalR Service** → emulatore via Aspire per dev locale
- **SQL Server** → container via Aspire (per read model persistenti)
- **Azure App Service** → deploy del CommandApi e ReadApi per la demo di scaling (richiede subscription Azure)
- **Azure Load Testing** → risorsa Azure per generare carico (richiede subscription Azure)

---

## Bonus Demo — Azure Load Testing + App Service Autoscale

### Scenario
Dopo aver mostrato l'architettura CQRS funzionante in locale, si deploya la App 2 su **Azure App Service** e si usa **Azure Load Testing** per generare un carico crescente di ordini. Si osserva come App Service scala automaticamente (scale-out) in risposta al traffico, e come il sistema reattivo (Service Bus + EventProcessor) assorbe il picco senza perdere messaggi.

### Architettura per la demo di scaling

```
┌──────────────────────┐
│  Azure Load Testing   │
│  (JMeter script)      │
│  POST /orders ×N      │
└──────────┬───────────┘
           │ HTTP flood
           ▼
┌──────────────────────┐     Service Bus      ┌──────────────────┐
│  App Service          │ ──────────────────▶  │  Event Processor  │
│  CommandApi           │    (topic)           │  (App Service)    │
│  ┌─────┐ ┌─────┐     │                      └────────┬─────────┘
│  │ i1  │ │ i2  │ ... │  ◀── autoscale                │
│  └─────┘ └─────┘     │                               │ SignalR
└──────────────────────┘                               ▼
                                              ┌──────────────────┐
                                              │  Blazor Web       │
                                              │  (aggiornamenti   │
                                              │   live)           │
                                              └──────────────────┘
```

### Cosa si prepara

#### 1. Script JMeter per Azure Load Testing
Uno script `.jmx` che simula un carico crescente di ordini:
- **Ramp-up**: da 10 a 500 utenti virtuali in 5 minuti
- **Endpoint**: `POST /api/orders` con payload JSON random
- **Durata**: 5-10 minuti
- **Criteri di successo**: p95 latency < 2s, error rate < 1%

```
loadtests/
├── orders-load-test.jmx          # Script JMeter
├── orders-load-test.yaml         # Config Azure Load Testing
└── test-data/
    └── orders-payload.csv        # Payload variabili per i test
```

#### 2. Configurazione App Service Autoscale
- **Regola scale-out**: CPU > 70% per 5 min → aggiungi 1 istanza (max 5)
- **Regola scale-in**: CPU < 30% per 10 min → rimuovi 1 istanza (min 1)
- **Piano**: Standard S1 o superiore (richiesto per autoscale)
- Configurabile via Bicep/ARM nel progetto Aspire o separatamente

#### 3. Azure Load Testing config (`orders-load-test.yaml`)
```yaml
version: v0.1
testId: reactive-orders-load-test
testName: ReactiveOrders Scale Test
testPlan: orders-load-test.jmx
engineInstances: 1
configurationFiles:
  - test-data/orders-payload.csv
failureCriteria:
  - avg(response_time_ms) > 2000
  - percentage(error) > 1
```

### Flusso della demo di scaling

1. **Mostra stato iniziale**: 1 istanza App Service, dashboard Aspire/Azure Monitor
2. **Lancia il load test** da Azure Portal o CLI (`az load test run`)
3. **Osserva in real-time**:
   - Azure Load Testing dashboard: throughput, latency, errori
   - App Service → Metriche: CPU%, request count, instance count
   - Blazor UI: ordini che arrivano a raffica, read model che si aggiornano
   - Service Bus: message count in coda (vedi se il consumer tiene il passo)
4. **Scale-out in azione**: mostrare il momento in cui nuove istanze si accendono
5. **Dopo il test**: scale-in automatico, nessun messaggio perso grazie a Service Bus

### Cosa si impara (bonus)
- **Azure Load Testing** come servizio gestito per test di carico
- **App Service Autoscale** basato su metriche (CPU, HTTP queue)
- **Resilienza del pattern reattivo**: Service Bus funge da buffer, nessun messaggio perso durante il scaling
- **Osservabilità sotto carico**: come leggere metriche e capire i colli di bottiglia
- La differenza tra **scalare l'API** (stateless, facile) e **scalare il consumer** (competing consumers su Service Bus)

### Note pratiche
- Questa parte richiede una **subscription Azure** attiva (non emulabile in locale)
- Preparare le risorse Azure in anticipo con un deploy Bicep/azd
- Il load test dura pochi minuti → costi minimi (~pochi centesimi)
- Si può usare `az load test` CLI per lanciarlo da terminale durante la presentazione

---

## Stack tecnologico comune

| Tecnologia | Versione | Uso |
|------------|----------|-----|
| .NET | 10 | Runtime |
| Aspire | 9.x+ | Orchestrazione, service discovery, telemetria |
| Blazor | .NET 10 | UI (Server + Interactive) |
| Azure Service Bus | Emulatore | Messaging (queue + topic) |
| Azure SignalR | Emulatore | Real-time push |
| Wolverine | 3.x | CQRS + messaging (App 2) |
| EF Core | 10 | Persistenza read model (App 2) |
| Azure Load Testing | Servizio gestito | Test di carico (Bonus) |
| Azure App Service | Standard S1+ | Deploy + autoscale (Bonus) |
| Apache JMeter | 5.x | Script di test (usato da Azure Load Testing) |

## Struttura cartelle del repository

```
GlobalAzure2026/
├── PIANO.md
├── src/
│   ├── 01-ReactiveBoard/
│   │   ├── ReactiveBoard.sln
│   │   ├── ReactiveBoard.AppHost/
│   │   ├── ReactiveBoard.ServiceDefaults/
│   │   ├── ReactiveBoard.Web/
│   │   └── ReactiveBoard.Simulator/
│   └── 02-ReactiveOrders/
│       ├── ReactiveOrders.sln
│       ├── ReactiveOrders.AppHost/
│       ├── ReactiveOrders.ServiceDefaults/
│       ├── ReactiveOrders.Web/
│       ├── ReactiveOrders.CommandApi/
│       ├── ReactiveOrders.ReadApi/
│       ├── ReactiveOrders.EventProcessor/
│       ├── ReactiveOrders.Domain/
│       ├── ReactiveOrders.ReadModel/
│       ├── ReactiveOrders.Contracts/
│       └── ReactiveOrders.Simulator/
├── loadtests/
│   ├── orders-load-test.jmx
│   ├── orders-load-test.yaml
│   └── test-data/
│       └── orders-payload.csv
├── infra/
│   └── main.bicep                # App Service + Autoscale + Load Testing
└── slides/
```

## Flow della presentazione

1. **Intro** (5 min) — Cos'è un'app reattiva? Non solo real-time UI.
2. **Demo 1 — ReactiveBoard** (10 min) — "Il modo più semplice: push diretto con SignalR"
3. **Demo 2 — ReactiveOrders** (20 min) — "Il pattern completo: CQRS + Service Bus + eventi + read model + push live"
4. **Bonus — Load Testing + Autoscale** (10 min) — "Quanto regge? Scaliamo su Azure sotto carico"
5. **Confronto e recap** (5 min) — Quando usare cosa, trade-off, prossimi passi

## Note per le demo

- Tutte le risorse Azure girano in locale grazie agli **emulatori Aspire** → zero costi, zero dipendenze cloud
- Aspire Dashboard mostra **traces, logs, metriche** di tutto il sistema → ottimo per spiegare l'osservabilità
- Il **Simulator** permette di generare carico a piacere durante la demo
- Preparare dei **breakpoint** strategici per mostrare il flusso dei messaggi
- Per la demo di **Load Testing**: preparare le risorse Azure ~30 min prima, lanciare un test di riscaldamento per avere le istanze pronte
- Avere la **dashboard Azure Portal** aperta su: App Service metrics, Load Testing results, Service Bus metrics
