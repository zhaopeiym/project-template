using System;
using Talk;

namespace ProjectNameTemplate.Core
{
    public interface ITalkSession : IScopedDependency
    {
        Guid TrackId { get; }
        long UserId { get; set; }
        string UserName { get; set; }

        object MiniProfiler { get; set; }
    }
}
