namespace FormBuilder.Models;

public class Coordinate
{
    public Coordinate()
    {
        
    }

    public Coordinate(double x, double y)
    {
        X = x; 
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }

    public Coordinate Positive()
    {
        X = X < 0 ? -X : X;
        Y = Y < 0 ? -Y : Y;
        return this;
    }

    public Coordinate Div(double divisor)
    {
        X = (double)X / divisor;
        Y = (double)Y / divisor;
        return this;
    }

    public Coordinate Round()
    {
        X = Math.Round(X);
        Y = Math.Round(Y);
        return this;
    }

    public Coordinate Min(Coordinate secondCoordinate)
    {
        return new Coordinate
        {
            X = X < secondCoordinate.X ? X : secondCoordinate.X,
            Y = Y < secondCoordinate.Y ? Y : secondCoordinate.Y
        };
    }

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