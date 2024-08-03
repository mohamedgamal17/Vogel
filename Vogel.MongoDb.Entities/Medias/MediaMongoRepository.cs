﻿using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Medias
{
    public class MediaMongoRepository : MongoRepository<MediaMongoEntity>
    {
        public MediaMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {


        }
    }
}
