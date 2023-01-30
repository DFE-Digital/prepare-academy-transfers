using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dfe.PrepareTransfers.Data.Models
{
    public enum ThreeOptions
    {
        Yes = 1,
        No = 2,
        [Description("Not applicable")] NotApplicable = 3
    }
}
