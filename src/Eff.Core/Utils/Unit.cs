namespace Nessos.Effects
{
    /// <summary>
    /// A type inhabited by a single value.
    /// </summary>
    public sealed class Unit
    {
        public static Unit Value => new Unit(); 
        private Unit() { }
        public override int GetHashCode() => 1;
        public override bool Equals(object o) => o is Unit;
        public static bool operator ==(Unit x, Unit y) => true;
        public static bool operator !=(Unit x, Unit y) => false;
    }
}
