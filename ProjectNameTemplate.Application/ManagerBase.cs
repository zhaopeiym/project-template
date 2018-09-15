using Talk;

namespace ProjectNameTemplate.Application
{
    public class ManagerBase: ITransientDependency
    {      
        /// <summary>
        /// 日记记录
        /// </summary>
        public Serilog.ILogger Logger { get; set; }

        public ManagerBase()
        {

        }
    }
}
