using ProjectNameTemplate.CodeGenerator.Helper;
using ProjectNameTemplate.CodeGenerator.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNameTemplate.CodeGenerator
{
    /// <summary>
    /// 代码生成器
    /// </summary>
    public class CodeGeneratorManager
    {
        private string Path => new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
        private string ClassTemplateName => "ClassTemplate";

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnInfos">表字段信息</param>
        /// <returns></returns>
        public async Task GeneratorEntityAsync(string tableName, List<ColumnInfo> columnInfos)
        {
            var entityName = tableName.ToBigHump();

            var strColumnInfo = @"
        /// <summary>
        /// Annotation
        /// </summary>
        public string Name { get; set; }";
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in columnInfos)
            {
                var attributeInfo = strColumnInfo.Replace("Annotation", item.Annotation)
                    .Replace("Name", item.Name)
                    .Replace("string", item.Type.ToCSharpType());
                stringBuilder.Append(attributeInfo + "\r\n");
            }
            var fileContent = await File.ReadAllTextAsync(Path + "/TemplateFile/ClassTemplatePO.cs");
            fileContent = fileContent.Replace(ClassTemplateName, entityName)
                .Replace("ProjectNameTemplate.CodeGenerator.TemplateFile", "ProjectNameTemplate.Entity")
                .Replace("//Attribute", stringBuilder.ToString())
                .ReplaceAnnotation();

            #region PO（数据库实体，和表一一对应的对象） 
            var poContent = fileContent.Replace("//AutoMap", $"")
                .Replace("//using", $"");
            var entityPath = new DirectoryInfo(Path).Parent.FullName + $"/ProjectNameTemplate.Core/Entitys/";
            if (!Directory.Exists(entityPath)) Directory.CreateDirectory(entityPath);
            poContent.WriteAllText($"{entityPath}{entityName}PO.cs");
            #endregion

            #region Dto（传输对象，用于接口对外传输）
            var fileDtoContent = fileContent.Replace("//using", $"using Talk.AutoMap.Extensions;");
            var dtoPath = new DirectoryInfo(Path).Parent.FullName + $"/ProjectNameTemplate.Application/Dtos/";
            if (!Directory.Exists(dtoPath)) Directory.CreateDirectory(dtoPath);
            var addContent = fileDtoContent.Replace($"{entityName}PO", $"Add{entityName}Input")
                .Replace("//AutoMap", $"[AutoMap(typeof({entityName}PO))]");
            var modifyContent = fileDtoContent.Replace($"{entityName}PO", $"Modify{entityName}Input")
                .Replace("//AutoMap", $"[AutoMap(typeof(Modify{entityName}BOInput))]");
            var queryContent = fileDtoContent.Replace($"{entityName}PO", $"Query{entityName}Output")
                .Replace("//AutoMap", $"[AutoMap(typeof(Query{entityName}BOOutput))]");
            addContent.WriteAllText($"{dtoPath}Add{entityName}Input.cs");
            modifyContent.WriteAllText($"{dtoPath}Modify{entityName}Input.cs");
            queryContent.WriteAllText($"{dtoPath}Query{entityName}Output.cs");
            #endregion

            #region BO （业务对象，数据库实体对象的拆分和组合）
            var boPath = new DirectoryInfo(Path).Parent.FullName + $"/ProjectNameTemplate.Core/Entitys/BO/";
            if (!Directory.Exists(boPath)) Directory.CreateDirectory(boPath);            
            var modifyBoContent = fileContent.Replace($"{entityName}PO", $"Modify{entityName}BOInput")
                .Replace("//AutoMap", $"")
                .Replace("//using", $"")
                .ReplaceAnnotation();
            var queryBoContent = fileContent.Replace($"{entityName}PO", $"Query{entityName}BOOutput")
                .Replace("//AutoMap", $"")
                .Replace("//using", $"")
                .ReplaceAnnotation();
            modifyBoContent.WriteAllText($"{boPath}Modify{entityName}BOInput.cs");
            queryBoContent.WriteAllText($"{boPath}Query{entityName}BOOutput.cs");
            #endregion
        }

        /// <summary>
        /// 生成仓储层接口
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnInfos">表字段信息</param>
        /// <returns></returns>
        public async Task GeneratorIRepositoryAsync(string tableName, List<ColumnInfo> columnInfos)
        {
            var entityName = tableName.ToBigHump();
            var irepositoryPath = new DirectoryInfo(Path).Parent.FullName + $"/ProjectNameTemplate.Core/IRepository/";
            if (!Directory.Exists(irepositoryPath)) Directory.CreateDirectory(irepositoryPath);
            var irepositoryContent = await File.ReadAllTextAsync(Path + "/TemplateFile/IClassTemplateRepository.cs");
            irepositoryContent = irepositoryContent.Replace(ClassTemplateName, entityName)
                .Replace("object", $"{entityName}PO")                
                .Replace("ModifyObject", $"Modify{entityName}BOInput")
                .Replace("QueryObject", $"Query{entityName}BOOutput")
                .Replace("ProjectNameTemplate.CodeGenerator.TemplateFile", "ProjectNameTemplate.Core.IRepository")
                .ReplaceAnnotation();

            irepositoryContent.WriteAllText($"{irepositoryPath}I{entityName}Repository.cs");
        }

        /// <summary>
        /// 生成仓储层
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnInfos">表字段信息</param>
        /// <returns></returns>
        public async Task GeneratorRepositoryAsync(string tableName, List<ColumnInfo> columnInfos)
        {
            var entityName = tableName.ToBigHump();
            var queryColumnNames = string.Join(@"
                        ,", columnInfos.Select(t => t.Name));
            var updateColumnNames = string.Join(@"
                        ,", columnInfos.Select(t => $"{t.Name}=@{t.Name}"));

            var fileContent = await File.ReadAllTextAsync(Path + "/TemplateFile/ClassTemplateRepository.cs");
            fileContent = fileContent.Replace(ClassTemplateName, entityName)
                .Replace("ProjectNameTemplate.CodeGenerator.TemplateFile", "ProjectNameTemplate.Repository")
                .Replace($"query_sql_string", $"select {queryColumnNames} from {tableName};")
                .Replace($"update_sql_string", $"UPDATE {tableName} set {updateColumnNames} where Id=@Id;")
                .Replace($"delete_sql_string", $"DELETE FROM {tableName} where Id=@id;")
                .Replace("object", $"{entityName}PO")                
                .Replace("ModifyObject", $"Modify{entityName}BOInput")
                .Replace("QueryObject", $"Query{entityName}BOOutput")
                .ReplaceAnnotation();

            var repositoryPath = new DirectoryInfo(Path).Parent.FullName + $"/ProjectNameTemplate.Repository/Repository/";
            if (!Directory.Exists(repositoryPath)) Directory.CreateDirectory(repositoryPath);
            fileContent.WriteAllText($"{repositoryPath}{entityName}Repository.cs");
        }

        /// <summary>
        /// 生成Domian的Manager操作类
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnInfos">表字段信息</param>
        /// <returns></returns>
        public async Task GeneratorManagerAsync(string tableName, List<ColumnInfo> columnInfos)
        {
            var entityName = tableName.ToBigHump();
            var fileContent = await File.ReadAllTextAsync(Path + "/TemplateFile/ClassTemplateManager.cs");
            fileContent = fileContent.Replace(ClassTemplateName, entityName)
                .Replace("ProjectNameTemplate.CodeGenerator.TemplateFile", "ProjectNameTemplate.Application")
                .Replace("object", $"{entityName}PO")
                .Replace("classTemplate", $"{entityName.ToSmallHump()}")
                .Replace("AddObject", $"Add{entityName}Input")
                .Replace("ModifyObject", $"Modify{entityName}Input")
                .Replace("QueryObject", $"Query{entityName}Output")
                .Replace("ModifyBO", $"Modify{entityName}BOInput")                
                .ReplaceAnnotation();
            var managerPath = new DirectoryInfo(Path).Parent.FullName + $"/ProjectNameTemplate.Application/Managers/";
            if (!Directory.Exists(managerPath)) Directory.CreateDirectory(managerPath);
            fileContent.WriteAllText($"{managerPath}{entityName}Manager.cs");
        }

        /// <summary>
        /// 生成Controller操作类
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnInfos">表字段信息</param>
        /// <returns></returns>
        public async Task GeneratorControllerAsync(string tableName, List<ColumnInfo> columnInfos)
        {
            var entityName = tableName.ToBigHump();
            var controllerContent = await File.ReadAllTextAsync(Path + "/TemplateFile/ClassTemplateController.cs");
            controllerContent = controllerContent.Replace(ClassTemplateName, entityName)
                .Replace("ProjectNameTemplate.CodeGenerator.TemplateFile", "ProjectNameTemplate.Host.Controllers")
                .Replace("object", $"{entityName}PO")
                .Replace("classTemplate", $"{entityName.ToSmallHump()}")
                .Replace("AddObject", $"Add{entityName}Input")
                .Replace("ModifyObject", $"Modify{entityName}Input")
                .Replace("QueryObject", $"Query{entityName}Output")
                .Replace("ModifyBO", $"Modify{entityName}BOInput")                
                .ReplaceAnnotation();
            var controllerPath = new DirectoryInfo(Path).Parent.FullName + $"/ProjectNameTemplate.Host/Controllers/";
            if (!Directory.Exists(controllerPath)) Directory.CreateDirectory(controllerPath);
            controllerContent.WriteAllText($"{controllerPath}{entityName}Controller.cs");
        }
    }
}