using Autofac;
using FluentAssertions;
using TinyCQRS.Autofac.Tests.Support;
using TinyCqrs;
using TinyCqrs.ServiceLocator.Autofac;

namespace TinyCQRS.Autofac.Tests
{
    public class When_resolving_query : SpecificationBase
    {
        private IQuery<Query1Args, string> _command;
        private IContainer _container;

        protected override void Given()
        {
            var cb = new ContainerBuilder();

            var fac = new AutofacRegistrator(cb);

            fac.RegisterForAssembly(GetType().Assembly);
            _container = cb.Build();
        }

        protected override void When()
        {
            _command = _container.Resolve<IQuery<Query1Args, string>>();
        }

        [Then]
        public void the_command_should_be_resolved()
        {
            _command.Should().NotBeNull();
        }
    }
}