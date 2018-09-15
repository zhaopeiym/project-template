using ProjectNameTemplate.Application;
using ProjectNameTemplate.Core;
using ProjectNameTemplate.Repository;
using Talk;

namespace ProjectNameTemplate.Tests
{
    [DependsOn(typeof(CoreModule), typeof(ApplicationModule), typeof(RepositoryModule))]
    public class StartModule : AppModule
    {
    }
}
