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
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct AngularAcceleration3d : IUnitType, IFormattable, IComparable, IEquatable<(double ValueX, double ValueY, double ValueZ)>, IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `AngularAcceleration3d` object.
        /// </summary>
        /// <param name="valueX">The X AngularAcceleration3d value.</param>
        /// <param name="valueY">The Y AngularAcceleration3d value.</param>
        /// <param name="valueZ">The Z AngularAcceleration3d value.</param>
        /// <param name="type"></param>
        public AngularAcceleration3d(double valueX, double valueY, double valueZ,
            AngularAcceleration.UnitType type = AngularAcceleration.UnitType.RadiansPerSecondSquared)
        {
            //always store reference value
            Unit = type;
            AccelerationX = new AngularAcceleration(valueX, Unit);
            AccelerationY = new AngularAcceleration(valueY, Unit);
            AccelerationZ = new AngularAcceleration(valueZ, Unit);
        }

        public AngularAcceleration3d(AngularAcceleration accelerationX, AngularAcceleration accelerationY, AngularAcceleration accelerationZ)
        {
            AccelerationX = new AngularAcceleration(accelerationX.Value, accelerationX.Unit);
            AccelerationY = new AngularAcceleration(accelerationY.Value, accelerationY.Unit);
            AccelerationZ = new AngularAcceleration(accelerationZ.Value, accelerationZ.Unit);

            Unit = AccelerationX.Unit;
        }

        public AngularAcceleration AccelerationX { get; set; }
        public AngularAcceleration AccelerationY { get; set; }
        public AngularAcceleration AccelerationZ { get; set; }

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public AngularAcceleration.UnitType Unit { get; set; }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((AngularAcceleration3d)obj);
        }

        [Pure]
        public bool Equals(AngularAcceleration3d other) =>
            AccelerationX == other.AccelerationX &&
            AccelerationY == other.AccelerationY &&
            AccelerationZ == other.AccelerationZ;


        [Pure] public override int GetHashCode() => (AccelerationX.GetHashCode() + AccelerationY.GetHashCode() + AccelerationZ.GetHashCode()) / 3;

        [Pure] public static bool operator ==(AngularAcceleration3d left, AngularAcceleration3d right) => Equals(left, right);
        [Pure] public static bool operator !=(AngularAcceleration3d left, AngularAcceleration3d right) => !Equals(left, right);
        //ToDo [Pure] public int CompareTo(AngularAcceleration3d other) => Equals(this, other) ? 0 : AccelerationX.CompareTo(other.AccelerationX);
        [Pure] public static bool operator <(AngularAcceleration3d left, AngularAcceleration3d right) => Comparer<AngularAcceleration3d>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(AngularAcceleration3d left, AngularAcceleration3d right) => Comparer<AngularAcceleration3d>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(AngularAcceleration3d left, AngularAcceleration3d right) => Comparer<AngularAcceleration3d>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(AngularAcceleration3d left, AngularAcceleration3d right) => Comparer<AngularAcceleration3d>.Default.Compare(left, right) >= 0;

        [Pure]
        public static AngularAcceleration3d operator +(AngularAcceleration3d lvalue, AngularAcceleration3d rvalue)
        {
            var x = lvalue.AccelerationX + rvalue.AccelerationX;
            var y = lvalue.AccelerationY + rvalue.AccelerationY;
            var z = lvalue.AccelerationZ + rvalue.AccelerationZ;

            return new AngularAcceleration3d(x, y, z);
        }

        [Pure]
        public static AngularAcceleration3d operator -(AngularAcceleration3d lvalue, AngularAcceleration3d rvalue)
        {
            var x = lvalue.AccelerationX - rvalue.AccelerationX;
            var y = lvalue.AccelerationY - rvalue.AccelerationY;
            var z = lvalue.AccelerationZ - rvalue.AccelerationZ;

            return new AngularAcceleration3d(x, y, z);
        }

        [Pure] public override string ToString() => $"{AccelerationX}, {AccelerationY}, {AccelerationZ}";
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => $"{AccelerationX.ToString(format, formatProvider)}, {AccelerationY.ToString(format, formatProvider)}, {AccelerationZ.ToString(format, formatProvider)}";

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals((double ValueX, double ValueY, double ValueZ) other)
        {
            return AccelerationX.Equals(other.ValueX) &&
                AccelerationY.Equals(other.ValueY) &&
                AccelerationZ.Equals(other.ValueZ);
        }

        public int CompareTo((double, double, double) other)
        {
            throw new NotImplementedException();
        }
    }
}