using System;
using System.Threading.Tasks;
using Dafda.Outbox;
using Dafda.Tests.TestDoubles;
using Xunit;
using static Dafda.Tests.TestDoubles.MockHelper;

namespace Dafda.Tests.Outbox
{
    public class TestOutboxMessageCollector
    {
        [Fact]
        public async Task Fails_for_unregistered_outgoing_messages()
        {
            var sut = OutboxMessageCollector.Create()
                .WithOutboxMessageRepository(Dummy<IOutboxMessageRepository>())
                .WithOutboxMessageRegistry(A.OutgoingMessageRegistry.Build())
                .Build();

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Enqueue(new[] {new Message()}));
        }

        [Fact]
        public async Task Can_delegate_persistence_for_outgoing_message()
        {
            var spy = new OutboxMessageRepositorySpy();

            var sut = OutboxMessageCollector.Create()
                .WithOutboxMessageRegistry(
                    A.OutgoingMessageRegistry
                        .Register<Message>("foo", "bar", @event => "baz")
                        .Build()
                )
                .WithOutboxMessageRepository(spy)
                .Build();

            await sut.Enqueue(new[] {new Message()});

            Assert.NotEmpty(spy.OutboxMessages);
        }

        public class Message
        {
        }
    }
}