namespace Eff.Examples.Config
{
    public static class Effect
    {
        public static ConfigEffect Config(string key) => new ConfigEffect(key);
    }
}
