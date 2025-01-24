namespace FormBuilder.Helpers;

public interface IDateTimeHelper
{
    DateTime GetCurrent();
}

public class DateTimeHelper : IDateTimeHelper
{
    public DateTime GetCurrent() => DateTime.UtcNow;
}
