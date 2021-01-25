using API.Models.Downstream.D365;
using API.ODataHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace API.Tests
{
    public class ODataModelHelperTests
    {
        #region Model Representation Extraction Tests

        internal class NoExpandLevels : BaseD365Model
        {
            [JsonProperty("privatefield")]
            private int privateField { get; set; }

            public Guid PropWithNoAttribute { get; set; }

            [JsonProperty("someproperty")]
            public string SomeProperty { get; set; }

            [JsonProperty("anotherproperty@OData.Community.Display.V1.FormattedValue")]
            public string AnotherProperty { get; set; }
        }

        [Fact]
        public void ExtractModelRepresentation_NoExpandLevels()
        {
            var helper = new D365ModelHelper<NoExpandLevels>();

            var result = helper.ExtractModelRepresentation();

            //No root expand name for root level
            Assert.Equal(string.Empty, result.RootExpandName);
            //Two base properties, one owned
            Assert.Equal(4, result.BaseProperties.Count);
            //Should return the property name with no annotations 
            Assert.Contains("someproperty", result.BaseProperties);
            //Should return the property name with no annotations 
            Assert.Contains("anotherproperty", result.BaseProperties);
            //No nested expansion levels
            Assert.Empty(result.ExpandProperties);
        }

        internal class OneExpandLevelOneExpand : BaseD365Model
        {
            [JsonProperty("someproperty")]
            public Guid SomeProperty { get; set; }

            [JsonProperty("leveloneexpand")]
            public LevelOneExpand LevelOneExpandProp { get; set; }

            public class LevelOneExpand
            {
                [JsonProperty("levelonepropnometadata")]
                public int LevelOneProp { get; set; }

                [JsonProperty("levelonepropwithmetadata@OData.Community.Display.V1.FormattedValue")]
                public string LevelOneAnotherProp { get; set; }
            }
        }

        [Fact]
        public void ExtractModelRepresentation_OneLevelOneExpansion()
        {
            var helper = new D365ModelHelper<OneExpandLevelOneExpand>();

            var result = helper.ExtractModelRepresentation();

            //No root expand name for root level
            Assert.Equal(string.Empty, result.RootExpandName);
            //Two base properties, one owned
            Assert.Equal(3, result.BaseProperties.Count);
            //Should bring back correct name for own property
            Assert.Contains("someproperty", result.BaseProperties);
            //One level one expand
            Assert.Single(result.ExpandProperties);
            //Level one expand has correct name
            Assert.Equal("leveloneexpand", result.ExpandProperties[0].RootExpandName);
            //Level one expand has two properties
            Assert.Equal(2, result.ExpandProperties[0].BaseProperties.Count);
            //Level one expand property with no metadata has correct name
            Assert.Contains("levelonepropnometadata", result.ExpandProperties[0].BaseProperties);
            //Level one expand property with metadata has correct name
            Assert.Contains("levelonepropwithmetadata", result.ExpandProperties[0].BaseProperties);
        }

        internal class TwoLevelOneExpands : BaseD365Model
        {
            [JsonProperty("someproperty")]
            public string SomeProperty { get; set; }

            [JsonProperty("leveloneexpand_one")]
            public LevelOneExpandOne LevelOneExpandPropOne { get; set; }

            [JsonProperty("leveloneexpand_two")]
            public LevelOneExpandTwo LevelOneExpandPropTwo { get; set; }

            public class LevelOneExpandOne
            {
                [JsonProperty("LevelOneExpand_One_Prop1")]
                public string LevelOneProp { get; set; }

                [JsonProperty("LevelOneExpand_One_Prop2@OData.Community.Display.V1.FormattedValue")]
                public string LevelOneAnotherProp { get; set; }
            }

            public class LevelOneExpandTwo
            {
                [JsonProperty("LevelOneExpand_Two_Prop1")]
                public string LevelOneProp { get; set; }

                [JsonProperty("LevelOneExpand_Two_Prop2@OData.Community.Display.V1.FormattedValue")]
                public string LevelOneAnotherProp { get; set; }

                [JsonProperty("LevelOneExpand_One_Prop3")]
                public string YetAnotherProp { get; set; }
            }
        }

        [Fact]
        public void ExtractModelRepresentation_TwoLevelOneExpansions()
        {
            var helper = new D365ModelHelper<TwoLevelOneExpands>();

            var result = helper.ExtractModelRepresentation();

            //No root expand name for root level
            Assert.Equal(string.Empty, result.RootExpandName);
            //Two base properties, one owned
            Assert.Equal(3, result.BaseProperties.Count);
            //Two Level Two expands
            Assert.Equal(2, result.ExpandProperties.Count);
            //First Level One expansion has correct root name
            Assert.Equal("leveloneexpand_one", result.ExpandProperties[0].RootExpandName);
            //First Level One expansion has two properties
            Assert.Equal(2, result.ExpandProperties[0].BaseProperties.Count);
            //First Level One expansion -> first property has correct name
            Assert.Equal("LevelOneExpand_One_Prop1", result.ExpandProperties[0].BaseProperties[0]);
            //First Level One expansion -> second property has correct name
            Assert.Equal("LevelOneExpand_One_Prop2", result.ExpandProperties[0].BaseProperties[1]);
            //First Level one expansion has no nested expands
            Assert.Empty(result.ExpandProperties[0].ExpandProperties);
            //Second Level One Expansion Has Correct Root Name
            Assert.Equal("leveloneexpand_two", result.ExpandProperties[1].RootExpandName);
            //Second Level One expansion has two properties
            Assert.Equal(3, result.ExpandProperties[1].BaseProperties.Count);
            //Second Level One expansion -> first property has correct name
            Assert.Equal("LevelOneExpand_Two_Prop1", result.ExpandProperties[1].BaseProperties[0]);
            //Second Level One expansion -> second property has correct name
            Assert.Equal("LevelOneExpand_Two_Prop2", result.ExpandProperties[1].BaseProperties[1]);
            //Second Level One expansion -> third property has correct name
            Assert.Equal("LevelOneExpand_One_Prop3", result.ExpandProperties[1].BaseProperties[2]);
            //Second Level one expansion has no nested expands
            Assert.Empty(result.ExpandProperties[1].ExpandProperties);
        }

        internal class OneLevelTwoExpand : BaseD365Model
        {
            [JsonProperty("someproperty")]
            public Guid SomeProperty { get; set; }

            [JsonProperty("leveloneexpand")]
            public LevelOneExpand LevelOneExpandProp { get; set; }

            internal class LevelOneExpand
            {
                [JsonProperty("leveloneexpandprop")]
                public int LevelOneProp { get; set; }

                [JsonProperty("leveltwoexpand")]
                public List<LevelTwoExpand> LevelTwoExpandProp { get; set; }

                internal class LevelTwoExpand
                {
                    [JsonProperty("leveltwoexpandprop")]
                    public Guid LevelTwoExpandProp { get; set; }
                }
            }
        }

        [Fact]
        public void ExtractModelRepresentation_OneLevelTwoExpansion()
        {
            var helper = new D365ModelHelper<OneLevelTwoExpand>();

            var result = helper.ExtractModelRepresentation();

            //No root expand name for root level
            Assert.Equal(string.Empty, result.RootExpandName);
            //Three base properties, one owned
            Assert.Equal(3, result.BaseProperties.Count);
            //Should containe owned property
            Assert.Contains("someproperty", result.BaseProperties);
            //One Level One expand
            Assert.Single(result.ExpandProperties);
            //First Level One expansion has correct root name
            Assert.Equal("leveloneexpand", result.ExpandProperties[0].RootExpandName);
            //First Level One expansion has one property
            Assert.Single(result.ExpandProperties[0].BaseProperties);
            //First Level One expansion -> first property has correct name
            Assert.Equal("leveloneexpandprop", result.ExpandProperties[0].BaseProperties[0]);
            //Level One expand contains nested expand
            Assert.Single(result.ExpandProperties[0].ExpandProperties);
            //Level Two expand has correct root name
            Assert.Equal("leveltwoexpand", result.ExpandProperties[0].ExpandProperties[0].RootExpandName);
            //Level Two expand has one property
            Assert.Single(result.ExpandProperties[0].ExpandProperties[0].BaseProperties);
            //Level Two expand has correct property name
            Assert.Equal("leveltwoexpandprop", result.ExpandProperties[0].ExpandProperties[0].BaseProperties[0]);
            //Level Two expand has no nested expand
            Assert.Empty(result.ExpandProperties[0].ExpandProperties[0].ExpandProperties);
        }

        #endregion

        #region Select and Expand Clause Builder Tests

        [Fact]
        public void NoExpandLevels_ReturnsCorrectQuery()
        {
            var representation = new D365ModelRepresentation
            {
                RootExpandName = string.Empty,
                BaseProperties = new List<string>
                {
                    "property_one",
                    "property_two"
                },
                ExpandProperties = new List<D365ModelRepresentation>()
            };

            var helper = new D365ModelHelper<BaseD365Model>();

            var result = helper.BuildSelectAndExpandClauses(representation);

            Assert.Equal("$select=property_one,property_two", result);
        }

        [Fact]
        public void OneLevelOneExpand_ReturnsCorrectQuery()
        {
            var representation = new D365ModelRepresentation
            {
                RootExpandName = string.Empty,
                BaseProperties = new List<string>
                {
                    "property_one",
                    "property_two"
                },
                ExpandProperties = new List<D365ModelRepresentation>
                {
                    new D365ModelRepresentation
                    {
                        RootExpandName = "expand_root",
                        BaseProperties = new List<string>
                        {
                            "expand1_prop1",
                            "expand1_prop2"
                        }
                    }
                }
            };

            var helper = new D365ModelHelper<BaseD365Model>();

            var result = helper.BuildSelectAndExpandClauses(representation);

            Assert.Equal("$select=property_one,property_two&$expand=expand_root($select=expand1_prop1,expand1_prop2)", result);
        }

        [Fact]
        public void ThreeLevelOneExpands_ReturnsCorrectQuery()
        {
            var representation = new D365ModelRepresentation
            {
                RootExpandName = string.Empty,
                BaseProperties = new List<string> { "property_one", "property_two" },
                ExpandProperties = new List<D365ModelRepresentation>
                {
                    new D365ModelRepresentation
                    {
                        RootExpandName = "expand_root1",
                        BaseProperties = new List<string> { "expand1_prop1", "expand1_prop2" }
                    },
                    new D365ModelRepresentation
                    {
                        RootExpandName = "expand_root2",
                        BaseProperties = new List<string> { "expand2_prop1", "expand2_prop2" }
                    },
                    new D365ModelRepresentation
                    {
                        RootExpandName = "expand_root3",
                        BaseProperties = new List<string> { "expand3_prop1", "expand3_prop2" }
                    }
                }
            };

            var helper = new D365ModelHelper<BaseD365Model>();

            var result = helper.BuildSelectAndExpandClauses(representation);

            Assert.Equal("$select=property_one,property_two&$expand=expand_root1($select=expand1_prop1,expand1_prop2),expand_root2($select=expand2_prop1,expand2_prop2),expand_root3($select=expand3_prop1,expand3_prop2)", result);
        }

        [Fact]
        public void TwoLevelTwoExpansion_ShouldReturnCorrectQuery()
        {
            var representation = new D365ModelRepresentation
            {
                RootExpandName = string.Empty,
                BaseProperties = new List<string> { "property_one", "property_two" },
                ExpandProperties = new List<D365ModelRepresentation>
                {
                    new D365ModelRepresentation
                    {
                        RootExpandName = "expand_root1",
                        BaseProperties = new List<string> { "expand1_prop1", "expand1_prop2" },
                        ExpandProperties = new List<D365ModelRepresentation>
                        {
                            new D365ModelRepresentation
                            {
                                RootExpandName = "expand_root2",
                                BaseProperties = new List<string> { "expand2_prop1", "expand2_prop2" }
                            },
                            new D365ModelRepresentation
                            {
                                RootExpandName = "expand_root3",
                                BaseProperties = new List<string> { "expand3_prop1", "expand3_prop2" }
                            }
                        }
                    },
                }
            };

            var helper = new D365ModelHelper<BaseD365Model>();

            var result = helper.BuildSelectAndExpandClauses(representation);

            Assert.Equal("$select=property_one,property_two&$expand=expand_root1($select=expand1_prop1,expand1_prop2;$expand=expand_root2($select=expand2_prop1,expand2_prop2),expand_root3($select=expand3_prop1,expand3_prop2))", result);
        }

        [Fact]
        public void JaggedExpansionLevels_ShouldReturnCorrectQuery()
        {
            var representation = new D365ModelRepresentation
            {
                RootExpandName = string.Empty,
                BaseProperties = new List<string> { "property_one", "property_two" },
                ExpandProperties = new List<D365ModelRepresentation>
                {
                    new D365ModelRepresentation
                    {
                        RootExpandName = "expand_root1",
                        BaseProperties = new List<string> { "expand1_prop1", "expand1_prop2" },
                        ExpandProperties = new List<D365ModelRepresentation>
                        {
                            new D365ModelRepresentation
                            {
                                RootExpandName = "expand_root2",
                                BaseProperties = new List<string> { "expand2_prop1", "expand2_prop2" }
                            },
                            new D365ModelRepresentation
                            {
                                RootExpandName = "expand_root3",
                                BaseProperties = new List<string> { "expand3_prop1", "expand3_prop2" },
                                ExpandProperties = new List<D365ModelRepresentation>
                                {
                                    new D365ModelRepresentation
                                    {
                                        RootExpandName = "expand_root4",
                                        BaseProperties = new List<string> { "expand4_prop1", "expand4_prop2" }
                                    },
                                    new D365ModelRepresentation
                                    {
                                        RootExpandName = "expand_root5",
                                        BaseProperties = new List<string> { "expand5_prop1", "expand5_prop2" }
                                    }
                                }
                            }
                        }
                    },
                }
            };

            var helper = new D365ModelHelper<BaseD365Model>();

            var result = helper.BuildSelectAndExpandClauses(representation);

            Assert.Equal("$select=property_one,property_two&$expand=expand_root1($select=expand1_prop1,expand1_prop2;$expand=expand_root2($select=expand2_prop1,expand2_prop2),expand_root3($select=expand3_prop1,expand3_prop2;$expand=expand_root4($select=expand4_prop1,expand4_prop2),expand_root5($select=expand5_prop1,expand5_prop2)))", result);
        }

        #endregion
    }
}
