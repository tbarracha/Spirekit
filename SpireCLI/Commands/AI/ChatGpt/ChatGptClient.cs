using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace SpireCLI.Commands.AI.ChatGpt;

public class ChatGptClient
{
    private readonly HttpClient _httpClient;

    public ChatGptClient(string cookie, string authorizationToken)
    {
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            AllowAutoRedirect = true
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://chatgpt.com/ ")
        };

        // Set default headers
        _httpClient.DefaultRequestHeaders.Add("authority", "chatgpt.com");
        _httpClient.DefaultRequestHeaders.Add("accept", "text/event-stream");
        _httpClient.DefaultRequestHeaders.Add("accept-language", "pt-PT,pt;q=0.9");
        _httpClient.DefaultRequestHeaders.Add("origin", "https://chatgpt.com ");
        _httpClient.DefaultRequestHeaders.Add("referer", "https://chatgpt.com/?model=gpt-4o&temporary-chat=true");
        _httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Brave\";v=\"138\"");
        _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-arch", "\"x86\"");
        _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-bitness", "\"64\"");
        _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"Not)A;Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"138.0.0.0\", \"Brave\";v=\"138.0.0.0\"");
        _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
        _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-model", "\"\"");
        _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
        _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "\"19.0.0\"");
        _httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
        _httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
        _httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
        _httpClient.DefaultRequestHeaders.Add("sec-gpc", "1");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");

        _httpClient.DefaultRequestHeaders.Add("oai-client-version", "prod-e62e936d439cfd1433471a11d6e9a7f3feda3953");
        _httpClient.DefaultRequestHeaders.Add("oai-device-id", "f908ae8a-1651-4d97-9fe6-4825e2e5490d");
        _httpClient.DefaultRequestHeaders.Add("oai-language", "en-US");
        _httpClient.DefaultRequestHeaders.Add("openai-sentinel-chat-requirements-token", "gAAAAABoaG8lSMJJsH7GeVbuq6GuZryZak6pC9TM_b_-d4BCV0Wfbm8qAArNk0Y7TO-9YMiR_0o3pXzyqGe9j107HHToSZURGXhE7aCf5E6jL-f6kDDdQixpPjk0_qhUImrVlpdWfWJpHe9UOwyszjJvTNmFq4qPs-A3eNj8XLEBl25R6B33aSKlCIordY1Du80GMhaaO9vWKlKBjU579NmBJq2aAp5Hnyil90CfK9Sl0aHse4UpS-4vCloHS8tR9yf2yB3qAs_3ve0CwjLSqODWMUjDnpPZdND3tfQQfr7uNxUwnEtAsVrfl44LzewOzgZorCC_eqCGMXxD7wNqVLewxzJCyhg2NtutpXnMtiJJAxpIRerHxoYB1Zr_9Xbk0OIqQpGvC42pwjKT1vuGK2jc7Jh5lP5mId_KWTtGCcRJr7j4IKbqlP-qCu2BS3cR5KJfHGJyZhJVDrSzz6JViBtYLpPxR9_80szDydjcfwcm6YjGP2CMroW0u4a-PsFHGLeYoCDbpGVSl-GDgw-pGSZ006q-AvqxDuBFsrEJWdBzdzE5EhTkQzqgDRLgQtBGHSegxACx6VZg45J7QsdbTE2YQmwIZCUDTX8k9R-2AFJA_U9hnOGrzWPloHXtD7I3qKuCBZx5nG-1LiorIbqtAG0icxL5fgH8C43Q4pbVxUeIJ9k_J3EiTCYxx8GDW1KGQMQnp5eDXVbKE6AJ3yNc6wguvUckZq63ARBSlKct3FckQpJoMbbZRcsJmqNCwGYwd8dE1ypfx8R6S0ox5YX-QSc2VtI-ch5UHpGc8WH6E2AGnPBajD9lgJS2_2oThDemv1guwZoAWLwFOEBjZ0EQ5jXGrs7ZG-ZXf8c3mpGr8hzMSznwG1WUeboHmiiK-54XCE9-oPlUTiOnPfQr0uHQIX_LbpeJ6x0soTowi8y_rlD7gHvdccbXk77iJ3pW7-bnbBlmQmxhHbV639C1loEkEx5ixQfG8mEpsPMT_gWBoeaV2qj5u5qqFKSMlQhurj7cE3Lu2vsmoeyW1MMWDJqFfblgSXGOT-L0Dij0YWJf8HTvQ8GInnaP0X1NX99VMu8FD_RJIYbOlXiMBnBJ1tKg9eKhNzC7bn11tCkliu7vPkl1tPb4B3N0nnyP5vsUu5XM29oPyyQAHZDptFVJzSqlDdPguuMaPXYS6dN1g8X4Rc5Gap0g1wytdCqIAJumtknSiyIh6AWv4mW-GQyvqPeHR7ziApEeOd10fYCf3En2JodTH3wwlEGZpGs_HMf3k13AELrApM2wKOnQvVE2S5zpHHcG6skQmlcXN0rlKyRhVmR0QxZBdlZnj3ybl2gLLnbxty8VPKcGKAKyueuy_Z0RviXU6q_JnrHOu9wFL4Y0yLN4l_RNIvchk2XXR6kUq7df4HiOZnxKImDxemsf1pc5s1--ruixlNIXSIb6dtS15F9grGhBTszKTh4MH24CkKEQBUPdMsbrwkg-7Ebs1XydK0WIeWBcjSsOC2QUn78ZA660rYuPDQvZ0Y_UuT77JuDGrN8Kp14W_uLIZ1bcw41T06jOvnSLyuBF6GT9K8JV8l304DyFTMG3rxHAg_31Yq-4g04HgTRBwsbv2Ljn8ks9o0hUytvxQ9UmhTZ9JVmYPCepwoE3OcgQJ3Y-rcPkVQBqi9kvdbw4hJLiopDDGhArEKK7_FLtHoRHVtsilizmI3SrYVK-uuuLgbOLuJHh_88bSB2trJIvPzE5cVpLtwcJKazIaxq9V8-LOjPbGXliCL9WVkXcbSkphQ1gJcoKsEQY88aVooYkhiN6dRahPKMK14l63e2RMSOBXckjk-R47SxfJ-KhRPOmj9r4lDOxSLSZvfNCpFPKUAaxC02M42xkDOWcYvl5nqqFwoHhZXP6TWjxPBquDjTUC_mde_3dgxuuAyNIlpwZEc2-tXZ81G6JO8ztIoj-czz10xzIgqinCOmanqhl6HwK07bdmkVaZ3X5AJFSAixd7--xSShps95r11jnkf2OikCq_vJKeOxG2Mp9e934XWapfjYti_vgIJj_42ZnGDV04-92w9FqvSHcqEi4ClAk8isoQrS2-vr-JXUcbnCEe-y5RZ8J9ANk7f4IYMH3oA5-uvvJ3z-BWnVXCKN2naqI9oXDDL25dI9ma7sDyuL0HDQnVAp95AjZi_pDKegcEa8sPJovqKKp41YA-uOr_LvH9fSi2J12bq0demvyJqIcD2hxFE0NHVrKHRP-KRdz8hoebj_u");
        _httpClient.DefaultRequestHeaders.Add("openai-sentinel-proof-token", "gAAAAABWzIwODAsIlNhdCBKdWwgMDUgMjAyNSAwMToxNzo1MiBHTVQrMDEwMCAoSG9yYSBkZSB2ZXLDo28gZGEgRXVyb3BhIE9jaWRlbnRhbCkiLDQyOTQ3MDUxNTIsMTgsIk1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQpIEFwcGxlV2ViS2l0LzUzNy4zNiAoS0hUTUwsIGxpa2UgR2Vja28pIENocm9tZS8xMzguMC4wLjAgU2FmYXJpLzUzNy4zNiIsbnVsbCwicHJvZC1lNjJlOTM2ZDQzOWNmZDE0MzM0NzFhMTFkNmU5YTdmM2ZlZGEzOTUzIiwicHQtUFQiLCJwdC1QVCIsMTcsIndlYmtpdEdldFVzZXJNZWRpYeKIkmZ1bmN0aW9uIHdlYmtpdEdldFVzZXJNZWRpYSgpIHsgW25hdGl2ZSBjb2RlXSB9IiwiX3JlYWN0TGlzdGVuaW5nZmhncHNnMTAwbjciLCJvbnBvaW50ZXJvdmVyIiwxNjI3LjcwMDAwMDAwMjk4MDIsImY5MjYwM2NhLWY2MmMtNDcwZi1iM2ZmLWVmNWVhNjRkNDZjYiIsIm1vZGVsIiwxMiwxNzUxNjc0NjcwNTIyLjZd~S");
        _httpClient.DefaultRequestHeaders.Add("openai-sentinel-turnstile-token", "TxAZAhcDBAwNG3ZgWW4bGBAWABcFAwwNG1ZKb1R7Q1lodHtuZW1mDXJmb2FgcFV0eXtDWWFhQW5nalAMbHRZcnRwXmNRbAVBYWF7AGJvZWBRdmltdGFkG1VtZkF3cn5+V31bY1BjbGJvclVrYH5lBmx1e0dzaU9eZXNZA2F2VU1WeFNVeVVOCQ8MGxsFCgAEDBYIDFVBdkJ5cWNgakVEYVd0XmV4dmd4cVRienlmVnN3b35pcmN8QRsYEBoCFwYDDA0bYwNgf35yWmtmVQRrfVt/YGpoZWtuA0pWaAVwT2FvBFB9XHtgZUVlaGFKVlNoBVp5b1JAXXtxeFd6b252cGNrVXhDQW10Tn5reXEJUGYeU3NmSh95b1hWdG9vTGFPWwhjYR9hYGZfeH9sBANNf1JMYG92Y2x6WWVbYl5/VnhTd2tia0NQeFtvVnN4bXNmd2t1b3JaRmZVBWF4YVFDZh9bYWFnb3RuWFprYnxyYmgGf3FhH1t3V3RgVm9cVmxlCVBmb09RUGYfA25iZ2RSa3ZoTWIIBVl2cX9Xa2hfDWVndHBfWGB4YWxAY3ZxSQxlHg9vYmNBVGxYRWx2QW5heUBscmFvfVthcBtTaAVab2ZsUGNoBl1gZngOWmFwVntoYlp3ZQkNdntxTVdqfFxydkV3bX1TWWp2UVNwaGFNYGBWX2Bmd2RTfnJ0eGZuRFd9W1VWantlWmF1dHtvWGBmUXxyAHpye1VXHlNyYWQbdH4EQm9iCER3e3F0V2p4fXJmA1p1bmNoSmFedXVqUAl1dW9uc3Bza1J+BWh4YE5yZX1ia3ZzeERwdlVrb2wFe2h/bAx3dkB7ZXpvAl1zc3dteHJGeHVRBHBtQGhxekJudHVjf3RuclpKf2wNen1iWlBjf2UJbGRWf18FcHpgVFBoS1tNV2VCfXtldUpSa3JKd29UTGV4BklsamxDbmJkXXVvX15rZmhyUHlyXXNUQm1cYWobdW9YA1liCERhaGJdc2YeeXJsZHt2b3JadmBSQGZoBn9xZh9DdGJndH9uclZmf3tTYm1mcHF6WVR7cHAbU2gFWm9mbFBjaAddc2YfYW9hZR9/bExRaHVrV3RqQGx3dElmc3N1Sn9jcQN4ZVJHaHlbQWBlQ3lrZl5KZ2wHC3dkCXJxemBdc2YeeVpzdB9nYVxoeGZVAXdoW11QY2txWmZZaGdoBXBMb35EdHlxc3NqfENuYmRddW9fXmtmaHJQeFt3YmEfeU5jX0pTbF9GG2EIBXd5cXdsUHhfdGNZd3RvYkp2dnxHdWpQbHF1f1NacHR0e29cZGZlUg12fVsIYnNrZVpmAmh/bnJWXGAJW2htQGxydFlmdHVzb259XGRAbVQNWE9ZCAVRdkMLUAFgXlhzQklRYUwASFl/QFR5eQ1QXBtFWHxwWX9VRFR2cVVsc0Vld2JndFNhWAZPYmwNZ3thf2NzaH1pYmQbVW9fA0pvbHZ3aEBgf3VJfnRwc0Fte2UCd2ZvRGV6Yl1WYXxbWGJkaHBsWAJPYQhcZXhif2BlQgJuY1lrdnt1VWB1UQx2bXZsdXpWQ3JldGBWa3Jwen98fmV9XHtAYx9fXWxkfH9sWANcZVJQU3Z1SWxlaG1vZWRgVmFYaE9maH5reXEJUGYeU3NmSh9wb19CQGEJQGhLcXtgaB9bd2FnbGdvcQNKZXxAWHlxY1FhVlBpdkVnU3gFCm92aHlib2ZOcHNZeVh2c3dzfVhaeXZ4eXN5UHx/dWhidGJVY1NvYgJ3YmwNZ3thfFBlHmVaZloXX25jC0h0UkRifXJdXVcecQtiX2BtWlpwGG5vBXZtYGhQdW9mc3ZFdH54Q110dFFtYm11UnNmf1R3c2NaZ29lQXR1bARxb2ZzZXUfYnB2A2x8fnFCT29vTFB5ck5SYEICcmV0H1RtXGRmZVINdn1bCGJza2VaZgJof25yVlxgCVtobXZOcnVvenh2VU1VeGZkdmIJBVR2dU18anh9dXNqH3ZgYntjYVVudX1aa1plZl9bbmd4RV9aVkBmb1t6T2VKcXp/VHJ1dH9We3ZjY3RRbXVodntmekl6bHBkYHp7ZmNqYmtHYmoGYHxmb25yYl53dWxycGFvCXFTeQZNUGVFZXdmXmBSfgVoeGBOcmV9Ymt2c3Z9e2Z0YHZsBkJsbwl6ZX0GCH1hH3kPYwB3dG5yWkp/bA16fWJaUGNCbXJlehd1bAVKd20IBWF2ZU18anh9dXNqH3ZgYntjYVVudX1aa1plZl9bbmd4RV9aVkBmb1t6T2VKcXp/VHJ1dH9We3ZjY3RRbXVodntmekl6bHBkYHp7ZmNqYmtHYmoGYHxmb25yYl53dG9yWnZgUkdrenJdbGpsRwtjX3xxfQUDY2YIBVxMcnMNZmltdldqWlBjcUJvdlcAa2pAWnd6f3lfdXNrc31Dd2N2Tnl2eVBoc3NZAmBiVW9zeHJdbHRRXGRqcWBwenhhXXMDaHVuYQt8Zn8FZXphCFBjbEdrbGdoVm5xRmB/UnJle3VNYGpFU3JzAmhVb2FCdmZ8UHp6YQhQY2ZxYGxkG3tsB0Z4b3x2YX1PUXZlQm13ZgNafX5hQm9iCUxQdnBNVmFGXHBwc3tueENzaXVBR3BoYl1sZh5hWGxwVmdoBANpYAlMcXphCFBja2ZsZgNKVmgFcE9hbwRQfVx7YGVFZWhhSlZ5b1hCTWEJW2h2cnNgY0V9YGV0WnVuYV1PdnsAcGp2eHJ1bwN3dkobdmxfdG9iCEB2e2VJUmEeZXJiZHh7fgVCTG8IAWt9W391cx5ld2MCbHt+BWh4YE56ZntxeFFUeXkNbXVoWVpzaBpRbgUASmBNXFdAX0tQanhFWHxkGVNuWBlKBHdAVGZ9Vm1qSg5gTGRMYglXa3lya3dqfF9YYmdkRW0EXkpmfEB3S3J4UWN4bWhzA2BSawRdTGVSDXZ2cmNzYXt5TmNcaHVuYQN8ZFVEW31iXWxjQkMOYmRWdmxfQXd2bEN3b3ZvbHVvbV12VXdRbAVGdmJofmt5cQlQZh5Tc2ZKH3lvWUpmYQhMW31iXWxjQkMOYmRWdmxfQXduYUQATGBdA1N5XwhRAWhBXAZoQG1Ucl5Lf3cHUXYCCm16Sg9cbFoaVWEFWEpaYwdReXJ3c3RodGFYSk9lXnpneFxjdWEfQ2BldEp8fl9CfGJvTGF6ZVFQZh8DbmJnZFJrdmhLYghYZHhbSWxraF93bGR8f29yWm9gCXJoSmFvdmVDYVpmZ0p7a3EDSWAJemFoYU1gYFZfYGZ3ZFN+cgN8Yn9AY09bf3NlQgJhYQJrdGBYcEBlUg12dnVJVmp2W2BlXlp9b18DSm9sdgJ4W1FRZh5TcmFkd3Z7Uw8TFRUWAx0ZDAMQFBV4dXNvdnh1c292eHVzb3Z4dXNvdnh1c28VFRYBGhkPABAUBxcCAxgHCgQAHwIPDAEWBwsHHgwCCRoHHxUDFn16fkx5dk0KGxgQGQcXBgsMDRt6ZklCdmBRExUVFgYaGQ0HEBQVbUxweXFoeXdmUEF7dVkDcXNzZ299dX95dV4AcW91Xnt2b1R9fHBdVWgFRmVmCHFlbXUJfHRZfnpzc01ne2VBSnZeBHVsX2B8dmxQaGZeZGZsBUJPcWh5dW1QeHd0fG1+ZWdNVX1TUXd2UVd/b09gc3ZJfnhzcxp6e0N/TXBBQHV5B1VSZnxucHxjTW94U3t3dU4MdG1PCXd2f25+clVNen52cGNgfAVzdmJ0YnNZcnVzWkV8e2VdSHZ4DHVsdVp3cUlieHVaeGFsYmBPcWh5d3pABAkQAhUNARwcARsOEHZyY1VgHlh2fGNoYmxiZGlkbFB8aVt/dmVGbnVsAmhjYXJVfmRsUHxpZmN2d3sGa2wDG25qBWRvYUFEU3Zxd1VxRQZ+ZgJdChsYEBkBFwcBDA0bdXNrChsYEBkDFwwFDA0bUGpWR1wHRXtwVHZaTX97A1BWRGNRX3xZWgdwH3JUWGFIBk11Znh+Y25fdAV/Q11rdWsAakxhCWdrQm1zZXQfUn8HB0VtTgBUbWJOd3pJVHN2VXRBe0xze1NSUHR2cV1zenZ6cHZgZG5rc2tsVWsAan1iXUB1HFBzc3BkRnt8Amh2TlcPDEo=");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);

        // Set cookies
        _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
    }

    public async Task<string> SendMessageAsync(ChatRequest request)
    {
        var json = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("backend-api/conversation", content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async IAsyncEnumerable<string> SendMessageAsStreamAsync(ChatRequest request)
    {
        var json = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Send request and get response headers early
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "backend-api/conversation")
        {
            Content = content
        }, HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        var buffer = new char[4096];
        while (true)
        {
            var read = await reader.ReadAsync(buffer, 0, buffer.Length);
            if (read == 0) break;

            var chunk = new string(buffer, 0, read);
            yield return chunk;
        }
    }
}

public class ChatRequest
{
    public string action { get; set; } = "next";
    public List<Message> messages { get; set; } = new();
    public string parent_message_id { get; set; } = "client-created-root";
    public string model { get; set; } = "gpt-4o";
    public int timezone_offset_min { get; set; } = -60;
    public string timezone { get; set; } = "Europe/Lisbon";
    public bool history_and_training_disabled { get; set; } = true;
    public ConversationMode conversation_mode { get; set; } = new();
    public bool enable_message_followups { get; set; } = true;
    public List<object> system_hints { get; set; } = new();
    public bool supports_buffering { get; set; } = true;
    public List<string> supported_encodings { get; set; } = new() { "v1" };
    public ClientContextualInfo client_contextual_info { get; set; } = new();
    public string paragen_cot_summary_display_override { get; set; } = "allow";
}

public class Message
{
    public string id { get; set; }
    public Author author { get; set; }
    public double create_time { get; set; }
    public Content content { get; set; }
    public Metadata metadata { get; set; }
}

public class Author
{
    public string role { get; set; }
}

public class Content
{
    public string content_type { get; set; }
    public List<string> parts { get; set; }
}

public class Metadata
{
    public List<object> selected_sources { get; set; }
    public List<object> selected_github_repos { get; set; }
    public bool selected_all_github_repos { get; set; }
    public SerializationMetadata serialization_metadata { get; set; }
}

public class SerializationMetadata
{
    public List<object> custom_symbol_offsets { get; set; }
}

public class ConversationMode
{
    public string kind { get; set; } = "primary_assistant";
}

public class ClientContextualInfo
{
    public bool is_dark_mode { get; set; }
    public int time_since_loaded { get; set; }
    public int page_height { get; set; }
    public int page_width { get; set; }
    public double pixel_ratio { get; set; }
    public int screen_height { get; set; }
    public int screen_width { get; set; }
}