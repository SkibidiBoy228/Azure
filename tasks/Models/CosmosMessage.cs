using System;

namespace tasks.Models
{
    public class CosmosMessage
    {
        public string id { get; set; }
        public Guid groupId { get; set; }
        public string text { get; set; }
    }
}
