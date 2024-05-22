using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Extensions;
using Mscc.GenerativeAI.Web;
using TLDBot.Services;
using TLDBot.Utility;

var builder = new HostApplicationBuilder(args);

// Discord
builder.Services.AddSingleton<DiscordSocketClient>(Helper.Client);
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddSingleton<CommandService>();

//Run Lavalink.jar
Lavalink.Start();

//Bot hosted
builder.Configuration.AddJsonFile("Json/Configuration.json", optional: true, reloadOnChange: true);
builder.Services.AddHostedService<BotService>();

//Gemini
builder.Services.AddGenerativeAI(builder.Configuration.GetSection("Gemini"));

// Lavalink
builder.Services.AddLavalink();
builder.Services.ConfigureLavalink(config =>
{
	config.BaseAddress = new Uri("http://localhost:2323");
	//config.WebSocketUri = new Uri("ws://localhost:2323/v4/websocket");
	//config.ReadyTimeout = TimeSpan.FromSeconds(10);
	//config.ResumptionOptions = new LavalinkSessionResumptionOptions(TimeSpan.FromSeconds(60));
	config.Passphrase = "on6tyeqwxdtvo3ld";
});
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

await builder.Build().RunAsync();