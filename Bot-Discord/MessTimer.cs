using System.Timers;
using Discord.Commands;

namespace Bot_Discord;

public static class MessTimer
{
    private static System.Timers.Timer messageTimer;
    private static SocketCommandContext _Context;

    public static void StartTimer(SocketCommandContext context)
    {
        _Context = context;

        messageTimer = new System.Timers.Timer(30000);
        messageTimer.Elapsed += OnTimerElapsed;
        messageTimer.AutoReset = true;
        messageTimer.Enabled = true;
    }

    public static void OnTimerElapsed(object source, ElapsedEventArgs e)
    {
        _Context.Channel.SendMessageAsync("Test Message");
        Console.WriteLine("Test Message");
    }
}