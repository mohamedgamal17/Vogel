﻿using AutoMapper;
using MassTransit;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Application.Messages.Events;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;

namespace Vogel.Messanger.Application.Messages.Consumers
{
    public class LogMessagesActivitiesEventConsumer : IConsumer<LogMessagesActivitiesEvent>
    {
        private readonly IMongoRepository<MessageLogMongoEntity> _messageActivityMongoRepository;
        private readonly IMapper _mapper;
        public LogMessagesActivitiesEventConsumer(IMongoRepository<MessageLogMongoEntity> messageActivityMongoRepository, IMapper mapper)
        {
            _messageActivityMongoRepository = messageActivityMongoRepository;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<LogMessagesActivitiesEvent> context)
        {
            var activites = _mapper.Map<List<MessageActivity>, List<MessageLogMongoEntity>>(context.Message.Activities);

            await _messageActivityMongoRepository.ReplaceOrInsertManyAsync(activites);
        }
    }
}