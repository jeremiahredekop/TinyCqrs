using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using TinyCqrs.Root.Caliburn.Micro;

namespace TinyCqrs.Root.Tests
{
    [TestFixture]
    public class When_events_occur_from_commands : SpecificationBase
    {
        private IServiceLocator _locator;
        private ApplicationRoot _hi;
        private Task _async;
        private EventAggregator _eventAggregator;
        private Handler _handler;
        private const string ExpectedName = "George";

        public class NameChanged
        {
            public string Name { get; set; }
        }

        public class Handler : IHandle<NameChanged>
        {
            public static string NewName { get; private set; }

            public void Handle(NameChanged message)
            {
                NewName = message.Name;
            }
        }

        public class RenameSomebodyArgs
        {
            public string Text { get; set; }
        }

        public class RenameSomebody : ICommand<RenameSomebodyArgs>
        {
            private readonly IPublisher _publisher;

            public RenameSomebody(IPublisher publisher)
            {
                _publisher = publisher;
            }

            public Task Invoke(RenameSomebodyArgs command)
            {
                return new TaskFactory().StartNew(() => _publisher.Publish(new NameChanged {Name = command.Text}));
            }
        }

        protected override void Given()
        {
            _locator = A.Fake<IServiceLocator>();
            _eventAggregator = new EventAggregator();

            _hi = new ApplicationRoot(_locator, _eventAggregator);

            _handler = new Handler();
            _hi.Subscribe(_handler);


            A.CallTo(() => _locator.GetInstance<ICommand<RenameSomebodyArgs>>()).Returns(new RenameSomebody(_hi));
        }

        protected override void When()
        {
            _async = _hi.PerformCommand(new RenameSomebodyArgs {Text = ExpectedName});
        }

        [Then]
        public void the_command_should_have_been_completed()
        {
            _async.IsCompleted.Should().BeTrue();
        }

        [Then]
        public void the_command_should_have_been_executed()
        {
            Handler.NewName.Should().Be(ExpectedName);
        }
    }
}