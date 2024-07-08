using System;
using System.Collections.Generic;

namespace LeoPasswordManager.Contexts;

public partial class Nlog
{
    public DateTime? Datelogged { get; set; }

    public string? Level { get; set; }

    public string? Message { get; set; }
}
