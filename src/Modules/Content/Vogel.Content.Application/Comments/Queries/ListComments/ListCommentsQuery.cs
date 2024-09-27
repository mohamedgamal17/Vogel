﻿using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.Comments.Dtos;
namespace Vogel.Content.Application.Comments.Queries.ListComments
{
    public class ListCommentsQuery : PagingParams , IQuery<Paging<CommentDto>>
    {
        public string PostId { get; set; }
    }
}
