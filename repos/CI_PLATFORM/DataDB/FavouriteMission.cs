using System;
using System.Collections.Generic;

namespace CI_PLATFORM.DataDB;

public partial class FavouriteMission
{
    public long FavouriteMissionId { get; set; }

    public long UserId { get; set; }

    public long MissionId { get; set; }

    public string? ApprovalStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
