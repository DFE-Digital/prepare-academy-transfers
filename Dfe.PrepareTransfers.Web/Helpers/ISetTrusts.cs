using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Web.Helpers;

public interface ISetTrusts
{
   void SetTrusts(IEnumerable<TrustSearchResult> trusts);
}
