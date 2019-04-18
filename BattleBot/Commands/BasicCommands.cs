using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BattleBot.Commands {
  // Just some basic commands to get started
  // use an empty name attribute because these are default
  [Name("")]
  public class BasicCommands : ModuleBase {
    private readonly CommandService _commandService;

    public BasicCommands(CommandService commands) {
      _commandService = commands;
    }

    // Basic Ping/Pong command
    [Command("Ping"), Summary("A simple ping/pong command, for checking if the bot works.")]
    public async Task Ping() {
      await ReplyAsync("Pong!");
    }

    // Hard-coded help command.
    // There are ways to do this programatically.  This is a placeholder until those are implemented.
    [Command("Help"), Summary("Help text.")]
    public async Task HelpText() {
      await ReplyAsync(
        @"**Help**:
?Ping
?Help
?Echo text
?InviteLink
?Debug");
    }

    // Echo command
    [Command("Echo"), Summary("A simple echo command.")]
    public async Task Echo([Name("Text"), Summary("The text to echo back."), Remainder]
      string text) {
      await ReplyAsync(Context.User.Mention + " : " + text);
    }

    [Command("InviteLink")]
    [Summary("Gets the invite link for the bot.")]
    [RequireUserPermission(GuildPermission.SendMessages | GuildPermission.EmbedLinks)]
    public async Task InviteLink() {
      // the link:
      // https://discordapp.com/oauth2/authorize?client_id=YOURBOTCLIENTIDGOESHERE&scope=bot
      // is used for inviting the bot with the default permissions for everyone
      // this calculator is a good way to generate these links as well
      // https://discordapi.com/permissions.html
      await ReplyAsync($"A user with the 'Manage Server' permission can add me to your server using the following link: https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&scope=bot");
    }

    [Command("Debug")]
    [Summary("Replies back with some debug info about the bot.")]
    [RequireUserPermission(GuildPermission.SendMessages)]
    public async Task DebugInfo() {
      await ReplyAsync(
        $"{Format.Bold("Info")}\n" +
        $"- D.NET Lib Version {DiscordConfig.Version} (API v{DiscordConfig.APIVersion})\n" +
        $"- Runtime (Framework, and OS): {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}\n" +
        $"- Heap (Memory the bot is using currently): {GetHeapSize()} MB\n" +
        $"- Up-time (Time since bots last reboot): {GetUpTime()}\n\n" +
        $"- Guilds (Servers Battle Bot serves in): {((DiscordSocketClient) Context.Client).Guilds.Count}\n" +
        $"- Channels (Channels in the server): {((DiscordSocketClient) Context.Client).Guilds.Sum(g => g.Channels.Count)}\n" +
        $"- Users (Users in the server): {((DiscordSocketClient) Context.Client).Guilds.Sum(g => g.Users.Count)}"
      );
    }

    private static string GetUpTime()
      => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");

    private static string GetHeapSize()
      => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
  }
}