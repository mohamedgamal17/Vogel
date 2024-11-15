using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.Comments.Commands.CreateComment
{
    [Authorize]
    public class CreateCommentCommand : ICommand<CommentDto>
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string? CommentId { get; set; }
    }

    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        private readonly IContentRepository<Comment> _commentRepository;
        public CreateCommentCommandValidator(IContentRepository<Comment> commentRepository)
        {
            _commentRepository = commentRepository;
            RuleFor(x => x.Content)
                .MaximumLength(CommentTableConsts.ContentLength)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.PostId)
                .MaximumLength(CommentTableConsts.PostIdLength)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.CommentId)
                .MaximumLength(CommentTableConsts.CommentIdLength)
                .NotEmpty()
                .MustAsync(CheckParentCommentExist)
                .WithMessage((_, commentId) => $"Comment with id : ({commentId}) , is not exist")
                .When(x => x.CommentId != null);

        }

        private async Task<bool> CheckParentCommentExist(string commentId , CancellationToken cancellationToken)
        {
            return await _commentRepository.AnyAsync(x => x.Id == commentId);
        }
    }
}
