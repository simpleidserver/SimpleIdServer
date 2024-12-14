namespace FormBuilder.Models;

public class Size
{
    public double width { get; set; }
    public double height { get; set; }

    public Size Clone()
    {
        return new Size
        {
            width = width,
            height = height
        };
    }
}
