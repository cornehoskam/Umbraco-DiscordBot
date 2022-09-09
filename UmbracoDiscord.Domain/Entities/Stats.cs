using System;
using System.Collections.Generic;

namespace UmbracoDiscord.Domain.Entities
{
    public partial class Stats
    {
        public long Id { get; set; }
        public byte[] ServerId { get; set; } = null!;
        public byte[] UserId { get; set; } = null!;
        public byte[]? ServerName { get; set; }
        public byte[]? UserName { get; set; }
        public byte[]? LastMessage { get; set; }
        public byte[] Experience { get; set; } = null!;
    }
}
