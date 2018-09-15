using System;
using Talk;

namespace ProjectNameTemplate.Core.IRepository
{
    public interface ICurrentUnitOfWork : ITransientDependency
    {
        /// <summary>
        /// 保存/暂存【作用于事务/工作单元】
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// 执行一个事务
        /// </summary>
        /// <param name="action"></param>
        void Execute(Action action);
    }
}
