using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dfe.PrepareTransfers.Data.Models.Projects.TransferFeatures;

namespace Dfe.PrepareTransfers.Data.Models.Projects
{
    public class ReasonParentAttribute : Attribute
    {
        public ReasonForTheTransferTypes Parent { get; private set; }

        public ReasonParentAttribute(ReasonForTheTransferTypes parent)
        {
            this.Parent = parent;
        }
    }
}
