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
    public struct MagneticField3d : IUnitType, IFormattable, IComparable, IEquatable<(double ValueX, double ValueY, double ValueZ)>, IComparable<(double, double, double)>
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
            MagneticFieldX = new MagneticField(valueX, Unit);
            MagneticFieldY = new MagneticField(valueY, Unit);
            MagneticFieldZ = new MagneticField(valueZ, Unit);
        }

        public MagneticField3d(MagneticField magneticFieldX, MagneticField magneticFieldY, MagneticField magneticFieldZ)
        {
            MagneticFieldX = new MagneticField(magneticFieldX.Value, magneticFieldX.Unit);
            MagneticFieldY = new MagneticField(magneticFieldY.Value, magneticFieldY.Unit);
            MagneticFieldZ = new MagneticField(magneticFieldZ.Value, magneticFieldZ.Unit);

            Unit = magneticFieldX.Unit;
        }

        // TODO: why aren't these just XYZ?
        public MagneticField MagneticFieldX { get; set; }
        public MagneticField MagneticFieldY { get; set; }
        public MagneticField MagneticFieldZ { get; set; }

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
            MagneticFieldX == other.MagneticFieldX &&
            MagneticFieldY == other.MagneticFieldY &&
            MagneticFieldZ == other.MagneticFieldZ;


        [Pure] public override int GetHashCode() => (MagneticFieldX.GetHashCode() + MagneticFieldY.GetHashCode() + MagneticFieldZ.GetHashCode()) / 3;

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
            var x = lvalue.MagneticFieldX + rvalue.MagneticFieldX;
            var y = lvalue.MagneticFieldY + rvalue.MagneticFieldY;
            var z = lvalue.MagneticFieldZ + rvalue.MagneticFieldZ;

            return new MagneticField3d(x, y, z);
        }

        [Pure]
        public static MagneticField3d operator -(MagneticField3d lvalue, MagneticField3d rvalue)
        {
            var x = lvalue.MagneticFieldX - rvalue.MagneticFieldX;
            var y = lvalue.MagneticFieldY - rvalue.MagneticFieldY;
            var z = lvalue.MagneticFieldZ - rvalue.MagneticFieldZ;

            return new MagneticField3d(x, y, z);
        }

        [Pure] public override string ToString() => $"{MagneticFieldX}, {MagneticFieldY}, {MagneticFieldZ}";
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => $"{MagneticFieldX.ToString(format, formatProvider)}, {MagneticFieldY.ToString(format, formatProvider)}, {MagneticFieldZ.ToString(format, formatProvider)}";

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals((double ValueX, double ValueY, double ValueZ) other)
        {
            return MagneticFieldX.Equals(other.ValueX) &&
                   MagneticFieldY.Equals(other.ValueY) &&
                   MagneticFieldZ.Equals(other.ValueZ);
        }

        public int CompareTo((double, double, double) other)
        {
            throw new NotImplementedException();
        }
    }
}