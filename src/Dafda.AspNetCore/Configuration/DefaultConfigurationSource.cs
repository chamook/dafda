namespace Dafda.Configuration
{
    internal class DefaultConfigurationSource : ConfigurationSource
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public DefaultConfigurationSource(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override string GetByKey(string keyName)
        {
            return _configuration[keyName];
        }
    }
}