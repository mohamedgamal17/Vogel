using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Domain.Messages;
namespace Vogel.Messanger.Application.Messages.Commands.SendMessage
{
    [Authorize]
    public class SendMessageCommand : ICommand<MessageDto>
    {
        public string ConversationId { get; set; }
        public string Content { get; set; }

    }

    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty()
                .NotNull()
                .MaximumLength(MessageTableConst.ConversationIdLength);

            RuleFor(x => x.Content)
                .NotEmpty()
                .NotNull()
                .MaximumLength(MessageTableConst.ContentLength);
        }
    }
}
