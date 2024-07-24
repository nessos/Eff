using System;

namespace Nessos.Effects
{
    /// <summary>
    ///   A type inhabited by a single value.
    /// </summary>
    /// <remarks>
    ///   While similar to <see cref="Void"/>, the unit type is inhabited 
    ///   by a single materialized value and can be used as a generic parameter.
    /// </remarks>
    public readonly record struct Unit
    {
        /// <summary>
        ///   Gets the Unit instance.
        /// </summary>
        public static Unit Value => new();
        /// Implements unit hashcode
        public override int GetHashCode() => 1;
        /// Implements string representation of unit
        public override string ToString() => "()";
    }
}
