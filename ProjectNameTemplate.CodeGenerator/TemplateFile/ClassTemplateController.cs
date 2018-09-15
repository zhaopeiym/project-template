////using Microsoft.AspNetCore.Mvc;
////using ProjectNameTemplate.Application;
////using ProjectNameTemplate.Entity;
////using System.Collections.Generic;
////using System.Threading.Tasks;


namespace ProjectNameTemplate.CodeGenerator.TemplateFile
{
    public class ClassTemplateController////: BaseController
    {
        private ClassTemplateManager  classTemplateManager;
        public ClassTemplateController(ClassTemplateManager classTemplateManager)
        {
            this.classTemplateManager = classTemplateManager;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ////[HttpPost]
        /*public async Task<int> Add(AddObject entity)
        {
            return await classTemplateManager.AddAsync(entity);
        }*/

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ////[HttpPost]
        ////public async Task<int> Remove(int id)
        ////{
        ////    return await classTemplateManager.RemoveAsync(id);
        ////}

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ////[HttpPost]
        /*public async Task<int> Modify(ModifyObject entity)
        {
            return await classTemplateManager.ModifyAsync(entity);
        }*/

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        ////[HttpPost]
        /*public async Task<List<QueryObject>> Query()
        {            
             return await classTemplateManager.QueryAsync();
        }*/
    }
}
