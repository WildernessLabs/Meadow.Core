using System;
namespace Meadow.Peripherals.Sensors.Atmospheric
{
    //public class AtmosphericConditions
    //{
    //    /// <summary>
    //    /// The temperature, in degrees celsius (ºC).
    //    /// </summary>
    //    public float? Temperature { get; set; }
    //    /// <summary>
    //    /// The pressure, in hectopascals (hPa), which is equal to one
    //    /// millibar, or 1/10th of a kilopascal (kPa)/centibar.
    //    /// </summary>
    //    public float? Pressure { get; set; }
    //    /// <summary>
    //    /// The humidity, in percent relative humidity.
    //    /// </summary>
    //    public float? Humidity { get; set; }

    //    public AtmosphericConditions()
    //    { }

    //    public AtmosphericConditions(
    //        float? temperature,
    //        float? pressure,
    //        float? humidity)
    //    {
    //        Temperature = temperature;
    //        Pressure = pressure;
    //        Humidity = humidity;
    //    }

    //    public static AtmosphericConditions From(AtmosphericConditions conditions)
    //    {
    //        return new AtmosphericConditions(
    //            conditions.Temperature,
    //            conditions.Pressure,
    //            conditions.Humidity
    //            );
    //    }
    //}

    //public class AtmosphericConditionChangeResult : IChangeResult<AtmosphericConditions>
    //{
    //    public AtmosphericConditions New {
    //        get => newValue; 
    //        set {
    //            newValue = value;
    //            RecalcDelta();
    //        }
    //    } 
    //    protected AtmosphericConditions newValue = new AtmosphericConditions();

    //    public AtmosphericConditions Old {
    //        get => oldValue; 
    //        set {
    //            oldValue = value;
    //            RecalcDelta();
    //        }
    //    }
    //    protected AtmosphericConditions oldValue = new AtmosphericConditions();

    //    public AtmosphericConditions Delta { get; protected set; } = new AtmosphericConditions();

    //    public AtmosphericConditionChangeResult(
    //        AtmosphericConditions newValue, AtmosphericConditions oldValue)
    //    {
    //        New = newValue;
    //        Old = oldValue;
    //    }

    //    protected void RecalcDelta()
    //    {
    //        AtmosphericConditions delta = new AtmosphericConditions();
    //        delta.Temperature = New.Temperature - Old.Temperature;
    //        delta.Pressure = New.Pressure - Old.Pressure;
    //        delta.Humidity = New.Humidity - Old.Humidity;
    //        Delta = delta;
    //    }
    //}
}