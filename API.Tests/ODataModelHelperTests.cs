using API.Mapping;
using API.Models.D365;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace API.Tests
{
    public class ODataModelHelperTests
    {
        [Fact]
        public void OneLevelExpansion()
        {
            var helper = new ExpandODataModelHelper<GetAcademiesD365Model>();

            var result = helper.GetSelectAndExpandClauses();

            var queryString = helper.BuildTopLevelExpand(result);

            var debug = 0;
        }
    }

    internal class OneExpandLevel : BaseD365Model
    {
        [JsonProperty("someproperty")]
        public string SomeProperty { get; set; }

        [JsonProperty("leveloneexpand")]
        public LevelOneExpand LevelOneExpandProp { get; set; }

        public class LevelOneExpand
        {
            [JsonProperty("leveloneprop")]
            public string LevelOneProp { get; set; }
        }
    }
}
