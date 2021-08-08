using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents AngularVelocity3d
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct AngularVelocity3D :
        IFormattable, IComparable,
        IEquatable<(double ValueX, double ValueY, double ValueZ)>,
        IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `AngularVelocity3d` object.
        /// </summary>
        /// <param name="valueX">The X AngularVelocity3d value.</param>
        /// <param name="valueY">The Y AngularVelocity3d value.</param>
        /// <param name="valueZ">The Z AngularVelocity3d value.</param>
        /// <param name="type"></param>
        public AngularVelocity3D(double valueX, double valueY, double valueZ,
            AngularVelocity.UnitType type = AngularVelocity.UnitType.RevolutionsPerSecond)
        {
            //always store reference value
            X = new AngularVelocity(valueX, type);
            Y = new AngularVelocity(valueY, type);
            Z = new AngularVelocity(valueZ, type);
        }

        public AngularVelocity3D(AngularVelocity x, AngularVelocity y, AngularVelocity z)
        {
            X = new AngularVelocity(x);
            Y = new AngularVelocity(y);
            Z = new AngularVelocity(z);
        }

        public AngularVelocity3D(AngularVelocity3D angularVelocity3D)
        {
            this.X = new AngularVelocity(angularVelocity3D.X);
            this.Y = new AngularVelocity(angularVelocity3D.Y);
            this.Z = new AngularVelocity(angularVelocity3D.Z);
        }

        public AngularVelocity X { get; set; }
        public AngularVelocity Y { get; set; }
        public AngularVelocity Z { get; set; }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((AngularVelocity3D)obj);
        }

        [Pure]
        public bool Equals(AngularVelocity3D other) =>
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;


        [Pure] public override int GetHashCode() => (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()) / 3;

        [Pure] public static bool operator ==(AngularVelocity3D left, AngularVelocity3D right) => Equals(left, right);
        [Pure] public static bool operator !=(AngularVelocity3D left, AngularVelocity3D right) => !Equals(left, right);
        //ToDo [Pure] public int CompareTo(AngularVelocity3d other) => Equals(this, other) ? 0 : VelocityX.CompareTo(other.VelocityX);
        [Pure] public static bool operator <(AngularVelocity3D left, AngularVelocity3D right) => Comparer<AngularVelocity3D>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(AngularVelocity3D left, AngularVelocity3D right) => Comparer<AngularVelocity3D>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(AngularVelocity3D left, AngularVelocity3D right) => Comparer<AngularVelocity3D>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(AngularVelocity3D left, AngularVelocity3D right) => Comparer<AngularVelocity3D>.Default.Compare(left, right) >= 0;

        [Pure]
        public static AngularVelocity3D operator +(AngularVelocity3D lvalue, AngularVelocity3D rvalue)
        {
            var x = lvalue.X + rvalue.X;
            var y = lvalue.Y + rvalue.Y;
            var z = lvalue.Z + rvalue.Z;

            return new AngularVelocity3D(x, y, z);
        }

        [Pure]
        public static AngularVelocity3D operator -(AngularVelocity3D lvalue, AngularVelocity3D rvalue)
        {
            var x = lvalue.X - rvalue.X;
            var y = lvalue.Y - rvalue.Y;
            var z = lvalue.Z - rvalue.Z;

            return new AngularVelocity3D(x, y, z);
        }

        [Pure] public override string ToString() => $"{X}, {Y}, {Z}";
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => $"{X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}";

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals((double ValueX, double ValueY, double ValueZ) other)
        {
            return X.Equals(other.ValueX) &&
                Y.Equals(other.ValueY) &&
                Z.Equals(other.ValueZ);
        }

        public int CompareTo((double, double, double) other)
        {
            throw new NotImplementedException();
        }
    }
}