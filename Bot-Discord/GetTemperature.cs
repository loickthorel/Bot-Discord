using Discord.WebSocket;
using Iot.Device.CpuTemperature;

namespace Bot_Discord;

public static class GetTemperature
{
    private static readonly CpuTemperature CpuTemperature = new();

    public static async Task GetTemp(SocketMessage message)
    {
        if (CpuTemperature.IsAvailable)
        {
            var temperature = CpuTemperature.ReadTemperatures();
            foreach (var entry in temperature)
            {
                if (!double.IsNaN(entry.Temperature.DegreesCelsius))
                {
                    await message.Channel.SendMessageAsync($"Temperature from {entry.Sensor}: {entry.Temperature.DegreesCelsius} °C");
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