using Xunit;
using System;
using Meadow.Units;

namespace Meadow.Units.Tests
{
    public class PressureTests
    {
        [Fact()]
        public void PressureCtors()
        {
            Pressure p = new Pressure(1, Pressure.UnitType.StandardAtmosphere);
            // fucking floating points. even comparing to another double (`1d`),
            // even though it should have the same exact value underneath. wtf.
            Assert.True(p.StandardAtmosphere.Equals4DigitPrecision(1)); 

            Pressure p2 = new Pressure();
            Assert.True(p2.Bar == 0);

            Pressure p3 = new Pressure(1, Pressure.UnitType.Bar);
            Assert.True(p3.Bar == 1);
        }

        [Fact()]
        public void Conversions()
        {
            Pressure p = new Pressure(1, Pressure.UnitType.Bar);
            Assert.True(p.Pascal == 100000d);
            Assert.True(p.Psi.Equals4DigitPrecision(14.503773773));
            Assert.True(p.StandardAtmosphere.Equals4DigitPrecision(0.9869233));

            Pressure p2 = new Pressure(1, Pressure.UnitType.Pascal);
            Assert.True(p2.Bar.Equals4DigitPrecision(0.000001));
            Assert.True(p2.Psi.Equals4DigitPrecision(0.000145038));
            Assert.True(p2.StandardAtmosphere.Equals4DigitPrecision(0.00000098692));

            Pressure p3 = new Pressure(1, Pressure.UnitType.Psi);
            Assert.True(p3.Bar.Equals4DigitPrecision(0.0689476));
            Assert.True(p3.Pascal.Equals4DigitPrecision(6894.7572931783));
            Assert.True(p3.StandardAtmosphere.Equals4DigitPrecision(0.068046));

            Pressure p4 = new Pressure(1, Pressure.UnitType.StandardAtmosphere);
            Assert.True(p4.Bar.Equals4DigitPrecision(1.01325));
            Assert.True(p4.Pascal.Equals4DigitPrecision(101325));
            Assert.True(p4.Psi.Equals4DigitPrecision(14.695948775));
        }
    }
}
