using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Authentication.Infrastructure;

namespace SpireApi.Infrastructure.Authentication;

public class AuthDbContext : BaseAuthDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
}
