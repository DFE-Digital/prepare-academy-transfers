using API.Models.Downstream.D365;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Response;
using System.Linq;

namespace API.Mapping
{
    public class SearchProjectsPageResponseDynamicsMapper : IDynamicsMapper<SearchProjectsD365PageModel, SearchProjectsPageModel>
    {
        private readonly IDynamicsMapper<SearchProjectsD365Model, SearchProjectsModel> _itemDynamicsMapper;

        public SearchProjectsPageResponseDynamicsMapper(IDynamicsMapper<SearchProjectsD365Model, SearchProjectsModel> itemDynamicsMapper)
        {
            _itemDynamicsMapper = itemDynamicsMapper;
        }

        public SearchProjectsPageModel Map(SearchProjectsD365PageModel input)
        {
            if (input == null)
            {
                return null;
            }

            var items = input.Projects?.Select(p => _itemDynamicsMapper.Map(p))
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
