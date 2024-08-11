using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vogel.BuildingBlocks.MongoDb.Extensions
{
    public static class MongoCollectionHelper
    {
        private static ConcurrentDictionary<Type, string> _resolvedTypes = new ConcurrentDictionary<Type, string>();

        public static string ResolveCollectionName(Type entityType)
        {
            return _resolvedTypes.GetOrAdd(entityType , _ =>
            {
                var collectionAttribute = entityType.GetCustomAttribute<MongoCollectionAttribute>();

                if (collectionAttribute != null)
                {
                    return collectionAttribute.Name;
                }

                return entityType.Name;
            });
        }
    }
}
