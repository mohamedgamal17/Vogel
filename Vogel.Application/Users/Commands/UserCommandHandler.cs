using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Users.Dtos;
using Vogel.Domain;
using Vogel.Domain.Utils;

namespace Vogel.Application.Users.Commands
{
    public class UserCommandHandler : 
        IApplicationRequestHandler<CreateUserCommand, UserDto>,
        IApplicationRequestHandler<UpdateUserCommand , UserDto>
    {
        private readonly IMongoDbRepository<User> _mongoDbRepository;

        public UserCommandHandler(IMongoDbRepository<User> mongoDbRepository)
        {
            _mongoDbRepository = mongoDbRepository;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User();

            PrepareUserEntity(request, user);

            user = await _mongoDbRepository.InsertAsync(user);

            return MapUserToUserDto(user);
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _mongoDbRepository.FindByIdAsync(request.Id);

            if(user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(User), request.Id));
            }

            PrepareUserEntity(request, user);

            await _mongoDbRepository.UpdateAsync(user);

            return MapUserToUserDto(user);
        }

        private void PrepareUserEntity(UserCommandBase command , User user)
        {
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.BirthDate = command.BirthDate;
            user.Gender = command.Gender;
            user.MediaId = command.MediaId;
        }


        private UserDto MapUserToUserDto(User user)
        {
            var dto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate
            };

            return dto;
        }

    
    }
}
