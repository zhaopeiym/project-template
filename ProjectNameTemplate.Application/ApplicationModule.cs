using System.Reflection;
using Talk;

namespace ProjectNameTemplate.Application
{
    public class ApplicationModule : AppModule
    {
        public override void Initialize()
        {
            ModuleAssembly = Assembly.GetExecutingAssembly();
        }
    }
}
