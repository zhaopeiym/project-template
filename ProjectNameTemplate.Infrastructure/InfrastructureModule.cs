using System.Reflection;
using Talk;

namespace ProjectNameTemplate.Infrastructure
{
    public class InfrastructureModule : AppModule
    {
        public override void Initialize()
        {
            ModuleAssembly = Assembly.GetExecutingAssembly();
        }
    }
}
