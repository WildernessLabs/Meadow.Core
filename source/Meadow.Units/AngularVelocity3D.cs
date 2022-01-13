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
        /// <param name="valueX">The X AngularVelocity value.</param>
        /// <param name="valueY">The Y AngularVelocity value.</param>
        /// <param name="valueZ">The Z AngularVelocity value.</param>
        /// <param name="type"></param>
        public AngularVelocity3D(double valueX, double valueY, double valueZ,
            AngularVelocity.UnitType type = AngularVelocity.UnitType.RevolutionsPerSecond)
        {
            X = new AngularVelocity(valueX, type);
            Y = new AngularVelocity(valueY, type);
            Z = new AngularVelocity(valueZ, type);
        }

        /// <summary>
        /// Creates a new `AngularVelocity3d` object.
        /// </summary>
        /// <param name="x">The X AngularVelocity component.</param>
        /// <param name="y">The Y AngularVelocity component.</param>
        /// <param name="z">The Z AngularVelocity component.</param>
        public AngularVelocity3D(AngularVelocity x, AngularVelocity y, AngularVelocity z)
        {
            X = new AngularVelocity(x);
            Y = new AngularVelocity(y);
            Z = new AngularVelocity(z);
        }

        /// <summary>
        /// Creates a new `AngularVelocity3d` object with values from another object
        /// </summary>
        /// <param name="angularVelocity3D">angularVelocity3D source object.</param>
        public AngularVelocity3D(AngularVelocity3D angularVelocity3D)
        {
            this.X = new AngularVelocity(angularVelocity3D.X);
            this.Y = new AngularVelocity(angularVelocity3D.Y);
            this.Z = new AngularVelocity(angularVelocity3D.Z);
        }

        /// <summary>
        /// X component of angular velocity
        /// </summary>
        public AngularVelocity X { get; set; }
        /// <summary>
        /// Y component of angular velocity
        /// </summary>
        public AngularVelocity Y { get; set; }
        /// <summary>
        /// Z component of angular velocity
        /// </summary>
        public AngularVelocity Z { get; set; }

        /// <summary>
        /// Compare two 3DAngularVelocity objects
        /// </summary>
        /// <param name="obj">object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((AngularVelocity3D)obj);
        }

        /// <summary>
        /// Get hash of object
        /// </summary>
        /// <returns>hash as 32 bit integer</returns>
        [Pure] public override int GetHashCode() => (X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode()) / 3;
		
		/// <summary>
        /// Compare two 3DAngularVelocity objects
        /// </summary>
        /// <param name="other">object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public bool Equals(AngularVelocity3D other) =>
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;

        /// <summary>
        /// Equals operator to compare two AngularVelocity3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if equal</returns>
        [Pure] public static bool operator ==(AngularVelocity3D left, AngularVelocity3D right) => Equals(left, right);

        /// <summary>
        /// Not equals operator to compare two AngularVelocity3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if not equal</returns>
        [Pure] public static bool operator !=(AngularVelocity3D left, AngularVelocity3D right) => !Equals(left, right);
  
        /// <summary>
        /// Less than operator to compare two AngularVelocity3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than right</returns>
        [Pure] public static bool operator <(AngularVelocity3D left, AngularVelocity3D right) => Comparer<AngularVelocity3D>.Default.Compare(left, right) < 0;

        /// <summary>
        /// Greater than operator to compare two AngularVelocity3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than right</returns>
        [Pure] public static bool operator >(AngularVelocity3D left, AngularVelocity3D right) => Comparer<AngularVelocity3D>.Default.Compare(left, right) > 0;

        /// <summary>
        /// Less than or equal operator to compare two AngularVelocity3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than or equal to right</returns>
        [Pure] public static bool operator <=(AngularVelocity3D left, AngularVelocity3D right) => Comparer<AngularVelocity3D>.Default.Compare(left, right) <= 0;

        /// <summary>
        /// Greater than or equal operator to compare two AngularVelocity3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than or equal to right</returns>
        [Pure] public static bool operator >=(AngularVelocity3D left, AngularVelocity3D right) => Comparer<AngularVelocity3D>.Default.Compare(left, right) >= 0;

        // Math
        /// <summary>
        /// Addition operator to add two AngularAcceleration objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new AngularVelocity3D object with a value of left + right</returns>
        [Pure] public static AngularVelocity3D operator +(AngularVelocity3D left, AngularVelocity3D right)
        {
            var x = left.X + right.X;
            var y = left.Y + right.Y;
            var z = left.Z + right.Z;

            return new AngularVelocity3D(x, y, z);
        }

        /// <summary>
        /// Subtraction operator to subtract two AngularVelocity3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new AngularVelocity3D object with a value of left - right</returns>
        [Pure] public static AngularVelocity3D operator -(AngularVelocity3D left, AngularVelocity3D right)
        {
            var x = left.X - right.X;
            var y = left.Y - right.Y;
            var z = left.Z - right.Z;

            return new AngularVelocity3D(x, y, z);
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
        /// Compare to another AngularVelocity3D object
        /// </summary>
        /// <param name="obj">The other AngularVelocity3D cast to object</param>
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