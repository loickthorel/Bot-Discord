using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Bot_Discord;

internal class Program
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _config;
    private const string Separator = "!";

    public static Task Main() => new Program().MainAsync();

    private Program()
    {
        _client = new DiscordSocketClient();

        //Hook into log event and write it out to the console
        _client.Log += Log;

        //Hook into the client ready event
        _client.Ready += Ready;

        //Hook into the message received event, this is how we handle the hello world example
        _client.MessageReceived += MessageReceivedAsync;

        //Create the configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(path: "config.json");
        _config = builder.Build();
    }

    private async Task MainAsync()
    {
        //This is where we get the Token value from the configuration file
        await _client.LoginAsync(TokenType.Bot, _config["Token"]);
        await _client.StartAsync();

        // Block the program until it is closed.
        await Task.Delay(-1);
    }

    private static Task Log(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private static Task Ready()
    {
        Console.WriteLine($"Connected as MegActuBot :)");
        return Task.CompletedTask;
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        //This ensures we don't loop things by responding to ourselves (as the bot)
        if (message.Author.Id == _client.CurrentUser.Id)
            return;

        if (message.Content[..1] == Separator)
        {
            string mess = message.Content.Replace(Separator, "");
            switch (mess[..4])
            {
                case "Amz-":
                    var amzObject = mess.Split("-");
                    var amzEmbeds = AmazonSearch.UrlWithParameter(amzObject[1]);
                    await message.Channel.SendMessageAsync(embeds: amzEmbeds);
                    break;
                
                case "Ike-":
                    var ikeaObject = mess.Split("-");
                    var ikeaEmbeds = IkeaSearch.UrlWithParameter(ikeaObject[1]);
                    await message.Channel.SendMessageAsync(embeds: ikeaEmbeds);
                    break;
            }

            switch (mess)
            {
                case "Hello":
                    await message.Channel.SendMessageAsync("world!");
                    break;
                case "Temperature":
                    await GetTemperature.GetTemp(message);
                    break;
                case "Test":
                    break;
            }
        }
    }
}