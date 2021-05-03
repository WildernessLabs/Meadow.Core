using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents MagneticField3d
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public class MagneticField3d : IUnitType, IFormattable, IComparable, IEquatable<(double ValueX, double ValueY, double ValueZ)>, IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `MagneticField3d` object.
        /// </summary>
        /// <param name="valueX">The X MagneticField3d value.</param>
        /// <param name="valueY">The Y MagneticField3d value.</param>
        /// <param name="valueZ">The Z MagneticField3d value.</param>
        /// <param name="type"></param>
        public MagneticField3d(double valueX, double valueY, double valueZ,
            MagneticField.UnitType type = MagneticField.UnitType.Telsa)
        {
            //always store reference value
            Unit = type;
            magneticFieldX = new MagneticField(valueX, Unit);
            magneticFieldY = new MagneticField(valueY, Unit);
            magneticFieldZ = new MagneticField(valueZ, Unit);
        }

        public MagneticField3d()
        {

        }

        public MagneticField3d(MagneticField magneticFieldX, MagneticField magneticFieldY, MagneticField magneticFieldZ)
        {
            magneticFieldX = new MagneticField(magneticFieldX.Value, magneticFieldX.Unit);
            magneticFieldY = new MagneticField(magneticFieldY.Value, magneticFieldY.Unit);
            magneticFieldZ = new MagneticField(magneticFieldZ.Value, magneticFieldZ.Unit);

            Unit = magneticFieldX.Unit;
        }

        public MagneticField magneticFieldX { get; set; }
        public MagneticField magneticFieldY { get; set; }
        public MagneticField magneticFieldZ { get; set; }

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public MagneticField.UnitType Unit { get; set; }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((MagneticField3d)obj);
        }

        [Pure]
        public bool Equals(MagneticField3d other) =>
            magneticFieldX == other.magneticFieldX &&
            magneticFieldY == other.magneticFieldY &&
            magneticFieldZ == other.magneticFieldZ;


        [Pure] public override int GetHashCode() => (magneticFieldX.GetHashCode() + magneticFieldY.GetHashCode() + magneticFieldZ.GetHashCode()) / 3;

        [Pure] public static bool operator ==(MagneticField3d left, MagneticField3d right) => Equals(left, right);
        [Pure] public static bool operator !=(MagneticField3d left, MagneticField3d right) => !Equals(left, right);
        //ToDo [Pure] public int CompareTo(MagneticField3d other) => Equals(this, other) ? 0 : magneticFieldX.CompareTo(other.magneticFieldX);
        [Pure] public static bool operator <(MagneticField3d left, MagneticField3d right) => Comparer<MagneticField3d>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(MagneticField3d left, MagneticField3d right) => Comparer<MagneticField3d>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(MagneticField3d left, MagneticField3d right) => Comparer<MagneticField3d>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(MagneticField3d left, MagneticField3d right) => Comparer<MagneticField3d>.Default.Compare(left, right) >= 0;

        [Pure]
        public static MagneticField3d operator +(MagneticField3d lvalue, MagneticField3d rvalue)
        {
            var x = lvalue.magneticFieldX + rvalue.magneticFieldX;
            var y = lvalue.magneticFieldY + rvalue.magneticFieldY;
            var z = lvalue.magneticFieldZ + rvalue.magneticFieldZ;

            return new MagneticField3d(x, y, z);
        }

        [Pure]
        public static MagneticField3d operator -(MagneticField3d lvalue, MagneticField3d rvalue)
        {
            var x = lvalue.magneticFieldX - rvalue.magneticFieldX;
            var y = lvalue.magneticFieldY - rvalue.magneticFieldY;
            var z = lvalue.magneticFieldZ - rvalue.magneticFieldZ;

            return new MagneticField3d(x, y, z);
        }

        [Pure] public override string ToString() => $"{magneticFieldX}, {magneticFieldY}, {magneticFieldZ}";
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => $"{magneticFieldX.ToString(format, formatProvider)}, {magneticFieldY.ToString(format, formatProvider)}, {magneticFieldZ.ToString(format, formatProvider)}";

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals((double ValueX, double ValueY, double ValueZ) other)
        {
            return magneticFieldX.Equals(other.ValueX) &&
                magneticFieldY.Equals(other.ValueY) &&
                magneticFieldZ.Equals(other.ValueZ);
        }

        public int CompareTo((double, double, double) other)
        {
            throw new NotImplementedException();
        }
    }
}