﻿using System;
using System.Collections.Generic;
using Dafda.Configuration;
using Dafda.Consuming;
using Dafda.Messaging;
using Dafda.Tests.TestDoubles;

namespace Dafda.Tests.Builders
{
    public class ConsumerBuilder
    {
        private IHandlerUnitOfWorkFactory _unitOfWorkFactory;
        private ITopicSubscriberScopeFactory _topicSubscriberScopeFactory;
        private MessageRegistration[] _messageRegistrations;
        private bool _enableAutoCommit;

        public ConsumerBuilder()
        {
            _unitOfWorkFactory = new HandlerUnitOfWorkFactoryStub(null);

            var messageStub = new MessageResultBuilder().Build();
            _topicSubscriberScopeFactory = new TopicSubscriberScopeFactoryStub(new TopicSubscriberScopeStub(messageStub));

            _messageRegistrations = new MessageRegistration[0];
        }

        public ConsumerBuilder WithUnitOfWorkFactory(Func<Type, IHandlerUnitOfWork> unitOfWorkFactory)
        {
            _unitOfWorkFactory = new DefaultUnitOfWorkFactory(unitOfWorkFactory);
            return this;
        }

        public ConsumerBuilder WithTopicSubscriberScopeFactory(ITopicSubscriberScopeFactory topicSubscriberScopeFactory)
        {
            _topicSubscriberScopeFactory = topicSubscriberScopeFactory;
            return this;
        }

        public ConsumerBuilder WithMessageRegistrations(params MessageRegistration[] messageRegistrations)
        {
            _messageRegistrations = messageRegistrations;
            return this;
        }

        public ConsumerBuilder WithEnableAutoCommit(bool enableAutoCommit)
        {
            _enableAutoCommit = enableAutoCommit;
            return this;
        }

        public Consumer Build()
        {
            var configuration = new ConsumerConfigurationStub
            {
                MessageHandlerRegistry = new MessageHandlerRegistryStub(_messageRegistrations),
                UnitOfWorkFactory = _unitOfWorkFactory,
                TopicSubscriberScopeFactory = _topicSubscriberScopeFactory,
                EnableAutoCommit = _enableAutoCommit
            };

            return new Consumer(configuration);
        }

        #region private helper classes

        private class ConsumerConfigurationStub : IConsumerConfiguration
        {
            public Dafda.Configuration.Configuration Configuration { get; set; }
            public IMessageHandlerRegistry MessageHandlerRegistry { get; set; }
            public IHandlerUnitOfWorkFactory UnitOfWorkFactory { get; set; }
            public ITopicSubscriberScopeFactory TopicSubscriberScopeFactory { get; set; }
            public bool EnableAutoCommit { get; set; }
            public IEnumerable<string> SubscribedTopics { get; set; }
        }

        #endregion
    }
}