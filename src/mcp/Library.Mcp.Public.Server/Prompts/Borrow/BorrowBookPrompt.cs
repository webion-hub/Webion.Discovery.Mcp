using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace Library.Mcp.Public.Server.Prompts.Borrow;

[McpServerPromptType]
public sealed class BorrowBookPrompt
{
    [McpServerPrompt(
        Name = "borrow_book",
        Title = "Borrow book"
    )]
    [Description("Crea un prompt per prenotare un libro")]
    public static ChatMessage Borrow(string title, string email)
    {
        return new ChatMessage(
            role: ChatRole.User, 
            content: 
                $"""
                 Prenotami il libro "{title}" con l'email "{email}"
                 """
        );
    }
}