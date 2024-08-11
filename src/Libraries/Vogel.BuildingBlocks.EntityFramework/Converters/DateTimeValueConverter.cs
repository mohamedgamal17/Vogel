using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace Vogel.BuildingBlocks.EntityFramework.Converters
{
    public class DateTimeValueConverter : ValueConverter<DateTime, DateTime>
    {
        public DateTimeValueConverter() 
            : base(
                x=> DateTime.SpecifyKind(x, DateTimeKind.Utc),
                x=> DateTime.SpecifyKind(x, DateTimeKind.Utc)
             )
        {

        }
    }

    public class NullableDateTimeValueConverter : ValueConverter<DateTime? , DateTime?>
    {
        public NullableDateTimeValueConverter()
            :base(
                 x=> x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : x,
                 x=> x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : x
               )
        {

        }
    }
}
