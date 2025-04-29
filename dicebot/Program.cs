using Discord;
using Discord.WebSocket;

var commonRandom = new Random();

var socketClient = new DiscordSocketClient(new DiscordSocketConfig
{
    LogLevel = LogSeverity.Verbose,
    AlwaysDownloadUsers = false,
    AlwaysResolveStickers = false,
    GatewayIntents = GatewayIntents.AllUnprivileged,
});

var commands = new Dictionary<SlashCommandProperties, Func<SocketSlashCommand, Task>>
{
    {
        new SlashCommandBuilder()
            .WithName("주사위")
            .WithDescription("주사위를 굴립니다.")
            .AddOption("눈수", ApplicationCommandOptionType.Integer, "눈의 수를 입력합니다. 입력하지 않으면 기본값 6.")
            .Build(),
        async command =>
        {
            var option = command.Data.Options.FirstOrDefault(option => option.Name == "눈수");
            var maxValue = 6;
            if (option != null)
                maxValue = (int) (long) option.Value;

            var value = commonRandom.Next(1, maxValue);
            await command.RespondAsync(text: $"주사위를 굴려서 {value}가 나왔습니다!");
        }
    },
    {
        new SlashCommandBuilder()
            .WithName("동전")
            .WithDescription("동전을 던집니다.")
            .Build(),
        async command =>
        {
            var value = commonRandom.Next(0, 2);
            await command.RespondAsync(text: $"동전을 굴려서 {value switch { 0 => "앞", 1 => "뒷", _ => "?" }}면이 나왔습니다!");
        }
    },
    {
        new SlashCommandBuilder()
            .WithName("확률")
            .WithDescription("확률에 따라 당첨 및 낙첨 여부를 결정합니다.")
            .AddOption("확률", ApplicationCommandOptionType.Integer, "1~100 사이로 확률을 입력합니다. 입력하지 않으면 기본값 50.")
            .Build(),
        async command =>
        {
            var option = command.Data.Options.FirstOrDefault(option => option.Name == "확률");
            var probability = 50;
            if (option != null)
                probability = (int) (long) option.Value;

            var value = commonRandom.Next(0, 100);
            await command.RespondAsync(text: $"{probability}%의 확률로 {(value <= probability ? "당첨!" : "낙첨!")}");
        }
    }
};

socketClient.LoggedIn += async () =>
{
    await Console.Out.WriteLineAsync("Discord Client is logged in.");
};
socketClient.LoggedOut += async () =>
{
    await Console.Out.WriteLineAsync("Discord Client is logged out.");
};
socketClient.Ready += async () =>
{
    await Console.Out.WriteLineAsync("Discord Client is ready.");
    await socketClient.Rest.DeleteAllGlobalCommandsAsync();
    await Console.Out.WriteLineAsync("Deleted all global commands.");
    
    var commandTasks = commands.Keys.Select(command => socketClient.CreateGlobalApplicationCommandAsync(command)).ToArray();
    await Task.WhenAll(commandTasks);
    await Console.Out.WriteLineAsync("Commands are registered.");
};
socketClient.SlashCommandExecuted += async command =>
{
    await Console.Out.WriteLineAsync("Slash command is executed.");
    var found = commands.FirstOrDefault(c => c.Key.Name.GetValueOrDefault() == command.Data.Name);
    if (found.Value != null)
        await found.Value.Invoke(command);
    else
        await command.RespondAsync(text: "알 수 없는 명령입니다.");
};

var discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
await socketClient.LoginAsync(TokenType.Bot, discordToken);
await socketClient.StartAsync();

Thread.Sleep(Timeout.InfiniteTimeSpan);