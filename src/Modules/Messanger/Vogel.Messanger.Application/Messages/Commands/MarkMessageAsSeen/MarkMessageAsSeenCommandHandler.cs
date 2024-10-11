using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Application.Messages.Factories;
using Vogel.Messanger.Application.Messages.Policies;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;
namespace Vogel.Messanger.Application.Messages.Commands.MarkMessageAsSeen
{
    public class MarkMessageAsSeenCommandHandler : IApplicationRequestHandler<MarkMessageAsSeenCommand, MessageDto>
    {
        private readonly IMessangerRepository<Message> _messageRepository;
        private readonly IMongoRepository<MessageMongoEntity> _messageMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IMessageResponseFactory _messageResponseFactory;

        public MarkMessageAsSeenCommandHandler(IMessangerRepository<Message> messageRepository, IMongoRepository<MessageMongoEntity> messageMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IMessageResponseFactory messageResponseFactory)
        {
            _messageRepository = messageRepository;
            _messageMongoRepository = messageMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _messageResponseFactory = messageResponseFactory;
        }

        public async Task<Result<MessageDto>> Handle(MarkMessageAsSeenCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.FindByIdAsync(request.MessageId);

            if(message == null)
            {
                return new Result<MessageDto>(new EntityNotFoundException(typeof(Message), request.MessageId));
            }

            var auhorizationResult = await _applicationAuthorizationService.AuthorizeAsync(message, MessageOperationalRequirments.IsMessageReciver);

            if (auhorizationResult.IsFailure)
            {
                return new Result<MessageDto>(auhorizationResult.Exception!);
            }

            if (message.IsSeen)
            {
                return new Result<MessageDto>(new BusinessLogicException($"Message with id ({message.Id}) is already marked as seen"));
            }

            message.MarkAsSeen();


            await _messageRepository.UpdateAsync(message);

            var mongoEntity = await _messageMongoRepository.FindByIdAsync(message.Id);

            return await _messageResponseFactory.PrepareMessageDto(mongoEntity!);
        }
    }
}
