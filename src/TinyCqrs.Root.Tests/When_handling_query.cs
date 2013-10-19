using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fabrication;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using TinyCqrs.Root.Caliburn.Micro;

namespace TinyCqrs.Root.Tests
{
    [TestFixture]
    public class When_handling_query : SpecificationBase
    {
        private IServiceLocator _locator;
        private ApplicationRoot _hi;
        private Task<IEnumerable<MyModel>> _async;


        public class MyQueryArgs : IQueryArgs<IEnumerable<MyModel>>
        {
            public int Count { get; set; }
        }

        public class MyModel
        {
        }

        public class MyQuery : IQuery<MyQueryArgs, IEnumerable<MyModel>>
        {
            public Task<IEnumerable<MyModel>> Perform(MyQueryArgs input)
            {
                return new TaskFactory().StartNew(() => Fabricator.Generate<MyModel>(input.Count));
            }
        }

        protected override void Given()
        {
            _locator = A.Fake<IServiceLocator>();
            A.CallTo(() => _locator.GetInstance<IQuery<MyQueryArgs, IEnumerable<MyModel>>>()).Returns(new MyQuery());


            _hi = new ApplicationRoot(_locator, A.Fake<IEventAggregator>());
        }

        protected override void When()
        {
            _async = _hi.PerformQuery(new MyQueryArgs {Count = 50});
        }

        [Then]
        public void the_command_should_have_been_completed()
        {
            _async.Wait();
            _async.IsCompleted.Should().BeTrue();
        }

        [Then]
        public void the_command_should_have_been_executed()
        {
            _async.Wait();
            _async.Result.Count().Should().Be(50);
        }
    }
}