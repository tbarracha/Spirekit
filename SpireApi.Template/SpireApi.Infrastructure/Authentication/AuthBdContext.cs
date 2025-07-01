using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Persistance;

namespace SpireApi.Infrastructure.Authentication;

public class AuthBdContext : BaseAuthDbContext
{
    public AuthBdContext(DbContextOptions<AuthBdContext> options) : base(options) { }
}
