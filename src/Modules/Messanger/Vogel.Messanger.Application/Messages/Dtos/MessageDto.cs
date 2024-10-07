﻿using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Messanger.Application.Messages.Dtos
{
    public class MessageDto : EntityDto<string>
    {
        public string Content { get; set; }
        public string SenderId { get; set; }
        public UserDto Sender { get; set; }
        public string ReciverId { get; set; }
        public UserDto Reciver { get; set; }
        public bool IsSeen { get; set; }
    }
}