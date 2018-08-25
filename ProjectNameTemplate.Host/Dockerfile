#基础镜像（用来构建镜像）
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY . .
#docker中的时间需要手动配置时区
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
EXPOSE 80
ENTRYPOINT ["dotnet", "ProjectNameTemplate.Host.dll"]