using API.Models.Downstream.D365;
using API.Models.Upstream.Response;
using System.Linq;

namespace API.Mapping
{
    public class SearchProjectsPageResponseMapper : IMapper<SearchProjectsD365PageModel, SearchProjectsPageModel>
    {
        private readonly IMapper<SearchProjectsD365Model, SearchProjectsModel> _itemMapper;

        public SearchProjectsPageResponseMapper(IMapper<SearchProjectsD365Model, SearchProjectsModel> itemMapper)
        {
            _itemMapper = itemMapper;
        }

        public SearchProjectsPageModel Map(SearchProjectsD365PageModel input)
        {
            if (input == null)
            {
                return null;
            }

            var items = input.Projects?.Select(p => _itemMapper.Map(p))
                                       .ToList();

            return new SearchProjectsPageModel
            {
                CurrentPage = input.CurrentPage,
                TotalPages = input.TotalPages,
                Projects = items
            };
        }
    }
}
