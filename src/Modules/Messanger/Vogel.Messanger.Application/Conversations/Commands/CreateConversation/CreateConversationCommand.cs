using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Users;

namespace Vogel.Messanger.Application.Conversations.Commands.CreateConversation
{
    [Authorize]
    public class CreateConversationCommand : ICommand<ConversationDto>
    {
        public string? Name { get; set; }
        public List<string> Participants { get; set; }
    }

    public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
    {
        private readonly IMongoRepository<UserMongoEntity> _userMongoRepository;
        public CreateConversationCommandValidator(IMongoRepository<UserMongoEntity> userMongoRepository)
        {
            _userMongoRepository = userMongoRepository;


            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(ConversationTableConst.Name)
                .When(x => x.Name != null);


            RuleFor(x => x.Participants)
                .Must(x => x.Count == 1)
                .WithMessage("Participants cannot be empty or more than 1")
                .When(x => x.Name == null);


            RuleFor(x => x.Participants)
                .Must(x => x.Count > 0)
                .WithMessage("Participants cannot be empty")
                .When(x => x.Name != null);

            RuleForEach(x => x.Participants)
                .MustAsync(CheckParticipantExist)
                .WithMessage((_, participantId) => $"Participant with id : ({participantId}) , is not exist")
                .When(x => x.Participants.Count > 0);


                
        }

        private async Task<bool> CheckParticipantExist(string participantId , CancellationToken cancellationToken = default)
        {
            return await _userMongoRepository.AnyAsync(x => x.Id == participantId);
        }
    }
}
