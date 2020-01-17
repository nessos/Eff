namespace Nessos.Eff.Examples.CancellationToken
{
    public class CancellationTokenEffect : Effect<System.Threading.CancellationToken>
    {

    }

    public static class Effects
    {
        public static CancellationTokenEffect CancellationToken() => new CancellationTokenEffect();
    }
}
