using System.Reflection;
using Talk;

namespace ProjectNameTemplate.Core
{    
    public class CoreModule : AppModule
    {
        public override void Initialize()
        {
            ModuleAssembly = Assembly.GetExecutingAssembly();
        }
    }
}
