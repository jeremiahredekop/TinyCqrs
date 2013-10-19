using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using TinyCqrs.Root.Caliburn.Micro;

namespace TinyCqrs.Root.Tests
{
    [TestFixture]
    public class When_handling_command : SpecificationBase
    {
        private IServiceLocator _locator;
        private ApplicationRoot _hi;
        private Task _async;

        public class MyCommandArgs
        {
            public string Text { get; set; }
        }

        public class MyCommand : ICommand<MyCommandArgs>
        {
            public static string StaticVal { get; private set; }


            public Task Invoke(MyCommandArgs command)
            {
                return new TaskFactory().StartNew(() => { StaticVal = command.Text; });
            }
        }

        protected override void Given()
        {
            _locator = A.Fake<IServiceLocator>();
            A.CallTo(() => _locator.GetInstance<ICommand<MyCommandArgs>>()).Returns(new MyCommand());
            _hi = new ApplicationRoot(_locator, A.Fake<IEventAggregator>());
        }

        protected override void When()
        {
            _async = _hi.PerformCommand(new MyCommandArgs {Text = "Yo"});
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
            MyCommand.StaticVal.Should().Be("Yo");
        }
    }
}