namespace Dafda.Producing
{
    public class OutgoingMessage
    {
        public static Builder Create()
        {
            return new Builder();
        }

        private OutgoingMessage(string topic, string messageId, string key, string type, string value)
        {
            Topic = topic;
            MessageId = messageId;
            Type = type;
            Key = key;
            Value = value;
        }

        public string Topic { get; }
        public string MessageId { get; }
        public string Type { get; }
        public string Key { get; }
        public string Value { get; }

        #region Builder

        public class Builder
        {
            private string _topic;
            private string _messageId;
            private string _key;
            private string _type;
            private string _value;

            internal Builder()
            {
            }

            public Builder WithTopic(string topic)
            {
                _topic = topic;
                return this;
            }

            public Builder WithMessageId(string messageId)
            {
                _messageId = messageId;
                return this;
            }

            public Builder WithKey(string key)
            {
                _key = key;
                return this;
            }

            public Builder WithType(string type)
            {
                _type = type;
                return this;
            }

            public Builder WithValue(string value)
            {
                _value = value;
                return this;
            }

            public OutgoingMessage Build()
            {
                return new OutgoingMessage(_topic, _messageId, _key, _type, _value);
            }
        }

        #endregion
    }
}