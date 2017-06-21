namespace MyCompany.Visitors.Data.Infrastructure.Mapping
{
    using MyCompany.Visitors.Model;
    using System.Data.Entity.ModelConfiguration;

    /// <summary>
    /// Visit entity type configuration
    /// </summary>
    class VisitEntityTypeConfiguration
        :EntityTypeConfiguration<Visit>
    {
        private VisitEntityTypeConfiguration()
        {
            this.HasKey(v => v.VisitId);

            this.Property(v => v.CreatedDateTime)
                .IsRequired();

            this.Property(v => v.VisitDateTime)
                .IsRequired();

            this.Property(v => v.HasCar)
                .IsRequired();

            this.Property(v => v.Status)
                .IsRequired();

            this.HasRequired(v => v.Employee)
                .WithMany(e => e.Visits)
                .HasForeignKey(v => v.EmployeeId);
        }
    }
}
