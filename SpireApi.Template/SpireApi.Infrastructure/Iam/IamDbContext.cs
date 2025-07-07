using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Infrastructure.Iam;

public class IamDbContext : BaseIamDbContext
{
    public IamDbContext(DbContextOptions options) : base(options) { }
}
