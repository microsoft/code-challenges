namespace MyCompany.Visitors.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// Visit entity
    /// </summary>
    [DataContract]
    public class Visit
    {
        /// <summary>
        /// the unique identifier for this entity
        /// </summary>
        [DataMember]
        public int VisitId { get; set; }

        /// <summary>
        /// VisitorId
        /// </summary>
        [DataMember]
        public int VisitorId { get; set; }

        /// <summary>
        /// created DateTime
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Visit DateTime
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime VisitDateTime { get; set; }

        /// <summary>
        /// Has car? 
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool HasCar { get; set; }

        /// <summary>
        /// Plate
        /// </summary>
        [DataMember]
        public string Plate { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        [DataMember]
        public string Comments { get; set; }

        /// <summary>
        /// Visitor
        /// </summary>
        [DataMember]
        public Visitor Visitor { get; set; }

        /// <summary>
        /// EmployeeId
        /// </summary>
        [DataMember]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Employee
        /// </summary>
        [DataMember]
        public Employee Employee { get; set; }

        /// <summary>
        /// Visit Status
        /// </summary>
        [DataMember(IsRequired = true)]
        public VisitStatus Status { get; set; }
        
    }
}
