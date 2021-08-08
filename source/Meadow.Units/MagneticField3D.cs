using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;
using MU = Meadow.Units.MagneticField.UnitType;

namespace Meadow.Units
{
    /// <summary>
    /// Represents a 3-dimensional (X,Y,Z) magnetic field.
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct MagneticField3D :
        IFormattable, IComparable,
        IEquatable<(double X, double Y, double Z)>, IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `MagneticField3d` object.
        /// </summary>
        /// <param name="x">The X MagneticField3d value.</param>
        /// <param name="y">The Y MagneticField3d value.</param>
        /// <param name="valueZ">The Z MagneticField3d value.</param>
        /// <param name="type"></param>
        public MagneticField3D(double x, double y, double z, MU type = MU.Tesla)
        {
            X = new MagneticField(x, type);
            Y = new MagneticField(y, type);
            Z = new MagneticField(z, type);
        }

        public MagneticField3D(MagneticField x, MagneticField y, MagneticField z)
        {
            X = new MagneticField(x);
            Y = new MagneticField(y);
            Z = new MagneticField(z);
        }

        public MagneticField3D(MagneticField3D magneticField3D)
        {
            this.X = new MagneticField(magneticField3D.X);
            this.Y = new MagneticField(magneticField3D.Y);
            this.Z = new MagneticField(magneticField3D.Z);
        }

        public MagneticField X { get; set; }
        public MagneticField Y { get; set; }
        public MagneticField Z { get; set; }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((MagneticField3D)obj);
        }

        [Pure]
        public bool Equals(MagneticField3D other) =>
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;


        [Pure] public override int GetHashCode() => (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()) / 3;

        [Pure] public static bool operator ==(MagneticField3D left, MagneticField3D right) => Equals(left, right);
        [Pure] public static bool operator !=(MagneticField3D left, MagneticField3D right) => !Equals(left, right);
        //ToDo [Pure] public int CompareTo(MagneticField3d other) => Equals(this, other) ? 0 : magneticFieldX.CompareTo(other.magneticFieldX);
        [Pure] public static bool operator <(MagneticField3D left, MagneticField3D right) => Comparer<MagneticField3D>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(MagneticField3D left, MagneticField3D right) => Comparer<MagneticField3D>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(MagneticField3D left, MagneticField3D right) => Comparer<MagneticField3D>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(MagneticField3D left, MagneticField3D right) => Comparer<MagneticField3D>.Default.Compare(left, right) >= 0;

        [Pure]
        public static MagneticField3D operator +(MagneticField3D lvalue, MagneticField3D rvalue)
        {
            var x = lvalue.X + rvalue.X;
            var y = lvalue.Y + rvalue.Y;
            var z = lvalue.Z + rvalue.Z;

            return new MagneticField3D(x, y, z);
        }

        [Pure]
        public static MagneticField3D operator -(MagneticField3D lvalue, MagneticField3D rvalue)
        {
            var x = lvalue.X - rvalue.X;
            var y = lvalue.Y - rvalue.Y;
            var z = lvalue.Z - rvalue.Z;

            return new MagneticField3D(x, y, z);
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