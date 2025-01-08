using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data
{
    [Table("StarBaseApiCallLog")]
    public class StarbaseApiCallLog
    {
        public int Id { get; set; }
        public string? ApiEndpoint { get; set; }
        public bool SuccessStatus { get; set; }
        public DateTime CallDate { get; set; }
        public string? ChangedField { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? ErrorLog { get; set; }
    }

    public class StarBaseApiCallLogConfiguration : IEntityTypeConfiguration<StarbaseApiCallLog>
    {
        public void Configure(EntityTypeBuilder<StarbaseApiCallLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
