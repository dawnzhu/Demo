using System;
using Newtonsoft.Json;

namespace DotNet.Demo.Utilities
{
    [Serializable]
    public class EnumInfo
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public bool Display { get; set; }

        public int Type { get; set; }

        [JsonIgnore]
        public int Sort { get; set; }

        [JsonIgnore]
        public string[] MutexIds { get; set; }
    }
}
