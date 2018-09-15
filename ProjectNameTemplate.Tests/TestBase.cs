using Autofac;
using Talk;

namespace ProjectNameTemplate.Tests
{
    public abstract class TestBase
    {
        public IContainer container;
        public TestBase()
        {
            var module = ModuleManager.Create<StartModule>();
            var builder = module.ContainerBuilder;                   
            module.Initialize();
            container = module.Container;
        }

        public virtual T Resolve<T>()
        {
            return container.Resolve<T>();
        }
    }
}
