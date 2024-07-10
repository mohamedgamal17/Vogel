﻿using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.Application.Posts.Commands;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Domain.Medias;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Posts;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Posts
{
    public class PostCommandHandler
    {

        public PostMongoRepository PostMongoRepository { get; set; }

        public PostCommandHandler()
        {
            PostMongoRepository = Testing.ServiceProvider.GetRequiredService<PostMongoRepository>();
        }

        [Test]
        public async Task Should_create_post()
        {
            await RunAsUserAsync();

            var fakeMedia = await CreateMediaAsync();

            var command = new CreatePostCommand
            {
                MediaId = fakeMedia.Id,
                Caption = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var post = await FindByIdAsync<Post>(result.Value!.Id);

            post.Should().NotBeNull();

            var postMongoEntity = await PostMongoRepository.FindByIdAsync(post!.Id);

            postMongoEntity.Should().NotBeNull();

            post!.AssertPost(command);

            post.AssertPostMongoEntity(postMongoEntity!);

            result.Value.AssertPostDto(post, CurrentUserProfile, fakeMedia);
        }

        public async Task Should_failure_while_creating_post_when_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var fakeMedia = await CreateMediaAsync();

            var command = new CreatePostCommand
            {
                MediaId = fakeMedia.Id,
                Caption = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_create_post_when_user_dose_not_own_this_media()
        {
            await RunAsUserAsync();

            var fakeMedia = await CreateMediaAsync();

            await RunAsUserAsync();

            var command = new CreatePostCommand
            {
                MediaId = fakeMedia.Id,
                Caption = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_update_post()
        {
            await RunAsUserAsync();

            var fakeMedia = await CreateMediaAsync();

            var fakePost = await CreatePostAsync(fakeMedia);

            var fakeMedia1 = await CreateMediaAsync();

            var command = new UpdatePostCommand
            {
                Id = fakePost.Id,
                Caption = Guid.NewGuid().ToString(),
                MediaId = fakeMedia1.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var post = await FindByIdAsync<Post>(result.Value!.Id);

            post.Should().NotBeNull();

            var postMongoEntity = await PostMongoRepository.FindByIdAsync(post!.Id);

            postMongoEntity.Should().NotBeNull();

            post!.AssertPost(command);

            post.AssertPostMongoEntity(postMongoEntity!);

            result.Value.AssertPostDto(post, CurrentUserProfile, fakeMedia1);

            result.Value.AssertPostDto(post, CurrentUserProfile!, fakeMedia1);
        }

        [Test]
        public async Task Should_failure_while_updaing_post_when_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var fakeMedia = await CreateMediaAsync();

            var fakePost = await CreatePostAsync(fakeMedia);

            var command = new UpdatePostCommand
            {
                Id = fakePost.Id,
                Caption = Guid.NewGuid().ToString(),
                MediaId = fakeMedia.Id
            };


            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_updating_post_when_user_dose_not_own_this_post()
        {
            await RunAsUserAsync();

            var fakeMedia = await CreateMediaAsync();

            var fakePost = await CreatePostAsync(fakeMedia);

            await RunAsUserAsync();

            var fakeMedia1 = await CreateMediaAsync();

            var command = new UpdatePostCommand
            {
                Id = fakePost.Id,
                Caption = Guid.NewGuid().ToString(),
                MediaId = fakeMedia1.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_failure_while_updating_post_when_user_dose_not_own_this_media()
        {

            await RunAsUserAsync();

            var fakeMedia = await CreateMediaAsync();

            await RunAsUserAsync();

            var fakeMedia1 = await CreateMediaAsync();

            var fakePost = await CreatePostAsync(fakeMedia1);

            var command = new UpdatePostCommand
            {
                Id = fakePost.Id,
                Caption = Guid.NewGuid().ToString(),
                MediaId = fakeMedia.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_remove_post()
        {
            await RunAsUserAsync();

            var fakeMedia = await CreateMediaAsync();

            var fakePost = await CreatePostAsync(fakeMedia);

            var command = new RemovePostCommand
            {
                Id = fakePost.Id,
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var post = await FindByIdAsync<Post>(fakePost.Id);

            post.Should().BeNull();

            var postMongoEntity = await PostMongoRepository.FindByIdAsync(command.Id);

            postMongoEntity.Should().BeNull();
        }

        [Test]
        public async Task Should_failure_while_removing_post_when_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var fakeMedia = await CreateMediaAsync();

            var fakePost = await CreatePostAsync(fakeMedia);

            var command = new RemovePostCommand
            {
                Id = fakePost.Id,
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_removing_post_when_user_dose_not_own_this_post()
        {
            await RunAsUserAsync();

            var fakeMedia1 = await CreateMediaAsync();

            var fakePost = await CreatePostAsync(fakeMedia1);

            await RunAsUserAsync();

            var command = new RemovePostCommand
            {
                Id = fakePost.Id,
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        private async Task<Post> CreatePostAsync(Media media)
        {
            var post = new Post
            {
                Caption = Guid.NewGuid().ToString(),
                MediaId = media.Id,
                UserId = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString()

            };

            return await InsertAsync(post);
        }
        private async Task<Media> CreateMediaAsync()
        {
            var media = new Media()
            {
                MediaType = MediaType.Image,
                Size = 56666,
                File = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                UserId = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString()
            };

            return await InsertAsync(media);
        }
    }
}
