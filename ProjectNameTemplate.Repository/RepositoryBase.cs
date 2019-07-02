
using ProjectNameTemplate.Core;
using ProjectNameTemplate.Core.IRepository;
using System.Data;

namespace ProjectNameTemplate.Repository
{
    public class RepositoryBase
    {
        /// <summary>
        /// DB上下文
        /// </summary>
        private IDBContext dBContext;

        /// <summary>
        /// 日记记录
        /// </summary>
        public ITalkLogger Logger { get; set; }

        public RepositoryBase(IDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        /// <summary>
        /// 数据库连接
        /// 注意：
        /// 1、不可通过DbConnection属性对BeginTransaction事务的使用。（与ICurrentUnitOfWork和UnitOfWork特性的使用有冲突）
        /// 2、如果需要使用事务请使用UnitOfWork特性或者ICurrentUnitOfWork提供的Execute方法。
        /// </summary>
        protected IDbConnection DbConnection => dBContext.DbConnection;
    }
}
