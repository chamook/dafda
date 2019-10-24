using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dafda.Configuration;
using Dafda.Consuming;
using Dafda.Messaging;
using Dafda.Tests.TestDoubles;
using Xunit;

namespace Dafda.Tests.Consuming
{
    public class TestConsumer
    {
        [Fact]
        public async Task invokes_expected_handler_when_consuming()
        {
            var message = A.MessageResult
                .WithTransportLevelMessage(new TransportLevelMessageStub(new FooMessage(), "bar"))
                .Build();

            var spy = new FooMessageHandler();

            var messageHandlerRegistry = A.MessageHandlerRegistry
                .Register<FooMessage, FooMessageHandler>("foo", "bar")
                .Build();

            var sut = Consumer.Create()
                .WithConfiguration(new ConsumerConfigurationBuilder()
                    .WithGroupId("foo.group.id")
                    .WithBootstrapServers("foo.bootstrap.servers")
                )
                .WithTopicSubscriberScopeFactory(A.TopicSubscriberScopeFactory.WithMessageResult(message))
                .WithUnitOfWorkFactory(new DefaultUnitOfWorkFactory(type => new UnitOfWorkStub(spy)))
                .WithMessageHandlerRegistry(messageHandlerRegistry)
                .Build();

            await sut.ConsumeSingle(CancellationToken.None);

            Assert.True(spy.WasCalled);
        }

        [Fact]
        public async Task throws_expected_exception_when_consuming_a_message_without_a_handler_as_been_registered_for_it()
        {
            var sut = Consumer.Create()
                .WithConfiguration(new ConsumerConfigurationBuilder()
                    .WithGroupId("foo.group.id")
                    .WithBootstrapServers("foo.bootstrap.servers")
                )
                .WithTopicSubscriberScopeFactory(A.TopicSubscriberScopeFactory.WithMessageResult(A.MessageResult))
                .WithUnitOfWorkFactory(new DefaultUnitOfWorkFactory(type => new UnitOfWorkStub(new object())))
                .Build();

            await Assert.ThrowsAsync<MissingMessageHandlerRegistrationException>(() => sut.ConsumeSingle(CancellationToken.None));
        }

        [Fact]
        public async Task expected_order_of_handler_invocation_in_unit_of_work()
        {
            var orderOfInvocation = new LinkedList<string>();

            var messageHandlerRegistry = A.MessageHandlerRegistry
                .Register<FooMessage, FooMessageHandler>("bar", "foo")
                .Build();

            var sut = Consumer.Create()
                .WithConfiguration(new ConsumerConfigurationBuilder()
                    .WithGroupId("foo.group.id")
                    .WithBootstrapServers("foo.bootstrap.servers")
                )
                .WithUnitOfWorkFactory(new DefaultUnitOfWorkFactory(type => new UnitOfWorkSpy(
                    handlerInstance: new MessageHandlerSpy<FooMessage>(() => orderOfInvocation.AddLast("during")),
                    pre: () => orderOfInvocation.AddLast("before"),
                    post: () => orderOfInvocation.AddLast("after")
                )))
                .WithTopicSubscriberScopeFactory(A.TopicSubscriberScopeFactory.WithMessageResult(A.MessageResult))
                .WithMessageHandlerRegistry(messageHandlerRegistry)
                .Build();

            await sut.ConsumeSingle(CancellationToken.None);

            Assert.Equal(new[] {"before", "during", "after"}, orderOfInvocation);
        }

        [Fact]
        public async Task will_not_call_commit_when_auto_commit_is_enabled()
        {
            var handlerStub = Dummy.Of<IMessageHandler<FooMessage>>();

            var wasCalled = false;

            var resultSpy = A.MessageResult
                .WithOnCommit(() =>
                {
                    wasCalled = true;
                    return Task.CompletedTask;
                })
                .Build();

            var messageHandlerRegistry = A.MessageHandlerRegistry
                .Register<FooMessage, FooMessageHandler>("bar", "foo")
                .Build();

            var sut = Consumer.Create()
                .WithConfiguration(new ConsumerConfigurationBuilder()
                    .WithGroupId("foo.group.id")
                    .WithBootstrapServers("foo.bootstrap.servers")
                    .WithEnableAutoCommit(true)
                )
                .WithTopicSubscriberScopeFactory(A.TopicSubscriberScopeFactory.WithMessageResult(resultSpy))
                .WithUnitOfWorkFactory(new DefaultUnitOfWorkFactory(x => new UnitOfWorkStub(handlerStub)))
                .WithMessageHandlerRegistry(messageHandlerRegistry)
                .Build();

            await sut.ConsumeSingle(CancellationToken.None);

            Assert.False(wasCalled);
        }

        [Fact]
        public async Task will_call_commit_when_auto_commit_is_disabled()
        {
            var handlerStub = Dummy.Of<IMessageHandler<FooMessage>>();

            var wasCalled = false;

            var resultSpy = A.MessageResult
                .WithOnCommit(() =>
                {
                    wasCalled = true;
                    return Task.CompletedTask;
                })
                .Build();

            var messageHandlerRegistry = A.MessageHandlerRegistry
                .Register<FooMessage, FooMessageHandler>("bar", "foo")
                .Build();

            var sut = Consumer.Create()
                .WithConfiguration(new ConsumerConfigurationBuilder()
                    .WithGroupId("foo.group.id")
                    .WithBootstrapServers("foo.bootstrap.servers")
                    .WithEnableAutoCommit(false)
                )
                .WithTopicSubscriberScopeFactory(A.TopicSubscriberScopeFactory.WithMessageResult(resultSpy))
                .WithUnitOfWorkFactory(new DefaultUnitOfWorkFactory(x => new UnitOfWorkStub(handlerStub)))
                .WithMessageHandlerRegistry(messageHandlerRegistry)
                .Build();

            await sut.ConsumeSingle(CancellationToken.None);

            Assert.True(wasCalled);
        }

        #region helper classes

        public class FooMessage
        {
            public string Value { get; set; }
        }

        public class FooMessageHandler : IMessageHandler<FooMessage>
        {
            public Task Handle(FooMessage message)
            {
                WasCalled = true;
                return Task.CompletedTask;
            }

            public bool WasCalled { get; private set; }
        }

        #endregion
    }
}