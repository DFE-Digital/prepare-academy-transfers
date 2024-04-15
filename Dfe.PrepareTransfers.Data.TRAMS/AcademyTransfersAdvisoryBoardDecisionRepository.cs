using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Dfe.PrepareTransfers.Data.Services.Interfaces;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Data.TRAMS;

public class AcademyTransfersAdvisoryBoardDecisionRepository : IAcademyTransfersAdvisoryBoardDecisionRepository
{
    private readonly IAcademisationHttpClient _academisationHttpClient;

    public AcademyTransfersAdvisoryBoardDecisionRepository(IAcademisationHttpClient academisationHttpClient)
    {
        _academisationHttpClient = academisationHttpClient;
    }

    public async Task Create(AdvisoryBoardDecision decision)
    {
        var content = new StringContent(JsonConvert.SerializeObject(decision), Encoding.Default,
                "application/json");

        HttpResponseMessage response = await _academisationHttpClient.PostAsync("/conversion-project/advisory-board-decision", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new TramsApiException(response);
        }
    }

    public async Task Update(AdvisoryBoardDecision decision)
    {
        var content = new StringContent(JsonConvert.SerializeObject(decision), Encoding.Default,
                "application/json");

        HttpResponseMessage response = await _academisationHttpClient.PutAsync($"/conversion-project/advisory-board-decision", content);
        if (!response.IsSuccessStatusCode)
        {
            throw new TramsApiException(response);
        }
    }

   public async Task<RepositoryResult<AdvisoryBoardDecision>> Get(int projectId)
   {
        HttpResponseMessage response = await _academisationHttpClient.GetAsync($"/transfer-project/advisory-board-decision/{projectId}");

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new RepositoryResult<AdvisoryBoardDecision>
                {
                    Result = null
                };

                throw new TramsApiException(response);
            }
        }

        var apiResponse = await response.Content.ReadAsStringAsync();
        var decision = JsonConvert.DeserializeObject<AdvisoryBoardDecision>(apiResponse);

        return new RepositoryResult<AdvisoryBoardDecision>
        {
            Result = decision
        };
    }
}
