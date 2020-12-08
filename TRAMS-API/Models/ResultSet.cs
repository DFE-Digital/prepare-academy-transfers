using Newtonsoft.Json;
using System.Collections.Generic;

namespace API.HttpHelpers
{
    public class ResultSet<TEntity>
    {
        [JsonProperty("value")]
        public List<TEntity> Items { get; set; }
    }
}
