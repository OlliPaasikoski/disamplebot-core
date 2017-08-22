using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using System;

namespace BasicSampleBot.DocumentDBCore.Models
{
    public class TodoItem
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "completed")]
        public bool Completed { get; set; }
    }
}
