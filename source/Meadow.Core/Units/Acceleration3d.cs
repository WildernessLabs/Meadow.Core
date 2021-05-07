using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Acceleration3d
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Acceleration3d : IUnitType, IFormattable, IComparable, IEquatable<(double X, double Y, double Z)>, IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `Acceleration3d` object.
        /// </summary>
        /// <param name="x">The X Acceleration3d value.</param>
        /// <param name="y">The Y Acceleration3d value.</param>
        /// <param name="y">The Z Acceleration3d value.</param>
        /// <param name="type"></param>
        public Acceleration3d(double x, double y, double z,
            Acceleration.UnitType type = Acceleration.UnitType.MetersPerSecondSquared)
        {
            //always store reference value
            Unit = type;
            X = new Acceleration(x, Unit);
            Y = new Acceleration(y, Unit);
            Z = new Acceleration(z, Unit);
        }

        public Acceleration3d(Acceleration x, Acceleration y, Acceleration z)
        {
            X = new Acceleration(x.Value, x.Unit);
            Y = new Acceleration(y.Value, y.Unit);
            Z = new Acceleration(z.Value, z.Unit);

            Unit = X.Unit;
        }

        public Acceleration X { get; set; }
        public Acceleration Y { get; set; }
        public Acceleration Z { get; set; }

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public Acceleration.UnitType Unit { get; set; }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Acceleration3d)obj);
        }

        [Pure]
        public bool Equals(Acceleration3d other) =>
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;


        [Pure] public override int GetHashCode() => (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()) / 3;

        [Pure] public static bool operator ==(Acceleration3d left, Acceleration3d right) => Equals(left, right);
        [Pure] public static bool operator !=(Acceleration3d left, Acceleration3d right) => !Equals(left, right);
        //ToDo [Pure] public int CompareTo(Acceleration3d other) => Equals(this, other) ? 0 : AccelerationX.CompareTo(other.AccelerationX);
        [Pure] public static bool operator <(Acceleration3d left, Acceleration3d right) => Comparer<Acceleration3d>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(Acceleration3d left, Acceleration3d right) => Comparer<Acceleration3d>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(Acceleration3d left, Acceleration3d right) => Comparer<Acceleration3d>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(Acceleration3d left, Acceleration3d right) => Comparer<Acceleration3d>.Default.Compare(left, right) >= 0;

        [Pure]
        public static Acceleration3d operator +(Acceleration3d lvalue, Acceleration3d rvalue)
        {
            var x = lvalue.X + rvalue.X;
            var y = lvalue.Y + rvalue.Y;
            var z = lvalue.Z + rvalue.Z;

            return new Acceleration3d(x, y, z);
        }

        [Pure]
        public static Acceleration3d operator -(Acceleration3d lvalue, Acceleration3d rvalue)
        {
            var x = lvalue.X - rvalue.X;
            var y = lvalue.Y - rvalue.Y;
            var z = lvalue.Z - rvalue.Z;

            return new Acceleration3d(x, y, z);
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