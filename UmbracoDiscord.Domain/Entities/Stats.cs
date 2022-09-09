using System;
using System.Collections.Generic;

namespace UmbracoDiscord.Domain.Entities
{
    public partial class Stats
    {
        public long Id { get; set; }
        public byte[] ServerId { get; set; }
        public byte[] UserId { get; set; }
        public byte[] Experience { get; set; }
    }
}
