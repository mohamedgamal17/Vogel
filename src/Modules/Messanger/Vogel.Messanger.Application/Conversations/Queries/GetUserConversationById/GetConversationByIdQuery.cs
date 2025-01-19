﻿using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Messanger.Application.Conversations.Dtos;

namespace Vogel.Messanger.Application.Conversations.Queries.GetUserConversationById
{
    [Authorize]
    public class GetConversationByIdQuery : IQuery<ConversationDto>
    {
        public string ConversationId { get; set; }
    }
}
