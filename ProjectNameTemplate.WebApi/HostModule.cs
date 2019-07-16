using ProjectNameTemplate.Application;
using ProjectNameTemplate.Core;
using ProjectNameTemplate.Infrastructure;
using ProjectNameTemplate.Repository;
using System.Reflection;
using Talk;

namespace ProjectNameTemplate.WebApi
{
    [DependsOn(typeof(CoreModule),
        typeof(ApplicationModule),
        typeof(RepositoryModule),
        typeof(InfrastructureModule))]
    public class HostModule : AppModule
    {
        public override void Initialize()
        {
            ModuleAssembly = Assembly.GetExecutingAssembly();
        }
    }
}
