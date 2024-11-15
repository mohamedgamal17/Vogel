using System.Reflection;

namespace Vogel.BuildingBlocks.Infrastructure.Endpoints
{
    public class EndpointAssemblyRegistery
    {

        public HashSet<Assembly> Assemblies { get; set; }

        public EndpointAssemblyRegistery()
        {
            Assemblies = new();
        }

        public EndpointAssemblyRegistery RegisterAssembelies(params Assembly[] asssemblies)
        {
            if(asssemblies != null && asssemblies.Count() > 0)
            {
                foreach (var assmbely in asssemblies)
                {
                    Assemblies.Add(assmbely);
                }
            }

            return this;
        }  
    }
}
