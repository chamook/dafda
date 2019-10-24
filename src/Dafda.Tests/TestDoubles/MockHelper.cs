using Moq;

namespace Dafda.Tests.TestDoubles
{
    public static class MockHelper
    {
        public static T Dummy<T>() where T : class
        {
            return new Mock<T>().Object;
        }
    }
}