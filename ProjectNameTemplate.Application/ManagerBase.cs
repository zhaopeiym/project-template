using ProjectNameTemplate.Core;
using Talk;

namespace ProjectNameTemplate.Application
{
    public class ManagerBase: ITransientDependency
    {      
        /// <summary>
        /// 日记记录
        /// </summary>
        public ITalkLogger Logger { get; set; }

        public ManagerBase()
        {

        }
    }
}
