using System;
using System.Collections.Generic;

namespace CI_PLATFORM.Models;

public partial class PasswordReset
{
    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public int Id { get; set; }

    public DateTime? CreatedAt { get; set; }
}
