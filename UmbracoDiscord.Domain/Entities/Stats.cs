using System;
using System.Collections.Generic;

namespace UmbracoDiscord.Domain.Entities
{
    public partial class Stats
    {
        public long Id { get; set; }
        public string ServerId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public DateTime? LastMessage { get; set; }
        public long Experience { get; set; }
    }
}
