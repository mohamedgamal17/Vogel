using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Vogel.Messanger.Application.Messages.Dtos;

namespace Vogel.Messanger.Presentation.Hubs.Chat
{
    [Route("chat")]
    public class Chathub : Hub<IChatHub>
    {
    }

    public interface IChatHub
    {
        Task ReciveMessage(MessageDto message);
    }
}
