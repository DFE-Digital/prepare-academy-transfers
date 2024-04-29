using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.Models;

public class ProjectFilterParameters
{
    public List<string> Statuses { get; set; } = new();
    public List<string> AssignedUsers { get; set; } = new();
    public List<string> Regions { get; set; } = new();
    public List<string> LocalAuthorities { get; set; } = new();
    public List<string> AdvisoryBoardDates { get; set; } = new();
}
