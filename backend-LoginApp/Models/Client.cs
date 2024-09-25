using System;
using System.Collections.Generic;

namespace backend_LoginApp.Models;

public partial class Client
{
    public int IdClient { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public string Password { get; set; } = null!;
}
