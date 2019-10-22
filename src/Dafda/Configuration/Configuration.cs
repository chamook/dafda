using System.Collections;
using System.Collections.Generic;

namespace Dafda.Configuration
{
    public class Configuration : IConfiguration, IReadOnlyDictionary<string, string>
    {
        private readonly IDictionary<string, string> _configuration;

        public Configuration(IDictionary<string, string> configuration)
        {
            _configuration = configuration;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _configuration.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _configuration).GetEnumerator();
        }

        int IReadOnlyCollection<KeyValuePair<string, string>>.Count => _configuration.Count;
        IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => _configuration.Keys;
        IEnumerable<string> IReadOnlyDictionary<string, string>.Values => _configuration.Values;

        string IReadOnlyDictionary<string, string>.this[string key] => _configuration[key];

        bool IReadOnlyDictionary<string, string>.ContainsKey(string key)
        {
            return _configuration.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _configuration.TryGetValue(key, out value);
        }
    }
}