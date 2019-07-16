using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using ProjectNameTemplate.Core;
using ProjectNameTemplate.Core.IRepository;
using ProjectNameTemplate.Infrastructure;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System;
using System.Data;
using System.Data.Common;

namespace ProjectNameTemplate.Repository
{
    /// <summary>
    /// DB上下文
    /// 注意：
    /// 1、DBContext不可跨线程使用
    /// 2、异步对DBContext的使用需要主动释放连接
    /// 3、不要直接对DBContext实例的使用，应该通过对IDBContext的注入使用。（为了保证IDBContext线程内唯一）
    /// 4、采用 MysqlConnector 驱动 https://nuget.org/packages/MySqlConnector https://mysql-net.github.io/MySqlConnector
    /// </summary>
    public class DBContext : IDBContext
    {
        public ITalkSession session;

        public DBContext(ITalkSession userModel)
        {
            this.session = userModel;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string MySqlConn => AppSetting.MySQLSPConnection;


        private IDbConnection _dbConnection;

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection DbConnection
        {
            get
            {
                if (!string.IsNullOrEmpty(MySqlConn))
                {
                    if (_dbConnection == null)
                    {
                        _dbConnection = new MySqlConnection(MySqlConn);
                        // 这里 MiniProfiler.Current 为空，应该是异步切换了线程？
                        if (session.MiniProfiler != null)
                        {
                            _dbConnection = new ProfiledDbConnection((DbConnection)_dbConnection, (MiniProfiler)session.MiniProfiler);
                            //https://stackoverflow.com/questions/50581540/dapper-contrib-and-miniprofiler-for-mysql-integration-issues
                            SqlMapperExtensions.GetDatabaseType = conn => "MySqlConnection";
                        }
                    }
                }
                else
                    throw new Exception("缺少MySQL配置");
                return _dbConnection;
            }
        }

        private IDbTransaction DbTransaction { get; set; }

        /// <summary>
        /// 是否已被提交
        /// </summary>
        public bool Committed { get; private set; } = true;

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTransaction()
        {
            Committed = false;
            bool isClosed = DbConnection.State == ConnectionState.Closed;
            if (isClosed) DbConnection.Open();
            DbTransaction = DbConnection?.BeginTransaction();
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public void CommitTransaction()
        {
            DbTransaction?.Commit();
            Committed = true;
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollBackTransaction()
        {
            DbTransaction?.Rollback();
            Committed = true;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            DbTransaction?.Dispose();
            _dbConnection?.Dispose();
            _dbConnection?.Close();
        }
    }
}
