using System.Reflection;
using Talk;

namespace ProjectNameTemplate.Repository
{
    public class RepositoryModule : AppModule
    {
        public override void Initialize()
        {
            ModuleAssembly = Assembly.GetExecutingAssembly();
        }
    }
}
