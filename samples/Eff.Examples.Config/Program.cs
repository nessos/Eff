using Nessos.Effects;
using Nessos.Effects.Examples.Config;

var handler = new ConfigurationManagerEffectHandler();
var result = await Test().Run(handler);
Console.WriteLine(result);

static async Eff<string> Test()
{
    var value1 = await ConfigEffect.Get("Setting1");
    var value2 = await ConfigEffect.Get("Setting2");

    return $"{value1} - {value2}";
}
