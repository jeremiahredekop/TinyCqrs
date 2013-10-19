﻿using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using TinyCQRS.Autofac.Tests.Support;
using TinyCqrs;
using TinyCqrs.ServiceLocator.Autofac;

namespace TinyCQRS.Autofac.Tests
{
    public class when_resolving_many_through_locator : SpecificationBase
    {
        private IEnumerable<ICommand> _comands;
        private IServiceLocator _sl;


        protected override void Given()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<Command1>().As<ICommand>();
            cb.RegisterType<Command2>().As<ICommand>();
            cb.RegisterType<Command3>().As<ICommand>();

            IContainer container = cb.Build();
            _sl = new AutofacServiceLocator(container);
        }

        protected override void When()
        {
            _comands = _sl.GetAllInstances<ICommand>();
        }

        [Then]
        public void three_commands_should_have_been_resolved()
        {
            _comands.Count().Should().Be(3);
        }
    }
}