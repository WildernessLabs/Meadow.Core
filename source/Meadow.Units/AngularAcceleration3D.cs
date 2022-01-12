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
        /// <param name="type">units of angular acceleration</param>
        public AngularAcceleration3D(double valueX, double valueY, double valueZ,
            AngularAcceleration.UnitType type = AngularAcceleration.UnitType.RadiansPerSecondSquared)
        {
            X = new AngularAcceleration(valueX, type);
            Y = new AngularAcceleration(valueY, type);
            Z = new AngularAcceleration(valueZ, type);
        }

        /// <summary>
        /// Creates a new `AngularAcceleration3d` object.
        /// </summary>
        /// <param name="x">The X AngularAcceleration value.</param>
        /// <param name="y">The Y AngularAcceleration value.</param>
        /// <param name="z">The Z AngularAcceleration value.</param>
        public AngularAcceleration3D(AngularAcceleration x, AngularAcceleration y, AngularAcceleration z)
        {
            X = new AngularAcceleration(x);
            Y = new AngularAcceleration(y);
            Z = new AngularAcceleration(z);
        }

        /// <summary>
        /// Creates a new `AngularAcceleration3D` object from an existing object.
        /// </summary>
        /// <param name="angularAcceleration3D">Source object</param>
        public AngularAcceleration3D(AngularAcceleration3D angularAcceleration3D)
        {
            this.X = new AngularAcceleration(angularAcceleration3D.X);
            this.Y = new AngularAcceleration(angularAcceleration3D.Y);
            this.Z = new AngularAcceleration(angularAcceleration3D.Z);
        }

        /// <summary>
        /// X component of angular acceleration
        /// </summary>
        public AngularAcceleration X { get; set; }
        /// <summary>
        /// Y component of angular acceleration
        /// </summary>
        public AngularAcceleration Y { get; set; }
        /// <summary>
        /// Z component of angular acceleration
        /// </summary>
        public AngularAcceleration Z { get; set; }

        /// <summary>
        /// Compare to another AngularAcceleration3D object
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true if equal</returns>
        [Pure] public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((AngularAcceleration3D)obj);
        }

        /// <summary>
        /// Get hash of object
        /// </summary>
        /// <returns>int32 hash value</returns>
        [Pure] public override int GetHashCode() => (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()) / 3;

        // Comparison
        /// <summary>
        /// Compare to another AngularAcceleration object
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public bool Equals(AngularAcceleration3D other) =>
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;

        /// <summary>
        /// Equals operator to compare two AngularAcceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if equal</returns>
        [Pure] public static bool operator ==(AngularAcceleration3D left, AngularAcceleration3D right) => Equals(left, right);

        /// <summary>
        /// Not equals operator to compare two AngularAcceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if not equal</returns>
        [Pure] public static bool operator !=(AngularAcceleration3D left, AngularAcceleration3D right) => !Equals(left, right);
  
        /// <summary>
        /// Less than operator to compare two AngularAcceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than right</returns>
        [Pure] public static bool operator <(AngularAcceleration3D left, AngularAcceleration3D right) => Comparer<AngularAcceleration3D>.Default.Compare(left, right) < 0;

        /// <summary>
        /// Greater than operator to compare two AngularAcceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than right</returns>
        [Pure] public static bool operator >(AngularAcceleration3D left, AngularAcceleration3D right) => Comparer<AngularAcceleration3D>.Default.Compare(left, right) > 0;

        /// <summary>
        /// Less than or equal operator to compare two AngularAcceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than or equal to right</returns>
        [Pure] public static bool operator <=(AngularAcceleration3D left, AngularAcceleration3D right) => Comparer<AngularAcceleration3D>.Default.Compare(left, right) <= 0;

        /// <summary>
        /// Greater than or equal operator to compare two AngularAcceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than or equal to right</returns>
        [Pure] public static bool operator >=(AngularAcceleration3D left, AngularAcceleration3D right) => Comparer<AngularAcceleration3D>.Default.Compare(left, right) >= 0;

        // Math
        /// <summary>
        /// Addition operator to add two AngularAcceleration objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new AngularAcceleration3D object with a value of left + right</returns>
        [Pure] public static AngularAcceleration3D operator +(AngularAcceleration3D left, AngularAcceleration3D right)
        {
            var x = left.X + right.X;
            var y = left.Y + right.Y;
            var z = left.Z + right.Z;

            return new AngularAcceleration3D(x, y, z);
        }

        /// <summary>
        /// Subtraction operator to subtract two AngularAcceleration3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new AngularAcceleration3D object with a value of left - right</returns>
        [Pure] public static AngularAcceleration3D operator -(AngularAcceleration3D left, AngularAcceleration3D right)
        {
            var x = left.X - right.X;
            var y = left.Y - right.Y;
            var z = left.Z - right.Z;

            return new AngularAcceleration3D(x, y, z);
        }

        /// <summary>
        /// Get a string represention of the object
        /// </summary>
        /// <returns>A string representing the object</returns>
        [Pure] public override string ToString() => $"{X}, {Y}, {Z}";

        /// <summary>
        /// Get a string represention of the object
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="formatProvider">format provider</param>
        /// <returns>A string representing the object</returns>
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => $"{X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}";

        // IComparable
        /// <summary>
        /// Compare to another AngularAcceleration3D object
        /// </summary>
        /// <param name="obj">The other AngularAcceleration3D cast to object</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compare the default value to three double 
        /// </summary>
        /// <param name="other">values to compare</param>
        /// <returns>true if equal</returns>
        public bool Equals((double ValueX, double ValueY, double ValueZ) other)
        {
            return X.Equals(other.ValueX) &&
                Y.Equals(other.ValueY) &&
                Z.Equals(other.ValueZ);
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