using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Vogel.BuildingBlocks.Infrastructure.SignalR
{
    public class SignalRRegistery
    {
        public List<Type> HubTypes { get; set; }

        public SignalRRegistery()
        {
            HubTypes = new List<Type>();
        }

        public void RegisterHubFromAssembley(Assembly assembly)
        {
            var hubTypes = assembly.GetTypes().Where(type => typeof(Hub).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var type in hubTypes)
            {
                RegisterHub(type);
            }
        }
        public void RegisterHub<T>() where T : Hub
        {
            RegisterHub(typeof(T));
        }
        public void RegisterHub(Type hubType) 
        {
            if (!hubType.IsAssignableTo(typeof(Hub)))
            {
                throw new InvalidOperationException($"({hubType.AssemblyQualifiedName}) : must inherit ({typeof(Hub).AssemblyQualifiedName}) to be able to be registerd as signalr hub");
            }

            HubTypes.Add(hubType);
        }


    }
}
