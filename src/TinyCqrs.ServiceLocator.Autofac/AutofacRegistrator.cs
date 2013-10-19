using System.Linq;
using System.Reflection;
using Autofac;

namespace TinyCqrs.ServiceLocator.Autofac
{
    public class AutofacRegistrator
    {
        private readonly ContainerBuilder _builder;

        public AutofacRegistrator(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void RegisterForAssembly(Assembly assembly)
        {
            _builder.RegisterAssemblyTypes(assembly).Where(type => type.IsAssignableTo<IQuery>() ||
                                                                   type.IsAssignableTo<ICommand>()
                ).AsImplementedInterfaces();
        }
    }
}