using System.ComponentModel;

namespace {Namespace}.Contracts.Dtos.Modules.Authentication;

public class RegisterRequestDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}

