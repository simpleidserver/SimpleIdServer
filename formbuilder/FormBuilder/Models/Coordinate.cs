namespace FormBuilder.Models;

public class Coordinate
{
    public double X { get; set; }
    public double Y { get; set; }

    public static Coordinate operator -(Coordinate pos1, Coordinate pos2) => new Coordinate { X = pos1.X - pos2.X, Y = pos1.Y - pos2.Y };

    public static Coordinate operator +(Coordinate pos1, Coordinate pos2) => new Coordinate { X = pos1.X + pos2.X, Y = pos1.Y + pos2.Y };

    public Coordinate Clone()
    {
        return new Coordinate
        {
            X = X,
            Y = Y
        };
    }
}