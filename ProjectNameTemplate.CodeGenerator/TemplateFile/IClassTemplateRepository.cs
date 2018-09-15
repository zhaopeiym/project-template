using System.Collections.Generic;
using System.Threading.Tasks;
////using ProjectNameTemplate.Entity;
////using Talk;

namespace ProjectNameTemplate.CodeGenerator.TemplateFile
{
    public interface IClassTemplateRepository ////: ITransientDependency
    {
        /// <summary>
        /// 新增
        /// </summary>
        ////Task<int> InsertAsync(object entity);

        /// <summary>
        /// 删除
        /// </summary>
        Task<bool> DeleteAsync(object entity);

        /// <summary>
        /// 删除
        /// </summary>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// 修改
        /// </summary>
        ////Task<int> UpdateAsync(ModifyObject entity);

        /// <summary>
        /// 查询
        /// </summary>
        ////Task<List<QueryObject>> GetAsync();
    }
}
