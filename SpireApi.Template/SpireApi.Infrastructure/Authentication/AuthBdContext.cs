using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Infrastructure;

namespace SpireApi.Infrastructure.Authentication;

public class AuthBdContext : BaseAuthDbContext
{
    public AuthBdContext(DbContextOptions<AuthBdContext> options) : base(options) { }
}
