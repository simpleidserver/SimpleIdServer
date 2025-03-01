using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations;

public class DataSeederDbContext : DbContext
{
    public List<DataSeederExecutionHistory> ExecutionHistories { get; set; }
}
