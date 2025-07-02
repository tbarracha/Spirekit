﻿using System.ComponentModel;

namespace SpireApi.Contracts.Dtos.Modules.Authentication;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}

