namespace TemporalTableTest;

public interface IDateTime
{
    DateTime Now { get; }
}

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}
