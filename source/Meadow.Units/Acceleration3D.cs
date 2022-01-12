using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Acceleration3d
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Acceleration3D :
        IFormattable, IComparable,
        IEquatable<(double X, double Y, double Z)>,
        IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `Acceleration3d` object.
        /// </summary>
        /// <param name="x">The X Acceleration3d value.</param>
        /// <param name="y">The Y Acceleration3d value.</param>
        /// <param name="z">The Z Acceleration3d value.</param>
        /// <param name="type">Acceleration unit</param>
        public Acceleration3D(double x, double y, double z,
            Acceleration.UnitType type = Acceleration.UnitType.MetersPerSecondSquared)
        {
            X = new Acceleration(x, type);
            Y = new Acceleration(y, type);
            Z = new Acceleration(z, type);
        }

        /// <summary>
        /// Creates a new `Acceleration3d` object.
        /// </summary>
        /// <param name="x">The X Acceleration3d value.</param>
        /// <param name="y">The Y Acceleration3d value.</param>
        /// <param name="z">The Z Acceleration3d value.</param>
        public Acceleration3D(Acceleration x, Acceleration y, Acceleration z)
        {
            X = new Acceleration(x);
            Y = new Acceleration(y);
            Z = new Acceleration(z);
        }

        /// <summary>
        /// Creates a new `Acceleration3d` object. 
        /// </summary>
        /// <param name="acceleration3D">Create a new object using the values from another object</param>
        public Acceleration3D(Acceleration3D acceleration3D)
        {
            X = new Acceleration(acceleration3D.X);
            Y = new Acceleration(acceleration3D.Y);
            Z = new Acceleration(acceleration3D.Z);
        }

        /// <summary>
        /// X component of accleration
        /// </summary>
        public Acceleration X { get; set; }
        /// <summary>
        /// Y component of acceleration
        /// </summary>
        public Acceleration Y { get; set; }
        /// <summary>
        /// Z component of acceleration
        /// </summary>
        public Acceleration Z { get; set; }

        /// <summary>
        /// Compare to another acceleration object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if equals</returns>
        [Pure] public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Acceleration3D)obj);
        }

        /// <summary>
        /// Get hash object
        /// </summary>
        /// <returns>hash as int32</returns>
        [Pure] public override int GetHashCode() => (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()) / 3;

        /// <summary>
        /// Compare to another acceleration object
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if equals</returns>
        [Pure] public bool Equals(Acceleration3D other) =>
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;

        /// <summary>
        /// Equals operator to compare two Acceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if equal</returns>
        [Pure] public static bool operator ==(Acceleration3D left, Acceleration3D right) => Equals(left, right);

        /// <summary>
        /// Not equals operator to compare two Acceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if not equal</returns>
        [Pure] public static bool operator !=(Acceleration3D left, Acceleration3D right) => !Equals(left, right);

        /// <summary>
        /// Less than operator to compare two Acceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than right</returns>
        [Pure] public static bool operator <(Acceleration3D left, Acceleration3D right) => Comparer<Acceleration3D>.Default.Compare(left, right) < 0;

        /// <summary>
        /// Greater than operator to compare two Acceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than right</returns>
        [Pure] public static bool operator >(Acceleration3D left, Acceleration3D right) => Comparer<Acceleration3D>.Default.Compare(left, right) > 0;

        /// <summary>
        /// Less than or equal operator to compare two Acceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than or equal to right</returns>
        [Pure] public static bool operator <=(Acceleration3D left, Acceleration3D right) => Comparer<Acceleration3D>.Default.Compare(left, right) <= 0;

        /// <summary>
        /// Greater than or equal operator to compare two Acceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than or equal to right</returns>
        [Pure] public static bool operator >=(Acceleration3D left, Acceleration3D right) => Comparer<Acceleration3D>.Default.Compare(left, right) >= 0;

        // Math
        /// <summary>
        /// Addition operator to add two Acceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new Acceleration3D object with a value of left + right</returns>
        [Pure] public static Acceleration3D operator +(Acceleration3D lvalue, Acceleration3D rvalue)
        {
            var x = lvalue.X + rvalue.X;
            var y = lvalue.Y + rvalue.Y;
            var z = lvalue.Z + rvalue.Z;

            return new Acceleration3D(x, y, z);
        }


        /// <summary>
        /// Subtraction operator to subtract two Acceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new Acceleration3D object with a value of left - right</returns>
        [Pure] public static Acceleration3D operator -(Acceleration3D left, Acceleration3D right)
        {
            var x = left.X - right.X;
            var y = left.Y - right.Y;
            var z = left.Z - right.Z;

            return new Acceleration3D(x, y, z);
        }


        /// <summary>
        /// Covert to string
        /// </summary>
        [Pure] public override string ToString() => $"{X}, {Y}, {Z}";

        /// <summary>
        /// Covert to string
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>string representation of the object</returns>
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => $"{X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}";

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Compare the default value to three double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        public bool Equals((double X, double Y, double Z) other)
        {
            return X.Equals(other.X) &&
                Y.Equals(other.Y) &&
                Z.Equals(other.Z);
        }


        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        public int CompareTo((double, double, double) other)
        {
            throw new NotImplementedException();
        }
    }
}