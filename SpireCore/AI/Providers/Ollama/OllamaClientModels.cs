using System.Text.Json.Serialization;

namespace SpireCore.AI.Providers.Ollama;

public class OllamaGenerateRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "";

    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; }

    [JsonPropertyName("images")]
    public List<string>? Images { get; set; }

    [JsonPropertyName("think")]
    public bool? Think { get; set; }

    [JsonPropertyName("format")]
    public object? Format { get; set; } // can be "json" or a JSON schema object

    [JsonPropertyName("options")]
    public object? Options { get; set; } // e.g. { "temperature": 0.7 }

    [JsonPropertyName("system")]
    public string? System { get; set; }

    [JsonPropertyName("template")]
    public string? Template { get; set; }

    [JsonPropertyName("stream")]
    public bool? Stream { get; set; }

    [JsonPropertyName("raw")]
    public bool? Raw { get; set; }

    [JsonPropertyName("keep_alive")]
    public string? KeepAlive { get; set; }

    [JsonPropertyName("context")]
    public object? Context { get; set; } // deprecated
}

public class OllamaGenerateResponse
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }

    [JsonPropertyName("response")]
    public string? Response { get; set; }

    [JsonPropertyName("done")]
    public bool? Done { get; set; }

    [JsonPropertyName("done_reason")]
    public string? DoneReason { get; set; }

    [JsonPropertyName("context")]
    public object? Context { get; set; }

    [JsonPropertyName("total_duration")]
    public long? TotalDuration { get; set; }

    [JsonPropertyName("load_duration")]
    public long? LoadDuration { get; set; }

    [JsonPropertyName("prompt_eval_count")]
    public int? PromptEvalCount { get; set; }

    [JsonPropertyName("prompt_eval_duration")]
    public long? PromptEvalDuration { get; set; }

    [JsonPropertyName("eval_count")]
    public int? EvalCount { get; set; }

    [JsonPropertyName("eval_duration")]
    public long? EvalDuration { get; set; }
}
