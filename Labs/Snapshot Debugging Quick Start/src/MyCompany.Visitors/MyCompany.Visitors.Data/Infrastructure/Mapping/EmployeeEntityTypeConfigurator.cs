namespace MyCompany.Visitors.Data.Infrastructure.Mapping
{
    using MyCompany.Visitors.Model;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    /// <summary>
    /// The entity type configuration for <see cref="MyCompany.Visitors.Model.Employee"/>
    /// </summary>
    class EmployeeEntityTypeConfigurator
        : EntityTypeConfiguration<Employee>
    {
      
        private EmployeeEntityTypeConfigurator()
        {
            this.HasKey(e => e.EmployeeId);

            this.Property(e => e.EmployeeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(e => e.FirstName)
                .IsRequired();

            this.Property(e => e.LastName)
                .IsRequired();

            this.Property(e => e.Email)
                .IsRequired();
            
            this.HasOptional(t => t.Team)
                .WithMany(d => d.Employees)
                .HasForeignKey(t => t.TeamId)
                .WillCascadeOnDelete(false);
        }
    }
}
