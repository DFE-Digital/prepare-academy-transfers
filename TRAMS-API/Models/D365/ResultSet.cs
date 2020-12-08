using Newtonsoft.Json;
using System.Collections.Generic;

namespace API.Models.D365
{
    public class ResultSet<TEntity>
    {
        [JsonProperty("value")]
        public List<TEntity> Items { get; set; }
    }
}
