using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using MimeMapping;
using Vogel.Social.Application.Pictures.Commands.CreatePicture;
using Vogel.Social.Shared.Dtos;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
namespace Vogel.Social.Presentation.Endpoints.Pictures
{

    public class CreatePictureRequest
    {
        [FastEndpoints.FromBody]
        public IFormFile Image { get; set; }

        public async Task<CreatePictureCommand> ToCreatePictureCommand()
        {
            string extension = Image.FileName.Split(".")[1];

            string fileName = string.Format("{0}_{1}.{2}", Image.FileName.Split(".")[0], DateTime.Now.Ticks, extension);

            MemoryStream stream = new MemoryStream();

            await Image.CopyToAsync(stream);

            stream.Seek(0, SeekOrigin.Begin);

            return new CreatePictureCommand
            {
                Name = fileName,
                Content = stream,
                MimeType = MimeUtility.GetMimeMapping(Image.FileName)
            };
        }
    }

    public class CreatePictureEndpoint : Endpoint<CreatePictureRequest, PictureDto>
    {
        private readonly IMediator _mediator;

        public CreatePictureEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure() 
        {
            Post();
            Group<PictureRoutingGroup>();
            Description(x =>
            {
                x.Produces(StatusCodes.Status201Created, typeof(PictureDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails));
            });
        }
        public override async Task HandleAsync(CreatePictureRequest req, CancellationToken ct)
        {
            var command = await req.ToCreatePictureCommand();

            var result = await _mediator.Send(command);

            var response = result.ToCreatedAtRoute("GetPictureById", new { id = result.Value?.Id });

            await SendResultAsync(response);
        }
    }
}
