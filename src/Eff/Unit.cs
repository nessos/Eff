using System;
using System.Threading.Tasks;

namespace Nessos.Effects
{
    /// <summary>
    ///   A type inhabited by a single value.
    /// </summary>
    /// <remarks>
    ///   While similar to <see cref="void"/>, the unit type is inhabited 
    ///   by a single materialized value and can be used as a generic parameter.
    /// </remarks>
    public readonly struct Unit : IEquatable<Unit>
    {
        /// <summary>
        ///   Gets the Unit instance.
        /// </summary>
        public static Unit Value => new Unit();
        /// Implements unit hashcode
        public override int GetHashCode() => 1;
        /// Implements unit equality
        public override bool Equals(object other) => other is Unit;
        /// Implements unit equality
        public bool Equals(Unit other) => true;
        /// Implements unit equality
        public static bool operator ==(Unit x, Unit y) => true;
        /// Implements unit inequality
        public static bool operator !=(Unit x, Unit y) => false;

        /// allow to shortcut effect
        public static implicit operator ValueTask(Unit value) => default;
    }
}
