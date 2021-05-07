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
    public struct MagneticField3d : IUnitType, IFormattable, IComparable, IEquatable<(double X, double Y, double Z)>, IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `MagneticField3d` object.
        /// </summary>
        /// <param name="x">The X MagneticField3d value.</param>
        /// <param name="y">The Y MagneticField3d value.</param>
        /// <param name="valueZ">The Z MagneticField3d value.</param>
        /// <param name="type"></param>
        public MagneticField3d(double x, double y, double z,
            MagneticField.UnitType type = MagneticField.UnitType.Telsa)
        {
            //always store reference value
            Unit = type;
            X = new MagneticField(x, Unit);
            Y = new MagneticField(y, Unit);
            Z = new MagneticField(z, Unit);
        }

        public MagneticField3d(MagneticField x, MagneticField y, MagneticField z)
        {
            X = new MagneticField(x.Value, x.Unit);
            Y = new MagneticField(y.Value, y.Unit);
            Z = new MagneticField(z.Value, z.Unit);

            Unit = x.Unit;
        }

        // TODO: why aren't these just XYZ?
        public MagneticField X { get; set; }
        public MagneticField Y { get; set; }
        public MagneticField Z { get; set; }

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
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;


        [Pure] public override int GetHashCode() => (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()) / 3;

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
            var x = lvalue.X + rvalue.X;
            var y = lvalue.Y + rvalue.Y;
            var z = lvalue.Z + rvalue.Z;

            return new MagneticField3d(x, y, z);
        }

        [Pure]
        public static MagneticField3d operator -(MagneticField3d lvalue, MagneticField3d rvalue)
        {
            var x = lvalue.X - rvalue.X;
            var y = lvalue.Y - rvalue.Y;
            var z = lvalue.Z - rvalue.Z;

            return new MagneticField3d(x, y, z);
        }

        [Pure] public override string ToString() => $"{X}, {Y}, {Z}";
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => $"{X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}";

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals((double X, double Y, double Z) other)
        {
            return X.Equals(other.X) &&
                   Y.Equals(other.Y) &&
                   Z.Equals(other.Z);
        }

        public int CompareTo((double, double, double) other)
        {
            throw new NotImplementedException();
        }
    }
}