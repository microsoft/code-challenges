namespace MyCompany.Visitors.Data.Infrastructure.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    using MyCompany.Visitors.Model;

    /// <summary>
    /// The entity type configuration
    /// </summary>
    class VisitorEntityTypeConfigurator
        : EntityTypeConfiguration<Visitor>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public VisitorEntityTypeConfigurator()
        {
            this.HasKey(v => v.VisitorId);

            this.Ignore(v => v.LastVisit);

            this.Property(v => v.FirstName)
                .IsRequired();

            this.Property(v => v.LastName)
                .IsRequired();

            this.Property(v => v.Company)
                .IsRequired();

            this.Property(v => v.Email)
                .IsRequired();

            this.Property(v => v.CreatedDateTime)
                .IsRequired();

            this.Property(v => v.LastModifiedDateTime)
                .IsRequired();

        }
    }
}
