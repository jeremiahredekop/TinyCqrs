using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Practices.ServiceLocation;

namespace TinyCqrs.ServiceLocator.Autofac
{
    public class AutofacServiceLocator : ServiceLocatorImplBase
    {
        private readonly IContainer _container;

        public AutofacServiceLocator(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (string.IsNullOrEmpty(key))
                return _container.Resolve(serviceType);

            return _container.ResolveNamed(key, serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            Type type = typeof (IEnumerable<>).MakeGenericType(serviceType);

            return (_container.Resolve(type) as IEnumerable).Cast<object>();
        }
    }
}