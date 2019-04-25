using ProjectNameTemplate.Core;
using System;

namespace ProjectNameTemplate.Infrastructure
{
    public class TalkSession : ITalkSession
    {
        public Guid TrackId { get; set; }
        public Guid ParentTrackId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public object MiniProfiler { get; set; }
    }
}
