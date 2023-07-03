using System;
using System.Collections.Generic;

namespace CI_PLATFORM.DataDB;

public partial class MissionApplication
{
    public long MissionAppplicationId { get; set; }

    public long MissionId { get; set; }

    public long UserId { get; set; }

    public DateTime? AppliedAt { get; set; }

    public string? ApprovalStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Mission Mission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
