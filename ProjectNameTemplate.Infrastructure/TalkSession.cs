using ProjectNameTemplate.Core;
using System;

namespace ProjectNameTemplate.Infrastructure
{
    public class TalkSession : ITalkSession
    {
        public Guid TrackId { get; }
        public long UserId { get; set; }
        public string UserName { get; set; }
    }
}
