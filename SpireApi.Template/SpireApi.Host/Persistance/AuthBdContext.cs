using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Persistance;

namespace SpireApi.Host.Persistance;

public class AuthBdContext : BaseAuthDbContext
{
    public AuthBdContext(DbContextOptions<AuthBdContext> options) : base(options) { }
}
