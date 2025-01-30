namespace FormBuilder.Models;

public record Size
{
    public double width { get; set; }
    public double height { get; set; }

    public Size Half()
        => new Size { width = HalfWidth(), height = HalfHeight() };

    public double HalfWidth() => width / 2;

    public double HalfHeight() => height / 2;
}
