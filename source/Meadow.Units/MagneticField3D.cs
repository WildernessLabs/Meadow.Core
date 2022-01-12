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
        IEquatable<(double X, double Y, double Z)>, 
		IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `MagneticField3d` object.
        /// </summary>
        /// <param name="x">The X MagneticField3d value.</param>
        /// <param name="y">The Y MagneticField3d value.</param>
        /// <param name="z">The Z MagneticField3d value.</param>
        /// <param name="type"></param>
        public MagneticField3D(double x, double y, double z, MU type = MU.Tesla)
        {
            X = new MagneticField(x, type);
            Y = new MagneticField(y, type);
            Z = new MagneticField(z, type);
        }

        /// <summary>
        /// Creates a new `MagneticField3D` object.
        /// </summary>
        /// <param name="x">The X MagneticField value.</param>
        /// <param name="y">The Y MagneticField value.</param>
        /// <param name="z">The Z MagneticField value.</param>
        public MagneticField3D(MagneticField x, MagneticField y, MagneticField z)
        {
            X = new MagneticField(x);
            Y = new MagneticField(y);
            Z = new MagneticField(z);
        }

        /// <summary>
        /// Creates a new `MagneticField3D` object from an existing object.
        /// </summary>
        /// <param name="magneticField3D">Source object</param>
        public MagneticField3D(MagneticField3D magneticField3D)
        {
            this.X = new MagneticField(magneticField3D.X);
            this.Y = new MagneticField(magneticField3D.Y);
            this.Z = new MagneticField(magneticField3D.Z);
        }

        /// <summary>
        /// X component of the magnetic field
        /// </summary>
        public MagneticField X { get; set; }
        /// <summary>
        /// Y component of the magnetic field
        /// </summary>
        public MagneticField Y { get; set; }
        /// <summary>
        /// Z component of the magnetic field
        /// </summary>
        public MagneticField Z { get; set; }

        /// <summary>
        /// Compare to another MagneticField3D object
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true if equal</returns>
        [Pure] public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((MagneticField3D)obj);
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
        [Pure] public bool Equals(MagneticField3D other) =>
            X == other.X &&
            Y == other.Y &&
            Z == other.Z;

        /// <summary>
        /// Equals operator to compare two MagneticField3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if equal</returns>
        [Pure] public static bool operator ==(MagneticField3D left, MagneticField3D right) => Equals(left, right);

        /// <summary>
        /// Not equals operator to compare two MagneticField3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if not equal</returns>
        [Pure] public static bool operator !=(MagneticField3D left, MagneticField3D right) => !Equals(left, right);
  
        /// <summary>
        /// Less than operator to compare two MagneticField3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than right</returns>
        [Pure] public static bool operator <(MagneticField3D left, MagneticField3D right) => Comparer<MagneticField3D>.Default.Compare(left, right) < 0;

        /// <summary>
        /// Greater than operator to compare two MagneticField3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than right</returns>
        [Pure] public static bool operator >(MagneticField3D left, MagneticField3D right) => Comparer<MagneticField3D>.Default.Compare(left, right) > 0;

        /// <summary>
        /// Less than or equal operator to compare two MagneticField3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than or equal to right</returns>
        [Pure] public static bool operator <=(MagneticField3D left, MagneticField3D right) => Comparer<MagneticField3D>.Default.Compare(left, right) <= 0;

        /// <summary>
        /// Greater than or equal operator to compare two MagneticField3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than or equal to right</returns>
        [Pure] public static bool operator >=(MagneticField3D left, MagneticField3D right) => Comparer<MagneticField3D>.Default.Compare(left, right) >= 0;

        // Math
        /// <summary>
        /// Addition operator to add two AngularAcceleration objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new MagneticField3D object with a value of left + right</returns>
        [Pure] public static MagneticField3D operator +(MagneticField3D left, MagneticField3D right)
        {
            var x = left.X + right.X;
            var y = left.Y + right.Y;
            var z = left.Z + right.Z;

            return new MagneticField3D(x, y, z);
        }

        /// <summary>
        /// Subtraction operator to subtract two MagneticField3D objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new MagneticField3D object with a value of left - right</returns>
        [Pure] public static MagneticField3D operator -(MagneticField3D left, MagneticField3D right)
        {
            var x = left.X - right.X;
            var y = left.Y - right.Y;
            var z = left.Z - right.Z;

            return new MagneticField3D(x, y, z);
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
        /// Compare to another MagneticField3D object
        /// </summary>
        /// <param name="obj">The other MagneticField3D cast to object</param>
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