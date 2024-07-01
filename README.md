# TLDBot

Discord bot build with Discord.Net and Lavalink....
<br/>
Using for play music and misc command.
<br/>
Button emoij referenced <a href="https://www.flaticon.com/authors/kp-arts/flat-gradient">KP Arts Flat Gradient</a>.
</br>

## Requirements

1. Discord Bot Token **[Guide](https://discordjs.guide/preparations/setting-up-a-bot-application.html#creating-your-bot)**  
2. .Net Core 8.0 (C#) and Java runtime 20 or newer
3. <a href="https://github.com/lavalink-devs/Lavalink"> Lavalink 4</a>
4. <a href="https://github.com/angelobreuer/Lavalink4NET"> Lavalink4NET</a>
5. <a href="https://github.com/discord-net/Discord.Net"> Discord .Net</a>
6. <a href="https://aistudio.google.com/app"> Google Gemini</a>

## Getting Started

```sh
git clone https://github.com/tnkbang/TLDBot
cd TLDBot
```

After installation finishes follow configuration instructions then run `dotnet run TLDBot.exe` or `open Visual Studio` to start the bot.

## Configuration

Copy or Rename `_Configuration.json` to `Configuration.json` and fill out the values:

Change your `TOKEN`, owner id for `OWNER` and other values....

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "Gemini": {
        "Credentials": {
            "ApiKey": "" // replace value with key from AI Studio
        },
        "ProjectId": "",
        "Region": "us-central1",
        "Model": "gemini-1.0-pro" // default value
    },
    "Prefix": ".",
    "DiscordToken": "" // replace value with your bot token
}
```

Gemini key get from: https://aistudio.google.com/app/apikey

## Features & Commands

- Music playing: play, queue, nowplaying, skip, stop,...
- TicTactoe game and HooHeyHow game.
- AI chat bot.
- And waiting for development :>>