using System.Timers;
using Discord.Commands;
using Timer = System.Timers.Timer;

namespace Bot_Discord;

public static class MessTimer
{
    private static Timer _messageTimer = new();
    private static SocketCommandContext _context = null!;

    public static void StartTimer(SocketCommandContext context)
    {
        _context = context;

        _messageTimer = new System.Timers.Timer(30000);
        _messageTimer.Elapsed += OnTimerElapsed;
        _messageTimer.AutoReset = true;
        _messageTimer.Enabled = true;
    }

    private static void OnTimerElapsed(object? source, ElapsedEventArgs e)
    {
        _context.Channel.SendMessageAsync("Test Message");
        Console.WriteLine("Test Message");
    }
}