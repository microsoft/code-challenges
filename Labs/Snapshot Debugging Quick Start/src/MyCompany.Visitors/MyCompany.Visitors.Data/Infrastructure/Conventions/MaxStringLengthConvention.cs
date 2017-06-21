

namespace MyCompany.Visitors.Data.Infrastructure.Conventions
{
    using System;
    using System.Data.Entity.ModelConfiguration.Conventions;

    /// <summary>
    /// Max string length convention
    /// </summary>
    class MaxStringLengthConvention
        :Convention
    {
        /// <summary>
        /// Max string length convention constructor
        /// </summary>
        public MaxStringLengthConvention()
        {
            this.Properties<String>()
                .Configure(c => c.HasMaxLength(255));
        }
    }
}
