using NUnit.Framework;
using System;
using Meadow.Units;
using static Meadow.Units.Tests.Helpers;

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
            Assert.That(Equals4DigitPrecision(p.Psi, 14.503773773));
            Assert.That(Equals4DigitPrecision(p.StandardAtmosphere, 0.9869233));

            Pressure p2 = new Pressure(1, Pressure.UnitType.Pascal);
            Assert.That(Equals4DigitPrecision(p2.Bar, 0.000001));
            Assert.That(Equals4DigitPrecision(p2.Psi, 0.000145038));
            Assert.That(Equals4DigitPrecision(p2.StandardAtmosphere, 0.00000098692));

            Pressure p3 = new Pressure(1, Pressure.UnitType.Psi);
            Assert.That(Equals4DigitPrecision(p3.Bar, 0.0689476));
            Assert.That(Equals4DigitPrecision(p3.Pascal, 6894.7572931783));
            Assert.That(Equals4DigitPrecision(p3.StandardAtmosphere, 0.068046));

            Pressure p4 = new Pressure(1, Pressure.UnitType.StandardAtmosphere);
            Assert.That(Equals4DigitPrecision(p4.Bar, 1.01325));
            Assert.That(Equals4DigitPrecision(p4.Pascal, 101325));
            Assert.That(Equals4DigitPrecision(p4.Psi, 14.695948775));
        }
    }
}
