using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ProjectNameTemplate.CodeGenerator;
using ProjectNameTemplate.CodeGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectNameProjectNameTemplate.CodeGenerator
{

    class Program
    {
        /// <summary>
        /// 代码生成器
        /// 1、设置ProjectNameTemplate.CodeGenerator为启动项
        /// 2、config.json中的SqlConnection为mysql数据了连接字符串
        /// 3、运行此控制台，输入你想要生成代码的表名
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                CodeGeneratorManager codeGenerator = new CodeGeneratorManager();
                Console.Write("请输入要生成的实体名：");

                var className = Console.ReadLine();
                Console.WriteLine("准备生成代码，请稍等...");
                //获取字段信息
                var columnNames = ValidationAndGetColumnInfos(ref className);
                if (columnNames == null) return;

                //生成PO、Dto、BO
                codeGenerator.GeneratorEntityAsync(className, columnNames).Wait();
                //生成仓储接口
                codeGenerator.GeneratorIRepositoryAsync(className, columnNames).Wait();
                //生成仓储
                codeGenerator.GeneratorRepositoryAsync(className, columnNames).Wait();
                //生成Manager
                codeGenerator.GeneratorManagerAsync(className, columnNames).Wait();
                //生成控制器
                codeGenerator.GeneratorControllerAsync(className, columnNames).Wait();
                Console.WriteLine("代码全部生成成功！");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.ReadKey();
            }
        }

        #region 数据库验证并获取字段信息
        /// <summary>
        /// 数据库验证并获取字段信息
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private static List<ColumnInfo> ValidationAndGetColumnInfos(ref string className)
        {
            var filePath = Directory.GetCurrentDirectory() + "/config.json";
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(filePath));
            if (string.IsNullOrWhiteSpace(config.SqlConnection))
            {
                Console.Write($"请在{filePath}配置有效的数据库连接字符串。");
                Console.ReadKey();
                return null;
            }
            var sqlConnection = new MySqlConnection(config.SqlConnection);
            try
            {
                if (config.SqlType == "mysql")
                {
                    sqlConnection.Open();
                    sqlConnection.Close();
                }
                else
                    throw new Exception("请输入有效的数据库类型。");
            }
            catch (Exception ex)
            {
                Console.Write($"请在{filePath}配置有效的数据库连接字符串。{ex.Message}");
                Console.ReadKey();
                return null;
            }

            setClassName:
            var sql_table_name = @"SELECT
	                        COUNT(1)
                        FROM
	                        information_schema.`TABLES`
                        WHERE
	                        TABLE_SCHEMA = @tableSchema
                        AND TABLE_NAME = @tableName";
            var count = sqlConnection.QueryFirst<int>(sql_table_name, new { tableSchema = sqlConnection.Database, tableName = className });
            if (count <= 0)
            {
                Console.Write($"请输入数据库中存在的实体名：");
                className = Console.ReadLine();
                goto setClassName;
            }

            var sql_column_name = @"SELECT
	                                        COLUMN_NAME `Name`,
	                                        DATA_TYPE Type,
	                                        IS_NULLABLE IsNull,
	                                        COLUMN_TYPE Length,
	                                        COLUMN_COMMENT Annotation
                                        FROM
	                                        information_schema.`COLUMNS`
                                        WHERE
	                                          TABLE_SCHEMA = @tableSchema
                                        AND TABLE_NAME = @tableName";
            var columnNames = sqlConnection.Query<ColumnInfo>(sql_column_name, new { tableSchema = sqlConnection.Database, tableName = className }).ToList();
            return columnNames;
        }
        #endregion
    }
}
