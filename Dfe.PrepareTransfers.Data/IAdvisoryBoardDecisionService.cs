using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Data.Services.Interfaces;

public interface IAcademyTransfersAdvisoryBoardDecisionRepository
{
   Task Create(AdvisoryBoardDecision decision);
   Task Update(AdvisoryBoardDecision decision);
   Task<RepositoryResult<AdvisoryBoardDecision>> Get(int id);
}
