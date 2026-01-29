namespace People.Worker.Options;

public sealed class OutboxOptions
{
    public int PollSeconds { get; set; } = 2;
    public int BatchSize { get; set; } = 50;
}
