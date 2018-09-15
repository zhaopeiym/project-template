using System.Collections.Generic;
using System.Threading.Tasks;
////using ProjectNameTemplate.Core.IRepository;
////using ProjectNameTemplate.Entity;
////using ProjectNameTemplate.Common.Extensions;
////using System.Linq;
////using Talk.AutoMap.Extensions;


namespace ProjectNameTemplate.CodeGenerator.TemplateFile
{
    public class ClassTemplateManager////: ManagerBase
    {
        private IClassTemplateRepository classTemplateRepository;
        public ClassTemplateManager(IClassTemplateRepository classTemplateRepository)
        {
            this.classTemplateRepository = classTemplateRepository;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /*public async Task<int> AddAsync(AddObject entity)
        {
           var po = entity.MapTo<object>();
           return await classTemplateRepository.InsertAsync(po);
        }*/

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(object entity)
        {
            return await classTemplateRepository.DeleteAsync(entity);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> RemoveAsync(int id)
        {
            return await classTemplateRepository.DeleteAsync(id);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /*public async Task<int> ModifyAsync(ModifyObject entity)
        {
            var po = entity.MapTo<ModifyBO>();
            return await classTemplateRepository.UpdateAsync(po);
        }*/

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        /*public async Task<List<QueryObject>> QueryAsync()
        {            
            var entitys = await classTemplateRepository.GetAsync();
            return entitys.Select(t => t.MapTo<QueryObject>()).ToList();
        }*/
    }
}
