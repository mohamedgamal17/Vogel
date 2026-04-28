using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeMapping;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.MediaEngine.Application.Medias.Commands.CreateMedia;
using Vogel.MediaEngine.Domain.Medias;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Presentation.Endpoints.Medias
{
    public class CreateMediaRequest
    {
        public IFormFile File { get; set; }
        public MediaType MediaType { get; set; }

        public async Task<CreateMediaCommand> ToCreateMediaCommand()
        {
            var extension = Path.GetExtension(File.FileName);
            var baseName = Path.GetFileNameWithoutExtension(File.FileName);
            var fileName = $"{baseName}_{DateTime.Now.Ticks}{extension}";

            var stream = new MemoryStream();
            await File.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return new CreateMediaCommand
            {
                Name = fileName,
                Content = stream,
                MimeType = MimeUtility.GetMimeMapping(File.FileName),
                MediaType = MediaType,
            };
        }
    }

    public class CreateMediaEndpoint : Endpoint<CreateMediaRequest, MediaDto>
    {
        private readonly IMediator _mediator;

        public CreateMediaEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("");
            AllowFileUploads();
            Description(x =>
            {
                x.Produces(StatusCodes.Status201Created, typeof(MediaDto))
                 .Produces(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails));
            });
            Group<MediaRoutingGroup>();
        }

        public override async Task HandleAsync(CreateMediaRequest req, CancellationToken ct)
        {
            var command = await req.ToCreateMediaCommand();
            var result = await _mediator.Send(command, ct);
            var response = result.ToCreatedAtRoute("GetMediaById", new { id = result.Value?.Id });
            await SendResultAsync(response);
        }
    }
}
