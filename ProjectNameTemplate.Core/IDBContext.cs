using System.Data;
using Talk;

namespace ProjectNameTemplate.Core.IRepository
{
    /// <summary>
    /// DB上下文接口
    /// </summary>
    public interface IDBContext : IRelease, IScopedDependency
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        IDbConnection DbConnection { get; }

        /// <summary>
        /// 是否已被提交
        /// </summary>
        bool Committed { get; }
    }
}
