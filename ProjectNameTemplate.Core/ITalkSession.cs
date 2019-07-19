using System;
using Talk;

namespace ProjectNameTemplate.Core
{
    public interface ITalkSession : IScopedDependency
    {
        Guid TrackId { get; set; }
        Guid ParentTrackId { get; set; }
        long UserId { get; set; }
        string UserName { get; set; }

        object MiniProfiler { get; set; }
        string ControllerName { get; set; }
        string RequestUrl { get; set; }
        string ActionName { get; set; }
        bool NoJsonResult { get; set; }
    }
}
