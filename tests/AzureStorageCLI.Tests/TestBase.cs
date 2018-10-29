using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Moq;

namespace AzureStorageCLI.Tests
{
    public class TestBase
    {
        private readonly IFixture _fixture;
        public TestBase()
        {
            _fixture = new Fixture();
        }

        public IFixture FixtureRepository => _fixture;

        public T Create<T>()
        {
            return FixtureRepository.Create<T>();
        }

        public List<T> CreateMany<T>()
        {
            return FixtureRepository.CreateMany<T>().ToList();
        }

        public IMock<T> MockFor<T>() where T:class 
        {
            return new Mock<T>();
        }
    }
}