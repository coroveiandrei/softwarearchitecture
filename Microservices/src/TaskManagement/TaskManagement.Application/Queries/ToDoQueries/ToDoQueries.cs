using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.QueryProjections;
using System;
using System.Collections.Generic;

namespace TaskManagement.Application.Queries.ActivityQueries
{
    public class GetAllToDoQuery : BaseRequest<GetAllToDoQueryResponse>
    {
    }

    public class GetAllToDoQueryResponse
    {
        public List<ToDoProjection> ToDoModels { get; set; }
    }

    public class GetToDoByIdQuery : BaseRequest<GetToDoByIdQueryResponse>
    {
        public Guid ToDoId { get; set; }
    }

    public class GetToDoByIdQueryResponse
    {
        public ToDoProjection ToDo { get; set; }
    }
}
