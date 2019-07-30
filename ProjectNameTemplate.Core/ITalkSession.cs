using System;
using Talk;
using Talk.Contract;

namespace ProjectNameTemplate.Core
{
    public interface ITalkSession : IScopedDependency
    {
        Guid TrackId { get; set; }       
        long UserId { get; set; }
        string UserName { get; set; }

        object MiniProfiler { get; set; }
        string ControllerName { get; set; }
        string RequestUrl { get; set; }
        string ActionName { get; set; }
        bool NoJsonResult { get; set; }
        RPCContext RPCContext { get; set; }
    }
}
