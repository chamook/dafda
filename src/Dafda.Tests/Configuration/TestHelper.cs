using System.Collections.Generic;

namespace Dafda.Tests.Configuration
{
    public static class TestHelper
    {
        public static KeyValuePair<string, string> KeyValue(string key, string value)
        {
            return KeyValuePair.Create(key, value);
        }
    }
}