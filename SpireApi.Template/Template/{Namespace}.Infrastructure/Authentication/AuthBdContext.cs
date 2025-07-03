using Microsoft.EntityFrameworkCore;
using {Namespace}.Application.Modules.Authentication.Infrastructure;

namespace {Namespace}.Infrastructure.Authentication;

public class AuthBdContext : BaseAuthDbContext
{
    public AuthBdContext(DbContextOptions<AuthBdContext> options) : base(options) { }
}

