using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectNameTemplate.Tests
{
    public abstract class TestBase
    {

        public IContainer container;
        public TestBase()
        {
            container = InitContainerBuilder(new ContainerBuilder());
        }

        public virtual T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        protected abstract IContainer InitContainerBuilder(ContainerBuilder container);
    }
}
