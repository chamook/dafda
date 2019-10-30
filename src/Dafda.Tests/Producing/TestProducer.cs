﻿using System.Threading.Tasks;
using Dafda.Producing;
using Dafda.Tests.TestDoubles;
using Xunit;

namespace Dafda.Tests.Producing
{
    public class TestProducer
    {
        [Fact]
        public async Task Can_produce_message()
        {
            var spy = new KafkaProducerSpy();

            var sut = Producer.Create()
                .WithKafkaProducerFactory(new KafkaProducerFactoryStub(spy))
                .WithOutgoingMessageRegistry(A.OutgoingMessageRegistry
                    .Register<Message>("foo", "bar", @event => @event.Id)
                    .Build()
                )
                .Build();

            await sut.Produce(new Message {Id = "dummyId"});

            Assert.Equal("foo", spy.LastMessage.Topic);
            Assert.Equal("dummyId", spy.LastMessage.Key);
        }

        public class Message
        {
            public string Id { get; set; }
        }
    }
}