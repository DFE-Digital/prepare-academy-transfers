using Newtonsoft.Json;
using System.Collections.Generic;

namespace API.Models.Downstream.D365
{
    public class ResultSet<TEntity>
    {
        [JsonProperty("value")]
        public List<TEntity> Items { get; set; }
    }
}
