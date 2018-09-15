using ProjectNameTemplate.Core.IRepository;
using System;

namespace ProjectNameTemplate.Repository
{
    /// <summary>
    /// 当前工作单元
    /// </summary>
    public class CurrentUnitOfWork : ICurrentUnitOfWork
    {
        private DBContext dBContext;
        public CurrentUnitOfWork(IDBContext dBContext)
        {
            this.dBContext = dBContext as DBContext;
        }

        /// <summary>
        /// 保存/暂存【作用于事务/工作单元】
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                if (!dBContext.Committed)//没有被提交，说明已经开启了一个事务。所以提交后应马上开启一个新的事务。
                {
                    dBContext.CommitTransaction();
                    dBContext.BeginTransaction();
                }
            }
            catch (System.Exception)
            {
                if (!dBContext.Committed)
                    dBContext.RollBackTransaction();
                throw;
            }
        }

        /// <summary>
        /// 执行一个事务
        /// </summary>
        /// <param name="action"></param>
        public void Execute(Action action)
        {
            if (dBContext.Committed)
            {
                //开启事务
                dBContext.BeginTransaction();
                try
                {
                    action();
                    //提交事务
                    dBContext.CommitTransaction();
                }
                catch (Exception)
                {
                    dBContext.RollBackTransaction();
                    throw;
                }
            }
            else
            {
                action();
            }
        }
    }
}
