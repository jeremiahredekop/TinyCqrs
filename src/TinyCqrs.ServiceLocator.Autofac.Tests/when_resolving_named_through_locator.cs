using Autofac;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using TinyCQRS.Autofac.Tests.Support;
using TinyCqrs;
using TinyCqrs.ServiceLocator.Autofac;

namespace TinyCQRS.Autofac.Tests
{
    public class when_resolving_named_through_locator : SpecificationBase
    {
        private object _comand;
        private IServiceLocator _sl;


        protected override void Given()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<Command1>().Named<ICommand<Command1Args>>("Bacon").AsImplementedInterfaces();
            IContainer container = cb.Build();
            _sl = new AutofacServiceLocator(container);
        }

        protected override void When()
        {
            _comand = _sl.GetInstance(typeof (ICommand<Command1Args>), "Bacon");
        }

        [Then]
        public void command_should_have_been_resolved()
        {
            _comand.Should().NotBeNull();
        }
    }
}