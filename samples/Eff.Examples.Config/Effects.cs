namespace Nessos.Effects.Examples.Config
{
    public class ConfigEffect : Effect<string>
    {
        public ConfigEffect(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }

    public static class Effects
    {
        public static ConfigEffect Config(string key) => new ConfigEffect(key);
    }
}
