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

socketClient.Ready += async () =>
{
    await socketClient.CreateGlobalApplicationCommandAsync(new SlashCommandBuilder()
        .WithName("주사위")
        .WithDescription("주사위를 굴립니다.")
        .AddOption("눈수", ApplicationCommandOptionType.Integer, "눈의 수를 입력합니다. 입력하지 않으면 기본값 6.")
        .Build()
    );
    await socketClient.CreateGlobalApplicationCommandAsync(new SlashCommandBuilder()
        .WithName("동전")
        .WithDescription("동전을 던집니다.")
        .Build()
    );
    // await socketClient.CreateGlobalApplicationCommandAsync(new SlashCommandBuilder()
    //     .WithName("상태")
    //     .WithDescription("회피/살짝스침/스침/적중 중 하나를 고릅니다.")
    //     .Build()
    // );
};
socketClient.SlashCommandExecuted += async command =>
{
    switch (command.Data.Name)
    {
        case "주사위":
        {
            var option = command.Data.Options.FirstOrDefault(option => option.Name == "눈수");
            var maxValue = 6;
            if (option != null)
                maxValue = (int) (long) option.Value;

            var value = commonRandom.Next(1, maxValue);
            await command.RespondAsync(text: $"주사위를 굴려서 {value}가 나왔습니다!");
            break;
        }
        
        case "동전":
        {
            var value = commonRandom.Next(0, 2);
            await command.RespondAsync(text: $"동전을 굴려서 {(value switch { 0 => "앞", 1 => "뒷", _ => "?" })}면이 나왔습니다!");
            break;
        }
        
        // case "상태":
        // {
        //     var value = commonRandom.Next(0, 10);
        //     var result = value switch
        //     {
        //         0 => "회피",
        //         1 => "살짝 스침",
        //         2 => "살짝 스침",
        //         3 => "스침",
        //         4 => "스침",
        //         5 => "스침",
        //         6 => "적중",
        //         7 => "적중",
        //         8 => "적중",
        //         9 => "적중",
        //         _ => "?"
        //     };
        //     await command.RespondAsync(text: $"💁 {result}");
        //     break;
        // }
        
        default:
            await command.RespondAsync(text: "알 수 없는 명령입니다.");
            break;
    }
};

var discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
socketClient.LoginAsync(TokenType.Bot, discordToken).GetAwaiter().GetResult();
socketClient.StartAsync().GetAwaiter().GetResult();

Thread.Sleep(Timeout.InfiniteTimeSpan);