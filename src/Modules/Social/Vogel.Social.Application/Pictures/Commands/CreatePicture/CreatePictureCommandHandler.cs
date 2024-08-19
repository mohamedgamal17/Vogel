using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Pictures.Factories;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Pictures.Commands.CreatePicture
{
    public class CreatePictureCommandHandler : IApplicationRequestHandler<CreatePictureCommand, PictureDto>
    {
        private readonly ISocialRepository<Picture> _pictureRepository;
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        private readonly ISecurityContext _securityContext;
        private readonly IPictureResponseFactory _pictureResponseFactory;
        public CreatePictureCommandHandler(ISocialRepository<Picture> pictureRepository, IS3ObjectStorageService s3ObjectStorageService, ISecurityContext securityContext, IPictureResponseFactory pictureResponseFactory)
        {
            _pictureRepository = pictureRepository;
            _s3ObjectStorageService = s3ObjectStorageService;
            _securityContext = securityContext;
            _pictureResponseFactory = pictureResponseFactory;
        }

        public async Task<Result<PictureDto>> Handle(CreatePictureCommand request, CancellationToken cancellationToken)
        {

            var objectSaveModel = new S3ObjectStorageSaveModel
            {
                FileName = request.Name,
                Content = request.Content,
                ContentType = request.MimeType,
            };

            var objectResponse = await _s3ObjectStorageService.SaveObjectAsync(objectSaveModel);

            var picture = new Picture
            {
                File = objectResponse.ObjectName,
                UserId = _securityContext.User!.Id
            };

            await _pictureRepository.InsertAsync(picture);


            return await _pictureResponseFactory.PreparePictureDto(picture);

        }
    }
}
