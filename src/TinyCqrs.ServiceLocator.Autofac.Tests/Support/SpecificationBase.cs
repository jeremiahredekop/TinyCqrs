using NUnit.Framework;

namespace TinyCQRS.Autofac.Tests.Support
{
    public class SpecificationBase
    {
        [SetUp]
        public void Setup()
        {
            Given();
            When();
        }

        protected virtual void Given()
        {
        }

        protected virtual void When()
        {
        }
    }
}