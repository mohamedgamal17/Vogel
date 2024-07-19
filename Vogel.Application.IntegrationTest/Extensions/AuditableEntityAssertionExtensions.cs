using FluentAssertions;
using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class AuditableEntityAssertionExtensions
    {
        public static void AssertAuditingProperties(this IAuditedEntity left , IMongoAuditing right)
        {
            left.CreatorId.Should().Be(right.CreatorId);
            left.CreationTime.Should().BeCloseTo(right.CreationTime, TimeSpan.FromMilliseconds(1));
            left.ModificationTime.Should().BeCloseTo(right.ModificationTime, TimeSpan.FromMilliseconds(1));
            left.ModifierId.Should().Be(right.ModifierId);
            left.DeletorId.Should().Be(right.DeletorId);
            if(left.DeletionTime != null)
            {
                left.DeletionTime.Should().BeCloseTo(right.DeletionTime!.Value, TimeSpan.FromMilliseconds(1));
            }
            
        }

        public static void AssertAuditingProperties(this IMongoAuditing left, IAuditedEntity right)
        {
            left.CreatorId.Should().Be(right.CreatorId);
            left.CreationTime.Should().BeCloseTo(right.CreationTime, TimeSpan.FromMilliseconds(1));
            left.ModificationTime.Should().BeCloseTo(right.ModificationTime, TimeSpan.FromMilliseconds(1));
            left.ModifierId.Should().Be(right.ModifierId);
            left.DeletorId.Should().Be(right.DeletorId);
            if (left.DeletionTime != null)
            {
                left.DeletionTime.Should().BeCloseTo(right.DeletionTime!.Value, TimeSpan.FromMilliseconds(1));
            }

        }

    }
}
