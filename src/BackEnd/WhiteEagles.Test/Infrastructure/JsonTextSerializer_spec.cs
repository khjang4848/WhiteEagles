namespace WhiteEagles.Test.Infrastructure
{
    using AutoFixture;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WhiteEagles.Infrastructure.Serialization;

    [TestClass]
    public class JsonTextSerializer_spec
    {
        private IFixture _fixture;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
            => _fixture = new Fixture();


        [TestMethod]
        public void sut_implement_ITextSerializer()
        {
            var sut = new JsonTextSerializer();
            sut.Should().BeAssignableTo<ITextSerializer>();
        }


        [TestMethod]
        public void constructor_has_guard_clause()
        {
            var builder = new Fixture { OmitAutoProperties = true };
            var assertion = new GuardClauseAssertion(builder);
            assertion.Verify(typeof(JsonTextSerializer).GetConstructors());
        }

        [TestMethod]
        public void Deserialize_has_guard_clause()
        {
            var assertion = new GuardClauseAssertion(_fixture);
            var type = typeof(JsonTextSerializer);
            var method = type.GetMethod(nameof(JsonTextSerializer.Deserialize));
            assertion.Verify(method);
        }

        [TestMethod]
        public void Deserialize_restores_mutable_message_correctly()
        {
            var sut = new JsonTextSerializer();
            var message = _fixture.Create<MutableMessage>();
            var serialized = sut.Serialize(message);
            TestContext.WriteLine(serialized);

            var actual = sut.Deserialize<MutableMessage>(serialized);

            actual.Should().BeOfType<MutableMessage>();
            actual.Should().BeEquivalentTo(message);
        }

        [TestMethod]
        public void Deserialize_restores_immutable_message_correctly()
        {
            var sut = new JsonTextSerializer();
            var message = _fixture.Create<ImmutableMessage>();
            var serialized = sut.Serialize(message);
            TestContext.WriteLine(serialized);


            var actual = sut.Deserialize<ImmutableMessage>(serialized);

            actual.Should().BeOfType<ImmutableMessage>();
            actual.Should().BeEquivalentTo(message);
        }



        internal class MutableMessage
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        internal class ImmutableMessage
        {
            public ImmutableMessage(int id, string name)
                => (Id, Name) = (id, name);

            public int Id { get; }
            public string Name { get; }
        }
    }
}
