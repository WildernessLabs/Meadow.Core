using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents AngularAcceleration3d
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct AngularAcceleration3D :
        IFormattable, IComparable,
        IEquatable<(double ValueX, double ValueY, double ValueZ)>,
        IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `AngularAcceleration3d` object.
        /// </summary>
        /// <param name="valueX">The X AngularAcceleration3d value.</param>
        /// <param name="valueY">The Y AngularAcceleration3d value.</param>
        /// <param name="valueZ">The Z AngularAcceleration3d value.</param>
        /// <param name="type"></param>
        public AngularAcceleration3D(double valueX, double valueY, double valueZ,
            AngularAcceleration.UnitType type = AngularAcceleration.UnitType.RadiansPerSecondSquared)
        {
            //always store reference value
            X = new AngularAcceleration(valueX, type);
            Y = new AngularAcceleration(valueY, type);
            Z = new AngularAcceleration(valueZ, type);
        }

        public AngularAcceleration3D(AngularAcceleration x, AngularAcceleration y, AngularAcceleration z)
        {
            X = new AngularAcceleration(x);
            Y = new AngularAcceleration(y);
            Z = new AngularAcceleration(z);
        }

        public AngularAcceleration3D(AngularAcceleration3D angularAcceleration3D)
        {
            this.X = new AngularAcceleration(angularAcceleration3D.X);
            this.Y = new AngularAcceleration(angularAcceleration3D.Y);
            this.Z = new AngularAcceleration(angularAcceleration3D.Z);
        }

        public AngularAcceleration X { get; set; }
        public AngularAcceleration Y { get; set; }
        public AngularAcceleration Z { get; set; }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((AngularAcceleration3D)obj);
        }

        [Pure]
        public bool Equals(AngularAcceleration3D other) =>
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;


        [Pure] public override int GetHashCode() => (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()) / 3;

        [Pure] public static bool operator ==(AngularAcceleration3D left, AngularAcceleration3D right) => Equals(left, right);
        [Pure] public static bool operator !=(AngularAcceleration3D left, AngularAcceleration3D right) => !Equals(left, right);
        //ToDo [Pure] public int CompareTo(AngularAcceleration3d other) => Equals(this, other) ? 0 : AccelerationX.CompareTo(other.AccelerationX);
        [Pure] public static bool operator <(AngularAcceleration3D left, AngularAcceleration3D right) => Comparer<AngularAcceleration3D>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(AngularAcceleration3D left, AngularAcceleration3D right) => Comparer<AngularAcceleration3D>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(AngularAcceleration3D left, AngularAcceleration3D right) => Comparer<AngularAcceleration3D>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(AngularAcceleration3D left, AngularAcceleration3D right) => Comparer<AngularAcceleration3D>.Default.Compare(left, right) >= 0;

        [Pure]
        public static AngularAcceleration3D operator +(AngularAcceleration3D lvalue, AngularAcceleration3D rvalue)
        {
            var x = lvalue.X + rvalue.X;
            var y = lvalue.Y + rvalue.Y;
            var z = lvalue.Z + rvalue.Z;

            return new AngularAcceleration3D(x, y, z);
        }

        [Pure]
        public static AngularAcceleration3D operator -(AngularAcceleration3D lvalue, AngularAcceleration3D rvalue)
        {
            var x = lvalue.X - rvalue.X;
            var y = lvalue.Y - rvalue.Y;
            var z = lvalue.Z - rvalue.Z;

            return new AngularAcceleration3D(x, y, z);
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