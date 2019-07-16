﻿using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace ProjectNameTemplate.WebApi.Filters
{
    public class OperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            operation.Parameters.Add(new NonBodyParameter()
            {
                Name = "Authorization",  //添加Authorization头部参数
                In = "header",
                Type = "string",
                Required = false
            });

        }
    }
}
