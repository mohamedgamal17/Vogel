using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.Social.Presentation.Endpoints.People
{
    public class PeopleRoutingGroup : Group
    {
        public PeopleRoutingGroup()
        {
            Configure("people", ep =>
            {
                ep.Description(x => x.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
                    .WithSummary("people routes")
                    .WithTags("people")               
                );
                
            });
        }
    }
}
