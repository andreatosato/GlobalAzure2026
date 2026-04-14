using ShapeCrawler;

// ── Create presentation with 23 slides ──
var pres = new Presentation(p =>
{
    for (int i = 0; i < 23; i++) p.Slide();
});

// ── Color Palette ──
const string AzureBlue = "0078D4";
const string DarkBlue = "003B73";
const string White = "FFFFFF";
const string LightGray = "F3F3F3";
const string AccentGreen = "00B294";
const string AccentOrange = "FF8C00";
const string TextDark = "1A1A1A";
const string TextMedium = "4A4A4A";
const string CodeBg = "1E1E1E";
const string CodeComment = "6A9955";
const string CodeVar = "9CDCFE";
const string CodeStr = "CE9178";
const string LightBlue = "B4D6F5";
const string LightOrange = "FFF3E0";
const string Gray = "AAAAAA";
const string MedGray = "888888";

// ── Slide dimensions (approx widescreen points) ──
const int SW = 960; // slide width
const int SH = 540; // slide height
const int MX = 40;  // margin x
const int MY = 30;  // margin y

// ── Helpers ──
IShape Box(IUserSlide slide, int x, int y, int w, int h, string fill, string text, int fontSize, string fontColor, bool bold = false)
{
    slide.Shapes.AddShape(x, y, w, h, Geometry.Rectangle, text);
    var s = slide.Shapes.Last();
    s.Fill!.SetColor(fill);
    var f = s.TextBox!.Paragraphs[0].Portions[0].Font!;
    f.Size = fontSize;
    f.Color.Set(fontColor);
    f.IsBold = bold;
    return s;
}

void Line(IShape s, string text, int fontSize, string fontColor, bool bold = false)
{
    s.TextBox!.Paragraphs.Add();
    var p = s.TextBox!.Paragraphs.Last();
    p.Text = text;
    p.Portions[0].Font!.Size = fontSize;
    p.Portions[0].Font.Color.Set(fontColor);
    p.Portions[0].Font.IsBold = bold;
}

void Bg(IUserSlide slide, string color) => Box(slide, 0, 0, SW, SH, color, " ", 1, color);
void TopBar(IUserSlide slide) => Box(slide, 0, 0, SW, 6, AzureBlue, " ", 1, AzureBlue);

void Footer(IUserSlide slide, string num)
{
    var f = Box(slide, 15, SH - 30, 500, 20, White, "Global Azure 2026", 8, TextMedium);
    var n = Box(slide, SW - 60, SH - 30, 50, 20, White, num, 8, TextMedium);
}

void FooterDark(IUserSlide slide, string bg, string num)
{
    var f = Box(slide, 15, SH - 30, 500, 20, bg, "Global Azure 2026", 8, Gray);
    var n = Box(slide, SW - 60, SH - 30, 50, 20, bg, num, 8, Gray);
}

// ==================================================
// SLIDE 1: Title
// ==================================================
{
    var sl = pres.Slide(1);
    Bg(sl, DarkBlue);
    Box(sl, 0, 0, SW, 8, AzureBlue, " ", 1, AzureBlue);

    var t = Box(sl, MX, 120, 880, 50, DarkBlue, "I mille modi di creare", 36, White, true);
    Line(t, "una applicazione reattiva su Azure", 36, White, true);

    Box(sl, MX, 260, 880, 30, DarkBlue, ".NET 10  •  Aspire  •  Service Bus  •  SignalR  •  Blazor  •  CQRS", 16, AccentGreen);
    Box(sl, MX, 340, 880, 30, DarkBlue, "Global Azure 2026", 24, White, true);

    var b = Box(sl, MX, 410, 880, 30, DarkBlue, "Un'app reattiva non e' solo \"real-time UI\": e' un sistema che reagisce a eventi,", 11, Gray);
    Line(b, "scala sotto carico e resta osservabile.", 11, Gray);
}

// ==================================================
// SLIDE 2: Agenda
// ==================================================
{
    var sl = pres.Slide(2);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Agenda", 30, DarkBlue, true);

    var b = Box(sl, MX, 90, 880, 400, White, "1.  Cos'e' un'app reattiva?", 18, TextDark, true);
    Line(b, "", 10, TextDark);
    Line(b, "2.  Il nostro stack: .NET 10, Aspire, Servizi Azure", 18, TextDark, true);
    Line(b, "", 10, TextDark);
    Line(b, "3.  I servizi Azure che utilizziamo", 18, TextDark, true);
    Line(b, "     Azure SignalR  •  Azure Service Bus  •  App Service  •  Load Testing", 13, TextMedium);
    Line(b, "", 10, TextDark);
    Line(b, "4.  Demo 1 — ReactiveBoard (semplice)", 18, AzureBlue, true);
    Line(b, "     Push diretto con SignalR + Background Service", 13, TextMedium);
    Line(b, "", 10, TextDark);
    Line(b, "5.  Demo 2 — ReactiveOrders (avanzata)", 18, AzureBlue, true);
    Line(b, "     CQRS + Service Bus + Read Model + Wolverine", 13, TextMedium);
    Line(b, "", 10, TextDark);
    Line(b, "6.  Bonus — Load Testing + App Service Autoscale", 18, AccentOrange, true);
    Line(b, "", 10, TextDark);
    Line(b, "7.  Recap & Q/A", 18, TextDark, true);
    Footer(sl, "2");
}

// ==================================================
// SLIDE 3: Cos'è un'app reattiva?
// ==================================================
{
    var sl = pres.Slide(3);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Cos'e' un'app reattiva?", 30, DarkBlue, true);
    Box(sl, MX, 75, 880, 25, White, "Non e' solo \"real-time UI\" — e' molto di piu'", 14, TextMedium);

    // 4 pillars
    var b1 = Box(sl, MX, 115, 200, 140, LightGray, "Responsive", 16, AzureBlue, true);
    Line(b1, "", 6, TextDark);
    Line(b1, "Risponde rapidamente", 11, TextDark);
    Line(b1, "agli utenti con", 11, TextDark);
    Line(b1, "feedback immediato", 11, TextDark);

    var b2 = Box(sl, 260, 115, 200, 140, LightGray, "Message-Driven", 16, AzureBlue, true);
    Line(b2, "", 6, TextDark);
    Line(b2, "Comunicazione asincrona", 11, TextDark);
    Line(b2, "tramite messaggi ed", 11, TextDark);
    Line(b2, "eventi disaccoppiati", 11, TextDark);

    var b3 = Box(sl, 480, 115, 200, 140, LightGray, "Elastic", 16, AzureBlue, true);
    Line(b3, "", 6, TextDark);
    Line(b3, "Scala sotto carico,", 11, TextDark);
    Line(b3, "autoscale automatico", 11, TextDark);
    Line(b3, "delle risorse", 11, TextDark);

    var b4 = Box(sl, 700, 115, 200, 140, LightGray, "Observable", 16, AzureBlue, true);
    Line(b4, "", 6, TextDark);
    Line(b4, "Traces, metriche, log", 11, TextDark);
    Line(b4, "distribuiti per capire", 11, TextDark);
    Line(b4, "cosa succede", 11, TextDark);

    var bb = Box(sl, MX, 280, 880, 200, White, "Oggi vedremo come combinare:", 14, TextDark, true);
    Line(bb, "", 6, TextDark);
    Line(bb, "  •  Azure SignalR -> aggiornamenti live verso web/mobile", 12, TextDark);
    Line(bb, "  •  Azure Service Bus -> comandi/eventi asincroni affidabili", 12, TextDark);
    Line(bb, "  •  CQRS + Read Models -> dati ottimizzati per la lettura", 12, TextDark);
    Line(bb, "  •  Azure Load Testing + Autoscale -> verificare e scalare sotto carico", 12, TextDark);
    Footer(sl, "3");
}

// ==================================================
// SLIDE 4: Stack Tecnologico
// ==================================================
{
    var sl = pres.Slide(4);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Il nostro stack tecnologico", 30, DarkBlue, true);

    var c1 = Box(sl, MX, 90, 270, 220, LightGray, "Runtime & Framework", 16, AzureBlue, true);
    Line(c1, "", 6, TextDark);
    Line(c1, "  •  .NET 10", 12, TextDark);
    Line(c1, "  •  Blazor (Server + Interactive)", 12, TextDark);
    Line(c1, "  •  EF Core 10", 12, TextDark);
    Line(c1, "  •  Wolverine 3.x (CQRS)", 12, TextDark);

    var c2 = Box(sl, 340, 90, 270, 220, LightGray, "Orchestrazione", 16, AzureBlue, true);
    Line(c2, "", 6, TextDark);
    Line(c2, "  •  .NET Aspire 9.x+", 12, TextDark);
    Line(c2, "  •  Service Discovery", 12, TextDark);
    Line(c2, "  •  OpenTelemetry integrata", 12, TextDark);
    Line(c2, "  •  Emulatori Azure locali", 12, TextDark);

    var c3 = Box(sl, 640, 90, 270, 220, LightGray, "Servizi Azure", 16, AzureBlue, true);
    Line(c3, "", 6, TextDark);
    Line(c3, "  •  Azure SignalR Service", 12, TextDark);
    Line(c3, "  •  Azure Service Bus", 12, TextDark);
    Line(c3, "  •  Azure App Service", 12, TextDark);
    Line(c3, "  •  Azure Load Testing", 12, TextDark);

    var n = Box(sl, MX, 340, 880, 50, White, "Tutto gira in locale con emulatori Aspire — zero costi, zero subscription", 14, AccentGreen, true);
    Line(n, "(tranne la parte di Load Testing + Autoscale, che richiede Azure)", 11, TextMedium);
    Footer(sl, "4");
}

// ==================================================
// SLIDE 5: .NET Aspire
// ==================================================
{
    var sl = pres.Slide(5);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, ".NET Aspire — L'orchestratore", 30, DarkBlue, true);

    var left = Box(sl, MX, 90, 430, 400, White, "Cos'e' .NET Aspire?", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "Un framework per costruire app cloud-native", 12, TextDark);
    Line(left, "distribuite, osservabili e pronte per la produzione.", 12, TextDark);
    Line(left, "", 12, TextDark);
    Line(left, "Perche' lo usiamo?", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "  •  AppHost orchestra tutti i servizi", 12, TextDark);
    Line(left, "  •  ServiceDefaults: health, logging, tracing", 12, TextDark);
    Line(left, "  •  Service Discovery automatico", 12, TextDark);
    Line(left, "  •  Emulatori Azure integrati", 12, TextDark);
    Line(left, "  •  Dashboard con traces e metriche", 12, TextDark);

    var code = Box(sl, 500, 90, 420, 280, CodeBg, "// AppHost — Program.cs", 11, CodeComment);
    Line(code, "", 6, White);
    Line(code, "var signalr = builder", 11, CodeVar);
    Line(code, "    .AddAzureSignalR(\"signalr\")", 11, CodeStr);
    Line(code, "    .RunAsEmulator();", 11, CodeStr);
    Line(code, "", 6, White);
    Line(code, "var bus = builder", 11, CodeVar);
    Line(code, "    .AddAzureServiceBus(\"bus\")", 11, CodeStr);
    Line(code, "    .RunAsEmulator();", 11, CodeStr);
    Line(code, "", 6, White);
    Line(code, "builder.AddProject<Web>(\"web\")", 11, CodeVar);
    Line(code, "    .WithReference(signalr);", 11, CodeStr);
    Footer(sl, "5");
}

// ==================================================
// SLIDE 6: Azure SignalR Service
// ==================================================
{
    var sl = pres.Slide(6);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Azure SignalR Service", 30, DarkBlue, true);
    Box(sl, MX, 72, 880, 20, White, "Real-time push senza gestire infrastruttura WebSocket", 14, TextMedium);

    var left = Box(sl, MX, 105, 430, 380, White, "Cos'e'?", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "Servizio gestito che scala connessioni WebSocket,", 12, TextDark);
    Line(left, "Server-Sent Events e Long Polling per applicazioni", 12, TextDark);
    Line(left, "real-time. Basato sul protocollo SignalR di ASP.NET.", 12, TextDark);
    Line(left, "", 12, TextDark);
    Line(left, "Punti di forza", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "  •  Scala fino a milioni di connessioni", 12, TextDark);
    Line(left, "  •  Zero infrastruttura da gestire", 12, TextDark);
    Line(left, "  •  Supporta Hub/Group/User targeting", 12, TextDark);
    Line(left, "  •  Latenza sub-secondo per gli update", 12, TextDark);
    Line(left, "  •  Modalita' Serverless o Default", 12, TextDark);

    var right = Box(sl, 500, 105, 420, 250, LightGray, "Come lo usiamo noi", 16, DarkBlue, true);
    Line(right, "", 6, TextDark);
    Line(right, "Demo 1: il BackgroundService invia", 12, TextDark);
    Line(right, "eventi tramite Hub -> Blazor li mostra.", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Demo 2: l'EventProcessor aggiorna", 12, TextDark);
    Line(right, "i read model e notifica i client", 12, TextDark);
    Line(right, "Blazor connessi via SignalR.", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Emulatore locale via Aspire", 12, AccentGreen, true);
    Footer(sl, "6");
}

// ==================================================
// SLIDE 7: Azure Service Bus
// ==================================================
{
    var sl = pres.Slide(7);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Azure Service Bus", 30, DarkBlue, true);
    Box(sl, MX, 72, 880, 20, White, "Messaging enterprise affidabile con code, topic e subscription", 14, TextMedium);

    var left = Box(sl, MX, 105, 430, 400, White, "Cos'e'?", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "Message broker enterprise completamente gestito.", 12, TextDark);
    Line(left, "Supporta code (1:1) e topic/subscription (1:N).", 12, TextDark);
    Line(left, "Garantisce consegna, ordine, deduplicazione.", 12, TextDark);
    Line(left, "", 12, TextDark);
    Line(left, "Punti di forza", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "  •  Garanzia FIFO e at-least-once delivery", 12, TextDark);
    Line(left, "  •  Topic/Sub per pub-sub scalabile", 12, TextDark);
    Line(left, "  •  Dead-letter queue per errori", 12, TextDark);
    Line(left, "  •  Sessions per message grouping", 12, TextDark);
    Line(left, "  •  Scheduled messages e TTL", 12, TextDark);
    Line(left, "  •  Competing consumers per scaling", 12, TextDark);

    var right = Box(sl, 500, 105, 420, 280, LightGray, "Come lo usiamo noi", 16, DarkBlue, true);
    Line(right, "", 6, TextDark);
    Line(right, "Queue: comandi PlaceOrder inviati", 12, TextDark);
    Line(right, "dall'API al command handler.", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Topic \"order-events\": eventi pubblicati", 12, TextDark);
    Line(right, "dal command side, consumati da N", 12, TextDark);
    Line(right, "subscription (read model diversi).", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Wolverine usa il trasporto Service Bus", 12, TextDark);
    Line(right, "con outbox pattern integrato.", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Emulatore locale via Aspire", 12, AccentGreen, true);
    Footer(sl, "7");
}

// ==================================================
// SLIDE 8: Azure App Service + Autoscale
// ==================================================
{
    var sl = pres.Slide(8);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Azure App Service + Autoscale", 30, DarkBlue, true);
    Box(sl, MX, 72, 880, 20, White, "PaaS con scaling automatico basato su metriche", 14, TextMedium);

    var left = Box(sl, MX, 105, 430, 400, White, "Azure App Service", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "  •  PaaS per web app, API, worker", 12, TextDark);
    Line(left, "  •  Deploy continuo (GitHub, Azure DevOps)", 12, TextDark);
    Line(left, "  •  Slot di staging per zero-downtime", 12, TextDark);
    Line(left, "  •  Custom domains, TLS automatico", 12, TextDark);
    Line(left, "", 12, TextDark);
    Line(left, "Autoscale", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "  •  Scale-out su CPU, memoria, HTTP queue", 12, TextDark);
    Line(left, "  •  Regole personalizzabili con soglie", 12, TextDark);
    Line(left, "  •  Scale-in automatico quando il carico cala", 12, TextDark);
    Line(left, "  •  Richiede piano Standard (S1) o superiore", 12, TextDark);

    var right = Box(sl, 500, 105, 420, 250, LightGray, "Config Autoscale per la demo", 16, DarkBlue, true);
    Line(right, "", 6, TextDark);
    Line(right, "Scale-out:", 13, AccentOrange, true);
    Line(right, "  CPU > 70% per 5 min -> +1 istanza", 12, TextDark);
    Line(right, "  Max 5 istanze", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Scale-in:", 13, AccentGreen, true);
    Line(right, "  CPU < 30% per 10 min -> -1 istanza", 12, TextDark);
    Line(right, "  Min 1 istanza", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Service Bus funge da buffer durante", 12, TextDark);
    Line(right, "lo scaling: nessun messaggio perso!", 12, TextDark);
    Footer(sl, "8");
}

// ==================================================
// SLIDE 9: Azure Load Testing
// ==================================================
{
    var sl = pres.Slide(9);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Azure Load Testing", 30, DarkBlue, true);
    Box(sl, MX, 72, 880, 20, White, "Test di carico gestito, basato su Apache JMeter", 14, TextMedium);

    var left = Box(sl, MX, 105, 430, 380, White, "Cos'e'?", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "Servizio completamente gestito per eseguire", 12, TextDark);
    Line(left, "test di carico su larga scala. Usa script JMeter.", 12, TextDark);
    Line(left, "", 12, TextDark);
    Line(left, "Punti di forza", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "  •  Nessuna infrastruttura da gestire", 12, TextDark);
    Line(left, "  •  Scala a migliaia di utenti virtuali", 12, TextDark);
    Line(left, "  •  Dashboard real-time con metriche", 12, TextDark);
    Line(left, "  •  Criteri di successo/fallimento", 12, TextDark);
    Line(left, "  •  Integrazione CI/CD (GitHub Actions)", 12, TextDark);
    Line(left, "  •  Confronto tra test run", 12, TextDark);

    var right = Box(sl, 500, 105, 420, 280, LightGray, "Il nostro test", 16, DarkBlue, true);
    Line(right, "", 6, TextDark);
    Line(right, "Script JMeter:", 13, AccentOrange, true);
    Line(right, "  POST /api/orders con payload random", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Ramp-up:", 13, AccentOrange, true);
    Line(right, "  10 -> 500 utenti virtuali in 5 min", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Criteri di successo:", 13, AccentOrange, true);
    Line(right, "  p95 latency < 2s", 12, TextDark);
    Line(right, "  error rate < 1%", 12, TextDark);
    Line(right, "", 8, TextDark);
    Line(right, "Costi: pochi centesimi per un run", 12, AccentGreen, true);
    Footer(sl, "9");
}

// ==================================================
// SLIDE 10: DEMO 1 — ReactiveBoard — Intro
// ==================================================
{
    var sl = pres.Slide(10);
    Bg(sl, AzureBlue);
    Box(sl, MX, 40, 300, 25, AzureBlue, "DEMO 1", 13, AccentGreen, true);
    Box(sl, MX, 80, 880, 50, AzureBlue, "ReactiveBoard", 38, White, true);
    Box(sl, MX, 155, 880, 30, AzureBlue, "Il modo piu' semplice: push diretto con SignalR", 18, LightBlue);

    var b = Box(sl, MX, 220, 880, 280, AzureBlue, "Scenario", 20, White, true);
    Line(b, "", 6, White);
    Line(b, "Dashboard real-time della conferenza:", 14, White);
    Line(b, "", 6, White);
    Line(b, "  •  Numero partecipanti in sala", 14, White);
    Line(b, "  •  Talk in corso e prossimi", 14, White);
    Line(b, "  •  Voti live del pubblico", 14, White);
    Line(b, "", 10, White);
    Line(b, "Un BackgroundService simula gli eventi.", 14, LightBlue);
    Line(b, "La UI Blazor si aggiorna in tempo reale.", 14, LightBlue);
}

// ==================================================
// SLIDE 11: DEMO 1 — Architettura
// ==================================================
{
    var sl = pres.Slide(11);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Demo 1 — ReactiveBoard — Architettura", 26, DarkBlue, true);

    Box(sl, MX, 110, 220, 90, LightGray, "Simulator", 16, AzureBlue, true);
    Line((IShape)sl.Shapes.Last(), "", 4, TextDark);
    Line((IShape)sl.Shapes.Last(), "BackgroundService", 11, TextDark);
    Line((IShape)sl.Shapes.Last(), "Genera eventi ogni 2-3s", 11, TextDark);

    Box(sl, 290, 130, 160, 30, White, "---- SignalR Hub ---->", 11, AzureBlue, true);

    Box(sl, 470, 110, 220, 90, LightGray, "Blazor Server", 16, AzureBlue, true);
    Line((IShape)sl.Shapes.Last(), "", 4, TextDark);
    Line((IShape)sl.Shapes.Last(), "Dashboard UI", 11, TextDark);
    Line((IShape)sl.Shapes.Last(), "Grafici real-time", 11, TextDark);

    Box(sl, 230, 230, 260, 55, AzureBlue, "Azure SignalR Service", 14, White, true);
    Line((IShape)sl.Shapes.Last(), "(emulatore locale via Aspire)", 11, LightBlue);

    var p = Box(sl, MX, 320, 880, 170, White, "4 Progetti:", 14, DarkBlue, true);
    Line(p, "", 4, TextDark);
    Line(p, "  ReactiveBoard.AppHost            -> Aspire Host (orchestrazione)", 11, TextDark);
    Line(p, "  ReactiveBoard.ServiceDefaults     -> Defaults (health, tracing)", 11, TextDark);
    Line(p, "  ReactiveBoard.Web                 -> Blazor Server (dashboard)", 11, TextDark);
    Line(p, "  ReactiveBoard.Simulator           -> Worker Service (simulatore)", 11, TextDark);
    Footer(sl, "11");
}

// ==================================================
// SLIDE 12: DEMO 1 — Cosa si impara
// ==================================================
{
    var sl = pres.Slide(12);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Demo 1 — Cosa impariamo", 30, DarkBlue, true);

    var b = Box(sl, MX, 95, 880, 380, White, "Azure SignalR Service", 18, AzureBlue, true);
    Line(b, "     Emulatore locale, zero costi, push sub-secondo", 12, TextDark);
    Line(b, "", 10, TextDark);
    Line(b, "Blazor Server + Streaming Rendering", 18, AzureBlue, true);
    Line(b, "     Componenti reattivi con HubConnection", 12, TextDark);
    Line(b, "", 10, TextDark);
    Line(b, "BackgroundService come simulatore", 18, AzureBlue, true);
    Line(b, "     IHostedService che genera eventi a cadenza regolare", 12, TextDark);
    Line(b, "", 10, TextDark);
    Line(b, ".NET Aspire come orchestratore", 18, AzureBlue, true);
    Line(b, "     Service discovery, health checks, dashboard telemetria", 12, TextDark);
    Line(b, "", 16, TextDark);
    Line(b, "Semplicita': ~4 progetti, nessun message broker", 16, AccentGreen, true);
    Line(b, "Limite: accoppiamento diretto Simulator <-> Hub", 16, AccentOrange, true);
    Footer(sl, "12");
}

// ==================================================
// SLIDE 13: DEMO 2 — ReactiveOrders — Intro
// ==================================================
{
    var sl = pres.Slide(13);
    Bg(sl, DarkBlue);
    Box(sl, MX, 40, 300, 25, DarkBlue, "DEMO 2", 13, AccentOrange, true);
    Box(sl, MX, 80, 880, 50, DarkBlue, "ReactiveOrders", 38, White, true);
    Box(sl, MX, 155, 880, 30, DarkBlue, "CQRS + Service Bus + Read Model + SignalR", 18, AccentGreen);

    var b = Box(sl, MX, 220, 880, 280, DarkBlue, "Scenario", 20, White, true);
    Line(b, "", 6, White);
    Line(b, "Sistema ordini catering della conferenza:", 14, White);
    Line(b, "", 6, White);
    Line(b, "  •  I partecipanti ordinano da Blazor", 14, White);
    Line(b, "  •  Comandi gestiti da Wolverine via Service Bus", 14, White);
    Line(b, "  •  Eventi aggiornano read model ottimizzati", 14, White);
    Line(b, "  •  UI riceve push via SignalR", 14, White);
    Line(b, "", 10, White);
    Line(b, "Separazione completa: Command Side / Read Side", 14, AccentGreen, true);
}

// ==================================================
// SLIDE 14: CQRS Pattern con Wolverine
// ==================================================
{
    var sl = pres.Slide(14);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Pattern CQRS con Wolverine", 30, DarkBlue, true);

    var left = Box(sl, MX, 90, 430, 400, White, "Cos'e' CQRS?", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "Command Query Responsibility Segregation:", 12, TextDark);
    Line(left, "il modello di scrittura e' separato dal", 12, TextDark);
    Line(left, "modello di lettura.", 12, TextDark);
    Line(left, "", 10, TextDark);
    Line(left, "Perche' Wolverine?", 18, AzureBlue, true);
    Line(left, "", 6, TextDark);
    Line(left, "  •  Command/Event handling nativo", 12, TextDark);
    Line(left, "  •  Integrazione Azure Service Bus", 12, TextDark);
    Line(left, "  •  Outbox pattern built-in", 12, TextDark);
    Line(left, "  •  Saga/workflow per processi multi-step", 12, TextDark);
    Line(left, "  •  Si integra con Aspire", 12, TextDark);
    Line(left, "  •  NuGet: WolverineFx.AzureServiceBus", 12, TextMedium);

    var code = Box(sl, 500, 90, 420, 380, CodeBg, "// Command Handler", 11, CodeComment);
    Line(code, "", 4, White);
    Line(code, "public static OrderPlaced Handle(", 10, CodeVar);
    Line(code, "    PlaceOrder command,", 10, CodeVar);
    Line(code, "    OrderDbContext db)", 10, CodeVar);
    Line(code, "{", 10, White);
    Line(code, "    var order = new Order(command);", 10, CodeStr);
    Line(code, "    db.Orders.Add(order);", 10, CodeStr);
    Line(code, "    // Wolverine pubblica l'evento", 10, CodeComment);
    Line(code, "    return new OrderPlaced(order.Id);", 10, CodeStr);
    Line(code, "}", 10, White);
    Line(code, "", 8, White);
    Line(code, "// Event Handler (Read Side)", 11, CodeComment);
    Line(code, "public static async Task Handle(", 10, CodeVar);
    Line(code, "    OrderPlaced @event,", 10, CodeVar);
    Line(code, "    IHubContext<OrderHub> hub)", 10, CodeVar);
    Line(code, "{", 10, White);
    Line(code, "    // Aggiorna read model + push", 10, CodeComment);
    Line(code, "    await hub.Clients.All", 10, CodeStr);
    Line(code, "        .SendAsync(\"OrderUpdate\");", 10, CodeStr);
    Line(code, "}", 10, White);
    Footer(sl, "14");
}

// ==================================================
// SLIDE 15: Demo 2 — Architettura CQRS
// ==================================================
{
    var sl = pres.Slide(15);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Demo 2 — Architettura CQRS", 26, DarkBlue, true);

    // Command Side
    Box(sl, MX, 85, 160, 25, AzureBlue, " COMMAND SIDE", 11, White, true);
    Box(sl, MX, 120, 170, 65, LightGray, "Blazor SSR", 13, AzureBlue, true);
    Line((IShape)sl.Shapes.Last(), "UI Ordini", 10, TextDark);

    Box(sl, 215, 130, 30, 25, White, "->", 14, AzureBlue, true);

    Box(sl, 255, 120, 180, 65, LightGray, "Command API", 13, AzureBlue, true);
    Line((IShape)sl.Shapes.Last(), "Wolverine handlers", 10, TextDark);

    // Service Bus
    Box(sl, 190, 210, 290, 35, AzureBlue, "Service Bus — Topic: order-events", 11, White, true);

    // Read Side
    Box(sl, 530, 85, 140, 25, AccentGreen, " READ SIDE", 11, White, true);
    Box(sl, 530, 120, 180, 65, LightGray, "Event Processor", 13, AccentGreen, true);
    Line((IShape)sl.Shapes.Last(), "Wolverine handlers", 10, TextDark);

    Box(sl, 715, 130, 30, 25, White, "->", 14, AccentGreen, true);

    Box(sl, 755, 120, 170, 65, LightGray, "Read Models", 13, AccentGreen, true);
    Line((IShape)sl.Shapes.Last(), "EF Core + SQL", 10, TextDark);

    // Arrow bus to event
    Box(sl, 485, 215, 50, 25, White, "---->", 10, AzureBlue, true);

    Box(sl, 530, 210, 180, 35, AzureBlue, "SignalR -> Blazor", 11, White, true);

    // Projects
    var p = Box(sl, MX, 280, 880, 200, White, "10 Progetti nella solution:", 14, DarkBlue, true);
    Line(p, "", 4, TextDark);
    Line(p, "  AppHost  •  ServiceDefaults  •  Web (Blazor)  •  CommandApi (Wolverine)", 10, TextDark);
    Line(p, "  ReadApi  •  EventProcessor (Wolverine)  •  Domain  •  ReadModel  •  Contracts  •  Simulator", 10, TextDark);
    Footer(sl, "15");
}

// ==================================================
// SLIDE 16: Demo 2 — Comandi ed Eventi
// ==================================================
{
    var sl = pres.Slide(16);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Demo 2 — Comandi ed Eventi", 30, DarkBlue, true);

    var cmd = Box(sl, MX, 90, 420, 230, LightGray, "Commands", 18, AzureBlue, true);
    Line(cmd, "", 6, TextDark);
    Line(cmd, "  PlaceOrder", 13, TextDark, true);
    Line(cmd, "     { Items[], AttendeeId, Notes }", 11, TextMedium);
    Line(cmd, "", 4, TextDark);
    Line(cmd, "  ConfirmOrder", 13, TextDark, true);
    Line(cmd, "     { OrderId }", 11, TextMedium);
    Line(cmd, "", 4, TextDark);
    Line(cmd, "  RejectOrder", 13, TextDark, true);
    Line(cmd, "     { OrderId, Reason }", 11, TextMedium);
    Line(cmd, "", 4, TextDark);
    Line(cmd, "  CancelOrder", 13, TextDark, true);
    Line(cmd, "     { OrderId }", 11, TextMedium);

    var evt = Box(sl, 500, 90, 420, 230, LightGray, "Events", 18, AccentGreen, true);
    Line(evt, "", 6, TextDark);
    Line(evt, "  OrderPlaced", 13, TextDark, true);
    Line(evt, "     { OrderId, Items[], AttendeeId, Timestamp }", 11, TextMedium);
    Line(evt, "", 4, TextDark);
    Line(evt, "  OrderConfirmed", 13, TextDark, true);
    Line(evt, "     { OrderId, EstimatedReady, Timestamp }", 11, TextMedium);
    Line(evt, "", 4, TextDark);
    Line(evt, "  OrderRejected", 13, TextDark, true);
    Line(evt, "     { OrderId, Reason, Timestamp }", 11, TextMedium);
    Line(evt, "", 4, TextDark);
    Line(evt, "  OrderCancelled", 13, TextDark, true);
    Line(evt, "     { OrderId, Timestamp }", 11, TextMedium);

    Box(sl, MX, 345, 880, 30, White, "Flusso: Blazor -> POST -> CommandApi -> Wolverine -> Service Bus -> EventProcessor -> Read Model -> SignalR -> Blazor", 10, DarkBlue, true);
    Footer(sl, "16");
}

// ==================================================
// SLIDE 17: Demo 2 — Read Models
// ==================================================
{
    var sl = pres.Slide(17);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Demo 2 — Read Models", 30, DarkBlue, true);
    Box(sl, MX, 72, 880, 20, White, "Proiezioni ottimizzate per la lettura, costruite dagli eventi", 14, TextMedium);

    var r1 = Box(sl, MX, 105, 200, 170, LightGray, "OrderListView", 13, AzureBlue, true);
    Line(r1, "", 4, TextDark);
    Line(r1, "Fonte:", 10, TextMedium, true);
    Line(r1, "OrderPlaced", 9, TextDark);
    Line(r1, "OrderConfirmed", 9, TextDark);
    Line(r1, "OrderRejected", 9, TextDark);
    Line(r1, "OrderCancelled", 9, TextDark);
    Line(r1, "", 4, TextDark);
    Line(r1, "Lista ordini con stato", 10, AccentGreen, true);

    var r2 = Box(sl, 265, 105, 200, 170, LightGray, "KitchenDashboard", 13, AzureBlue, true);
    Line(r2, "", 4, TextDark);
    Line(r2, "Fonte:", 10, TextMedium, true);
    Line(r2, "OrderPlaced", 9, TextDark);
    Line(r2, "OrderConfirmed", 9, TextDark);
    Line(r2, "", 4, TextDark);
    Line(r2, "Coda ordini cucina", 10, AccentGreen, true);

    var r3 = Box(sl, 490, 105, 200, 170, LightGray, "OrderStatsView", 13, AzureBlue, true);
    Line(r3, "", 4, TextDark);
    Line(r3, "Fonte:", 10, TextMedium, true);
    Line(r3, "Tutti gli eventi", 9, TextDark);
    Line(r3, "", 4, TextDark);
    Line(r3, "Totale ordini,", 10, AccentGreen, true);
    Line(r3, "tempi medi,", 10, AccentGreen, true);
    Line(r3, "tasso conferma", 10, AccentGreen, true);

    var r4 = Box(sl, 715, 105, 200, 170, LightGray, "PopularItems", 13, AzureBlue, true);
    Line(r4, "", 4, TextDark);
    Line(r4, "Fonte:", 10, TextMedium, true);
    Line(r4, "OrderPlaced", 9, TextDark);
    Line(r4, "", 4, TextDark);
    Line(r4, "Classifica piatti", 10, AccentGreen, true);
    Line(r4, "piu' ordinati", 10, AccentGreen, true);

    var n = Box(sl, MX, 300, 880, 170, White, "Ogni read model e' una vista indipendente, aggiornata in modo asincrono", 12, TextDark, true);
    Line(n, "", 6, TextDark);
    Line(n, "  •  Scalano indipendentemente dal write side", 11, TextDark);
    Line(n, "  •  Si possono ricostruire riprocessando gli eventi", 11, TextDark);
    Line(n, "  •  Ognuno ha la propria subscription su Service Bus", 11, TextDark);
    Line(n, "  •  Push automatico via SignalR quando aggiornati", 11, TextDark);
    Footer(sl, "17");
}

// ==================================================
// SLIDE 18: Demo 2 — Cosa si impara
// ==================================================
{
    var sl = pres.Slide(18);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Demo 2 — Cosa impariamo", 30, DarkBlue, true);

    var left = Box(sl, MX, 90, 430, 400, White, "Pattern & Architettura", 18, AzureBlue, true);
    Line(left, "", 4, TextDark);
    Line(left, "  •  CQRS: separazione command/read", 12, TextDark);
    Line(left, "  •  Event-driven projections", 12, TextDark);
    Line(left, "  •  Outbox pattern per consistenza", 12, TextDark);
    Line(left, "  •  Producer/Consumer + Pub/Sub", 12, TextDark);
    Line(left, "", 10, TextDark);
    Line(left, "Servizi Azure", 18, AzureBlue, true);
    Line(left, "", 4, TextDark);
    Line(left, "  •  Service Bus (queue + topic/sub)", 12, TextDark);
    Line(left, "  •  SignalR (push read model updates)", 12, TextDark);
    Line(left, "", 10, TextDark);
    Line(left, "Framework", 18, AzureBlue, true);
    Line(left, "", 4, TextDark);
    Line(left, "  •  Wolverine come command/event bus", 12, TextDark);
    Line(left, "  •  Aspire per orchestrazione complessa", 12, TextDark);

    var right = Box(sl, 500, 90, 420, 290, LightGray, "Da Demo 1 a Demo 2: cosa cambia?", 16, DarkBlue, true);
    Line(right, "", 6, TextDark);
    Line(right, "Demo 1:", 13, AzureBlue, true);
    Line(right, "  Simulator -> Hub -> UI", 11, TextDark);
    Line(right, "  Accoppiamento diretto", 11, AccentOrange);
    Line(right, "", 8, TextDark);
    Line(right, "Demo 2:", 13, AccentGreen, true);
    Line(right, "  UI -> API -> ServiceBus -> Processor", 11, TextDark);
    Line(right, "  -> ReadModel -> SignalR -> UI", 11, TextDark);
    Line(right, "", 4, TextDark);
    Line(right, "  Disaccoppiamento totale", 11, AccentGreen);
    Line(right, "  Resilienza (Service Bus = buffer)", 11, AccentGreen);
    Line(right, "  Read model indipendenti", 11, AccentGreen);
    Line(right, "  Scalabilita' per componente", 11, AccentGreen);
    Footer(sl, "18");
}

// ==================================================
// SLIDE 19: Bonus — Load Testing + Autoscale
// ==================================================
{
    var sl = pres.Slide(19);
    Bg(sl, AccentOrange);
    Box(sl, MX, 40, 300, 25, AccentOrange, "BONUS", 13, White, true);
    Box(sl, MX, 80, 880, 50, AccentOrange, "Load Testing + Autoscale", 38, White, true);
    Box(sl, MX, 155, 880, 30, AccentOrange, "Quanto regge? Scaliamo su Azure sotto carico!", 18, LightOrange);

    var b = Box(sl, MX, 220, 880, 280, AccentOrange, "Il piano:", 20, White, true);
    Line(b, "", 6, White);
    Line(b, "  1.  Deploy ReactiveOrders su Azure App Service", 14, White);
    Line(b, "  2.  Configurare autoscale (CPU > 70% -> +1 istanza)", 14, White);
    Line(b, "  3.  Lanciare Azure Load Testing (10 -> 500 utenti)", 14, White);
    Line(b, "  4.  Osservare scale-out live + Service Bus come buffer", 14, White);
    Line(b, "  5.  Dopo il test: scale-in, zero messaggi persi", 14, White);
    Line(b, "", 10, White);
    Line(b, "Costi: pochi centesimi per l'intero test", 14, LightOrange, true);
}

// ==================================================
// SLIDE 20: Bonus — Cosa osservare
// ==================================================
{
    var sl = pres.Slide(20);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Bonus — Cosa osservare durante il test", 26, DarkBlue, true);

    var c1 = Box(sl, MX, 90, 210, 230, LightGray, "Load Testing", 14, AccentOrange, true);
    Line(c1, "Dashboard", 12, AzureBlue, true);
    Line(c1, "", 4, TextDark);
    Line(c1, "  •  Throughput (req/s)", 10, TextDark);
    Line(c1, "  •  Latency (p50, p95)", 10, TextDark);
    Line(c1, "  •  Error rate", 10, TextDark);
    Line(c1, "  •  Utenti virtuali attivi", 10, TextDark);

    var c2 = Box(sl, 270, 90, 210, 230, LightGray, "App Service", 14, AzureBlue, true);
    Line(c2, "Metriche", 12, AzureBlue, true);
    Line(c2, "", 4, TextDark);
    Line(c2, "  •  CPU %", 10, TextDark);
    Line(c2, "  •  Request count", 10, TextDark);
    Line(c2, "  •  Instance count", 10, TextDark);
    Line(c2, "  •  HTTP queue length", 10, TextDark);
    Line(c2, "", 4, TextDark);
    Line(c2, "Scale-out visibile!", 11, AccentGreen, true);

    var c3 = Box(sl, 500, 90, 210, 230, LightGray, "Service Bus", 14, AzureBlue, true);
    Line(c3, "Metriche", 12, AzureBlue, true);
    Line(c3, "", 4, TextDark);
    Line(c3, "  •  Message count (coda)", 10, TextDark);
    Line(c3, "  •  Active messages", 10, TextDark);
    Line(c3, "  •  Dead-letter count", 10, TextDark);
    Line(c3, "", 4, TextDark);
    Line(c3, "Buffer durante scaling!", 11, AccentGreen, true);

    var c4 = Box(sl, 730, 90, 190, 230, LightGray, "Blazor UI", 14, AzureBlue, true);
    Line(c4, "Live", 12, AzureBlue, true);
    Line(c4, "", 4, TextDark);
    Line(c4, "  •  Ordini a raffica", 10, TextDark);
    Line(c4, "  •  Read model update", 10, TextDark);
    Line(c4, "  •  Statistiche live", 10, TextDark);

    var tk = Box(sl, MX, 350, 880, 120, White, "Takeaway: Service Bus disaccoppia API e consumer. Durante lo scaling,", 12, DarkBlue, true);
    Line(tk, "i messaggi si accumulano nella coda e vengono processati appena le nuove istanze sono pronte.", 12, DarkBlue);
    Line(tk, "Nessun messaggio perso, nessun errore utente.", 12, AccentGreen, true);
    Footer(sl, "20");
}

// ==================================================
// SLIDE 21: Confronto
// ==================================================
{
    var sl = pres.Slide(21);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Confronto: quando usare cosa?", 30, DarkBlue, true);

    Box(sl, MX, 90, 880, 30, DarkBlue, "                              ReactiveBoard                    ReactiveOrders", 12, White, true);

    string[,] rows = {
        { "Complessità      ", "Semplice                           ", "Avanzata" },
        { "Progetti         ", "4                                  ", "10" },
        { "Messaging        ", "Diretto (Hub)                      ", "Service Bus (queue + topic)" },
        { "Pattern          ", "Push diretto                       ", "CQRS + Event-driven" },
        { "Scalabilità      ", "Limitata (1 istanza)               ", "Orizzontale (competing consumers)" },
        { "Resilienza       ", "Bassa (se Hub cade)                ", "Alta (Service Bus = buffer)" },
        { "Osservabilità    ", "Aspire Dashboard                   ", "Aspire + Azure Monitor" },
        { "Caso d'uso ideale", "Dashboard, notifiche               ", "E-commerce, sistemi critici" },
    };

    for (int i = 0; i < rows.GetLength(0); i++)
    {
        var bgColor = i % 2 == 0 ? LightGray : White;
        Box(sl, MX, 120 + i * 30, 880, 30, bgColor, $"  {rows[i, 0]}  {rows[i, 1]}  {rows[i, 2]}", 10, TextDark);
    }

    Box(sl, MX, 380, 880, 50, White, "Non esiste la soluzione \"giusta\": dipende dalla complessita' del dominio e dai requisiti di resilienza.", 12, TextMedium);
    Footer(sl, "21");
}

// ==================================================
// SLIDE 22: Risorse e Link
// ==================================================
{
    var sl = pres.Slide(22);
    TopBar(sl);
    Box(sl, MX, MY, 880, 40, White, "Risorse e link utili", 30, DarkBlue, true);

    var b = Box(sl, MX, 90, 880, 400, White, "Documentazione", 18, AzureBlue, true);
    Line(b, "", 4, TextDark);
    Line(b, "  •  .NET Aspire — learn.microsoft.com/dotnet/aspire", 12, TextDark);
    Line(b, "  •  Azure SignalR — learn.microsoft.com/azure/azure-signalr", 12, TextDark);
    Line(b, "  •  Azure Service Bus — learn.microsoft.com/azure/service-bus-messaging", 12, TextDark);
    Line(b, "  •  Azure App Service — learn.microsoft.com/azure/app-service", 12, TextDark);
    Line(b, "  •  Azure Load Testing — learn.microsoft.com/azure/load-testing", 12, TextDark);
    Line(b, "", 10, TextDark);
    Line(b, "Framework", 18, AzureBlue, true);
    Line(b, "", 4, TextDark);
    Line(b, "  •  Wolverine — wolverine.netlify.app", 12, TextDark);
    Line(b, "  •  Blazor — learn.microsoft.com/aspnet/core/blazor", 12, TextDark);
    Line(b, "", 10, TextDark);
    Line(b, "Codice sorgente", 18, AzureBlue, true);
    Line(b, "", 4, TextDark);
    Line(b, "  •  github.com/<your-repo>/GlobalAzure2026", 12, TextMedium);
    Footer(sl, "22");
}

// ==================================================
// SLIDE 23: Grazie / Q&A
// ==================================================
{
    var sl = pres.Slide(23);
    Bg(sl, DarkBlue);
    Box(sl, 0, 0, SW, 8, AzureBlue, " ", 1, AzureBlue);
    Box(sl, MX, 150, 880, 60, DarkBlue, "Grazie!", 44, White, true);
    Box(sl, MX, 240, 880, 40, DarkBlue, "Domande?", 28, AccentGreen);

    var c = Box(sl, MX, 330, 880, 50, DarkBlue, "Global Azure 2026", 16, Gray);
    Line(c, "I mille modi di creare un'applicazione reattiva su Azure", 12, MedGray);
}

// ── Save ──
var finalPath = Path.Combine("c:", "code-personal", "GlobalAzure2026", "slides", "GlobalAzure2026.pptx");
Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);
pres.Save(finalPath);
Console.WriteLine($"Presentazione salvata: {finalPath}");
Console.WriteLine($"Totale slide: {pres.Slides.Count}");
