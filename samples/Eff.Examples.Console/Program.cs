using System.Threading.Tasks;

namespace Nessos.Effects.Examples.Console
{
    partial class Program
    {
        static async Eff Foo()
        {
            await ConsoleEffect.Print("Enter your name: ");
            await ConsoleEffect.Print($"Hello, { await ConsoleEffect.Read()}!\n");
        }

        static async Task Main()
        {
            await Foo().Run(new ConsoleEffectHandler());
        }
    }
}
