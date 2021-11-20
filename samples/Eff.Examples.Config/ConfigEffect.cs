namespace Nessos.Effects.Examples.Config
{
    /// <summary>
    ///   Defines an abstract effect for looking up configuration by key
    /// </summary>
    public class ConfigEffect : Effect<string?>
    {
        public ConfigEffect(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public static ConfigEffect Get(string key) => new ConfigEffect(key);
    }
}
