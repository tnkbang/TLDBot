using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Extensions;
using TLDBot.Structs;

var builder = new HostApplicationBuilder(args);

// Discord
builder.Services.AddSingleton<DiscordSocketClient>(Helper.Client);
builder.Services.AddSingleton<InteractionService>();

//Run Lavalink.jar
Lavalink.Start();

//Bot hosted
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddHostedService<Bot>();

// Lavalink
builder.Services.AddLavalink();
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Build().Run();