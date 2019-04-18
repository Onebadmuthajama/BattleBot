using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Discord.Commands;
using System.Reflection;

namespace BattleBot {
  public class BattleBot {
    private DiscordSocketClient _mClient;
    private CommandService _commands;
    private IServiceProvider _services;

    public async Task Start() {
      // starts the client
      // LogSeverity.Debug because more info the better
      _mClient = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Debug});

      // init the Command Service
      _commands = new CommandService();

      // log in as a bot using the connection token
      await _mClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
      await _mClient.StartAsync();

      // dependency injection
      // this is used to automatically populate the types that commands ask
      // for, since we don't instantiate the types ourselves
      _services = new ServiceCollection()
        .AddSingleton(_mClient)
        .AddSingleton(_commands)
        .BuildServiceProvider();

      await InstallCommandsAsync();

      // set up the logging function
      _mClient.Log += Log;

      // show an invite link when we are ready to go
      _mClient.Ready += ClientReady;

      // set some help text
      // this is a good way to let the user know which command to type to get started
      await _mClient.SetGameAsync($"Type {GlobalConfiguration.COMMAND_PREFIX}Help");

      // wait indefinitely 
      await Task.Delay(-1);
    }

    private async Task InstallCommandsAsync() {
      _mClient.MessageReceived += ClientMessageReceived;
      await _services.GetRequiredService<CommandService>().AddModulesAsync(Assembly.GetEntryAssembly(), null);

    }

    private async Task ClientMessageReceived(SocketMessage arg) {
      // Don't handle the command if it is a system message

      if (!(arg is SocketUserMessage message)) {
        return;
      }

      // Mark where the prefix ends and the command begins
      var argPos = 0;

      // Determine if the message has a valid prefix, adjust argPos 
      // ensure that the message either starts with the command prefix
      // or by @mentioning the bot user
      // IE: ?help or @battle_bot help
      if (!(message.HasMentionPrefix(_mClient.CurrentUser, ref argPos) || message.HasCharPrefix(GlobalConfiguration.COMMAND_PREFIX, ref argPos))) {
        return;
      }

      // Create a Command Context
      var context = new CommandContext(_mClient, message);

      // Execute the Command, store the result
      var result = await _commands.ExecuteAsync(context, argPos, _services);

      // If the command failed
      if (!result.IsSuccess) {
        // log the error
        var errorMessage = new LogMessage(LogSeverity.Warning, "CommandHandler", result.ErrorReason);
        await Log(errorMessage);
      }
    }

    private async Task ClientReady() {
      // display a helpful invite url in the log when the bot is ready
      var application = await _mClient.GetApplicationInfoAsync();

      await Log(new LogMessage(LogSeverity.Info, "Program",
        $"Invite URL: <https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot>"));
    }

    public async static Task Log(LogMessage arg) {
      // log stuff to console
      // could also log to a file here
      Console.WriteLine(arg.ToString());
    }
  }
}