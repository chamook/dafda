using System.Collections.Generic;
using System.Linq;
using Dafda.Logging;

namespace Dafda.Configuration
{
    public abstract class ConfigurationBuilderBase<T> where T : ConfigurationBuilderBase<T>
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly IDictionary<string, string> _configuration = new Dictionary<string, string>();
        private readonly IList<NamingConvention> _namingConventions = new List<NamingConvention>();

        private ConfigurationSource _configurationSource = ConfigurationSource.Null;

        public T WithConfiguration(string key, string value)
        {
            _configuration[key] = value;
            return (T) this;
        }

        public T WithBootstrapServers(string bootstrapServers)
        {
            return WithConfiguration(ConfigurationKey.BootstrapServers, bootstrapServers);
        }

        public T WithConfigurationSource(ConfigurationSource configurationSource)
        {
            _configurationSource = configurationSource;
            return (T) this;
        }

        public T WithNamingConvention(NamingConvention namingConvention)
        {
            _namingConventions.Add(namingConvention);
            return (T) this;
        }

        public T WithEnvironmentStyle(string prefix = null, params string[] additionalPrefixes)
        {
            WithNamingConvention(NamingConvention.UseEnvironmentStyle(prefix));

            foreach (var additionalPrefix in additionalPrefixes)
            {
                WithNamingConvention(NamingConvention.UseEnvironmentStyle(additionalPrefix));
            }

            return (T) this;
        }

        public IConfiguration Build()
        {
            BuildConfiguration();

            ValidateConfiguration();

            return new Configuration(_configuration);
        }

        private void BuildConfiguration()
        {
            if (!_namingConventions.Any())
            {
                _namingConventions.Add(item: NamingConvention.Default);
            }

            FillConfiguration();
        }

        private void FillConfiguration()
        {
            foreach (var key in ConfigurationKeys)
            {
                if (_configuration.ContainsKey(key))
                {
                    continue;
                }

                var value = GetByKey(key);
                if (value != null)
                {
                    _configuration[key] = value;
                }
            }
        }

        public abstract IEnumerable<string> ConfigurationKeys { get; }

        private string GetByKey(string key)
        {
            Logger.Debug("Looking for {Key} in {SourceName} using keys {AttemptedKeys}", key, GetSourceName(), GetAttemptedKeys(key));

            return _namingConventions
                .Select(namingConvention => namingConvention.GetKey(key))
                .Select(actualKey => _configurationSource.GetByKey(actualKey))
                .FirstOrDefault(value => value != null);
        }

        private void ValidateConfiguration()
        {
            foreach (var key in RequiredConfigurationKeys)
            {
                if (!_configuration.TryGetValue(key, out var value) || string.IsNullOrEmpty(value))
                {
                    var message = $"Expected key '{key}' not supplied in '{GetSourceName()}' (attempted keys: '{string.Join("', '", GetAttemptedKeys(key))}')";
                    throw new InvalidConfigurationException(message);
                }
            }
        }

        public abstract IEnumerable<string> RequiredConfigurationKeys { get; }

        private string GetSourceName()
        {
            return _configurationSource.GetType().Name;
        }

        private IEnumerable<string> GetAttemptedKeys(string key)
        {
            return _namingConventions.Select(convention => convention.GetKey(key));
        }
    }
}