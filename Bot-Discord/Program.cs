using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Iot.Device.CpuTemperature;

namespace Bot_Discord
{
    class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;
        private readonly CpuTemperature _cpuTemperature = new CpuTemperature();
        private const string Separator = "!"; 

        public static Task Main(string[] args) => new Program().MainAsync();

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
            Console.WriteLine($"Connected as -> [] :)");
            return Task.CompletedTask;
        }

        //I wonder if there's a better way to handle commands (spoiler: there is :))
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            //This ensures we don't loop things by responding to ourselves (as the bot)
            if (message.Author.Id == _client.CurrentUser.Id)
                return;
            
            if (message.Content.Substring(0,1) == Separator)
            {
                string mess = message.Content.Replace(Separator, "");

                if (mess.Substring(0, 4) == "Amz-")
                {
                    var amzObject = mess.Split("-");
                    Console.WriteLine(amzObject);
                }

                switch (mess)
                {
                    case "Test":
                        await message.Channel.SendMessageAsync("world!");
                        break;
                    case "Test2":
                        await message.Channel.SendMessageAsync("pas world!");
                        break;
                    case "Temperature":
                        await GetTemp(message);
                        break;
                }
            }
            
        }

        private async Task GetTemp(SocketMessage message)
        {
            if (_cpuTemperature.IsAvailable)
            {
                var temperature = _cpuTemperature.ReadTemperatures();
                foreach (var entry in temperature)
                {
                    if (!double.IsNaN(entry.Temperature.DegreesCelsius))
                    {
                        await message.Channel.SendMessageAsync($"Temperature from {entry.Sensor.ToString()}: {entry.Temperature.DegreesCelsius} °C");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Unable to read Temperature.");
                    }
                }
            }
            else
            {
                await message.Channel.SendMessageAsync("CPU temperature is not available");
            }
        }
    }
}