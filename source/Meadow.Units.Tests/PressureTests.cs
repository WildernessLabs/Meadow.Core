using NUnit.Framework;
using System;
using Meadow.Units;

namespace Meadow.Units.Tests
{
    [TestFixture()]
    public class PressureTests
    {
        [Test()]
        public void PressureCtors()
        {
            Pressure p = new Pressure(1, Pressure.UnitType.StandardAtmosphere);
            Assert.That(p.StandardAtmosphere == 1);

            Pressure p2 = new Pressure();
            Assert.That(p2.Bar == 0);

            Pressure p3 = Pressure.FromBar(1);
            Assert.That(p3.Bar == 1);
        }

        [Test()]
        public void Conversions()
        {
            Pressure p = new Pressure(1, Pressure.UnitType.Bar);
            Assert.That(p.Pascal == 100000);
            Assert.That(p.Psi.Equals4DigitPrecision(14.503773773));
            Assert.That(p.StandardAtmosphere.Equals4DigitPrecision(0.9869233));

            Pressure p2 = new Pressure(1, Pressure.UnitType.Pascal);
            Assert.That(p2.Bar.Equals4DigitPrecision(0.000001));
            Assert.That(p2.Psi.Equals4DigitPrecision(0.000145038));
            Assert.That(p2.StandardAtmosphere.Equals4DigitPrecision(0.00000098692));

            Pressure p3 = new Pressure(1, Pressure.UnitType.Psi);
            Assert.That(p3.Bar.Equals4DigitPrecision(0.0689476));
            Assert.That(p3.Pascal.Equals4DigitPrecision(6894.7572931783));
            Assert.That(p3.StandardAtmosphere.Equals4DigitPrecision(0.068046));

            Pressure p4 = new Pressure(1, Pressure.UnitType.StandardAtmosphere);
            Assert.That(p4.Bar.Equals4DigitPrecision(1.01325));
            Assert.That(p4.Pascal.Equals4DigitPrecision(101325));
            Assert.That(p4.Psi.Equals4DigitPrecision(14.695948775));
        }
    }
}
