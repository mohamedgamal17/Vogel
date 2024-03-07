using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Vogel.Host.Controllers
{
    public abstract class VogelController : Controller
    {
        public IServiceProvider ServiceProvider { get;  }
        public IMediator Mediator { get;  }

        public VogelController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Mediator = ServiceProvider.GetRequiredService<IMediator>();
        }

    }
}
