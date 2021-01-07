using API.HttpHelpers;
using API.Models.D365;
using API.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private static readonly string _route = "sip_academytransfersprojects";

        private readonly AuthenticatedHttpClient _client;
        private readonly IOdataUrlBuilder<GetAcademyTransfersProjectsD365Model> _urlBuilder;

        public ProjectsRepository(AuthenticatedHttpClient client,
                                  IOdataUrlBuilder<GetAcademyTransfersProjectsD365Model> urlBuilder)
        {
            _client = client;
            _urlBuilder = urlBuilder;
        }

        public async Task<List<GetAcademyTransfersProjectsD365Model>> GetAll()
        {
            var url = _urlBuilder.BuildFilterUrl(_route, null);

            await _client.AuthenticateAsync();

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var castedResults = JsonConvert.DeserializeObject<ResultSet<GetAcademyTransfersProjectsD365Model>>(results);

                return castedResults.Items;
            }

            return new List<GetAcademyTransfersProjectsD365Model>();
        }

        public async Task<GetAcademyTransfersProjectsD365Model> GetProjectById(Guid id)
        {
            var url = _urlBuilder.BuildRetrieveOneUrl(_route, id);

            await _client.AuthenticateAsync();

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var castedResults = JsonConvert.DeserializeObject<GetAcademyTransfersProjectsD365Model>(results);

                return castedResults;
            }

            return null;
        }

        public async Task<Guid?> InsertProject(PostAcademyTransfersProjectsD365Model project)
        {
            await _client.AuthenticateAsync();

            var jsonBody = JsonConvert.SerializeObject(project);

            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await _client.PostAsync(_route, byteContent);

            var res = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                if (result.Headers.TryGetValues("OData-EntityId", out var headerValues))
                {
                    var value = headerValues.First();
                    var guidString = value.Substring(value.Length - 37, 36);

                    if (Guid.TryParse(guidString, out var guidValue))
                    {
                        return guidValue;
                    }
                }


            return null;
        }
    }
}
