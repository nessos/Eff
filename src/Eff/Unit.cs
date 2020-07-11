using System;

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
        public static Unit Value => new Unit();
        public override int GetHashCode() => 1;
        public override bool Equals(object other) => other is Unit;
        public bool Equals(Unit other) => true;
        public static bool operator ==(Unit x, Unit y) => true;
        public static bool operator !=(Unit x, Unit y) => false;
    }
}
