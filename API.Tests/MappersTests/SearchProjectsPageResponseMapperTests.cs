using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Response;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace API.Tests.MapperTests
{
    public class SearchProjectsPageResponseMapperTests
    {
        private readonly Mapping.SearchProjectsPageResponseMapper _mapper;

        public SearchProjectsPageResponseMapperTests()
        {
            var itemMapper = new SearchProjectsItemMapper();
            _mapper = new Mapping.SearchProjectsPageResponseMapper(itemMapper);
        }

        [Fact]
        public void NullInput_Returns_NullValue()
        {
            var result = _mapper.Map(null);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(0, 0)]
        [InlineData(42, 42)]
        public void TotalPages_MapTest(int input, int expected)
        {
            var inputPageModel = new SearchProjectsD365PageModel
            {
                TotalPages = input
            };

            var result = _mapper.Map(inputPageModel);

            Assert.Equal(expected, result.TotalPages);
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(0, 0)]
        [InlineData(42, 42)]
        public void CurrentPage_MapTest(int input, int expected)
        {
            var inputPageModel = new SearchProjectsD365PageModel
            {
                CurrentPage = input
            };

            var result = _mapper.Map(inputPageModel);

            Assert.Equal(expected, result.CurrentPage);
        }

        [Fact]
        public void ItemMapper_CallTest()
        {
            var itemMapper = new Mock<IMapper<SearchProjectsD365Model, SearchProjectsModel>>();

            itemMapper.Setup(m => m.Map(It.IsAny<SearchProjectsD365Model>()))
                      .Verifiable();

            var pageMapper = new SearchProjectsPageResponseMapper(itemMapper.Object);

            var inputModel = new SearchProjectsD365PageModel
            {
                Projects = new List<SearchProjectsD365Model>
                {
                    new SearchProjectsD365Model(),
                    new SearchProjectsD365Model(),
                    new SearchProjectsD365Model(),
                    new SearchProjectsD365Model()
                }
            };

            var result = pageMapper.Map(inputModel);

            itemMapper.Verify(m => m.Map(It.IsAny<SearchProjectsD365Model>()), Times.Exactly(4));
        }
    }
}
