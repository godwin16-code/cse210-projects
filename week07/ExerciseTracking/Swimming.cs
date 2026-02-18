using System;

public class Swimming : Activity
{
    private int _laps;

    public Swimming(DateTime date, int lengthInMinutes, int laps) : base(date, lengthInMinutes)
    {
        _laps = laps;
    }

    public override double GetDistance() => _laps * 50.0 / 1000 * 0.62; // miles

    public override double GetSpeed() => (GetDistance() / LengthInMinutes) * 60;

    public override double GetPace() => LengthInMinutes / GetDistance();
}