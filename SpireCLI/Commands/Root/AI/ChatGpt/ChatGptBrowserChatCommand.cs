using SpireCore.AI.Interactions.Contracts;
using SpireCore.AI.Interactions.Contracts.Attachments;
using SpireCore.AI.Interactions.Implementations;
using SpireCore.AI.Interactions.Implementations.Attachments;
using SpireCore.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpireCLI.Commands.Root.AI.ChatGpt
{
    public class ChatGptBrowserChatCommand : BaseCommand
    {
        public override string Name => "chat-gpt";
        public override string Description => "Interactively chat with ChatGPT via browser backend API.";

        private const string DEFAULT_MODEL = "gpt-4o";

        public override CommandResult Execute(CommandContext context)
        {
            RunStreamingChatAsync(context).GetAwaiter().GetResult();
            return CommandResult.Success();
        }

        private async Task RunStreamingChatAsync(CommandContext context)
        {
            Console.WriteLine("== ChatGPT Browser Chat ==");

            // --- Setup model
            var model = DEFAULT_MODEL;

            // --- Prompt for cookies and token
            Console.Write("Paste full Cookie header: ");
            var cookie = Console.ReadLine();

            Console.Write("Paste Bearer token from Authorization header: ");
            var bearerToken = Console.ReadLine();

            // --- Create client
            var chatGptClient = new ChatGptClient(cookie, bearerToken);

            Console.WriteLine("\nType your messages below. Type /exit to quit.\n");

            var conversationId = "client-created-root"; // default root
            var messageHistory = new List<Message>(); // internal list to build payload

            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("You: ");
                Console.ResetColor();
                var userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput)) continue;
                if (userInput.Trim().Equals("/exit", StringComparison.OrdinalIgnoreCase)) break;

                var messageId = Guid.NewGuid().ToString();

                // --- Add user message to local history
                var userMessage = new Message
                {
                    id = messageId,
                    author = new Author { role = "user" },
                    create_time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    content = new Content
                    {
                        content_type = "text",
                        parts = new List<string> { userInput }
                    },
                    metadata = new Metadata
                    {
                        selected_sources = new List<object>(),
                        selected_github_repos = new List<object>(),
                        selected_all_github_repos = false,
                        serialization_metadata = new SerializationMetadata
                        {
                            custom_symbol_offsets = new List<object>()
                        }
                    }
                };

                messageHistory.Add(userMessage);

                try
                {
                    var request = new ChatRequest
                    {
                        action = "next",
                        messages = messageHistory,
                        parent_message_id = conversationId,
                        model = model,
                        timezone_offset_min = -60,
                        timezone = "Europe/Lisbon",
                        history_and_training_disabled = true,
                        conversation_mode = new ConversationMode { kind = "primary_assistant" },
                        enable_message_followups = true,
                        system_hints = new List<object>(),
                        supports_buffering = true,
                        supported_encodings = new List<string> { "v1" },
                        client_contextual_info = new ClientContextualInfo
                        {
                            is_dark_mode = false,
                            time_since_loaded = 84,
                            page_height = 566,
                            page_width = 816,
                            pixel_ratio = 2.0625,
                            screen_height = 800,
                            screen_width = 1280
                        },
                        paragen_cot_summary_display_override = "allow"
                    };

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("AI: ");
                    Console.WriteLine();
                    Console.ResetColor();

                    var fullResponse = new StringBuilder();

                    var responseStream = chatGptClient.SendMessageAsStreamAsync(request);
                    await foreach (var chunk in responseStream)
                    {
                        fullResponse.Append(chunk);
                        RenderChunk(chunk);
                    }

                    Console.WriteLine(); // finish line

                    // --- Append assistant response to message history
                    var assistantMessage = new Message
                    {
                        id = Guid.NewGuid().ToString(),
                        author = new Author { role = "assistant" },
                        create_time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        content = new Content
                        {
                            content_type = "text",
                            parts = new List<string> { fullResponse.ToString() }
                        },
                        metadata = new Metadata
                        {
                            selected_sources = new List<object>(),
                            selected_github_repos = new List<object>(),
                            selected_all_github_repos = false,
                            serialization_metadata = new SerializationMetadata()
                        }
                    };

                    messageHistory.Add(assistantMessage);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[Error: {ex.Message}]\n");
                    Console.ResetColor();
                }
            }
        }

        private static void RenderChunk(string chunk)
        {
            var pattern = new Regex(@"</think>(.*?)</think>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            int idx = 0;

            foreach (Match m in pattern.Matches(chunk))
            {
                if (m.Index > idx)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(chunk.Substring(idx, m.Index - idx));
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(m.Groups[1].Value);
                Console.ResetColor();

                idx = m.Index + m.Length;
            }

            if (idx < chunk.Length)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(chunk.Substring(idx));
                Console.ResetColor();
            }
        }
    }
}