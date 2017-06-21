
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCompany.Visitors.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// Visit entity
    /// </summary>
    [DataContract]
    public class Visitor
    {
        /// <summary>
        /// UniqueId
        /// </summary>
        [DataMember]
        public int VisitorId { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        [DataMember(IsRequired = true)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        [DataMember(IsRequired = true)]
        public string LastName { get; set; }

        /// <summary>
        /// PersonalId
        /// </summary>
        [DataMember]
        public string PersonalId { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Company { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Email { get; set; }

        /// <summary>
        /// Created DateTime
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Last Modified DateTime
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime LastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [DataMember]
        public string Position { get; set; }

        /// <summary>
        /// VisitorPictures
        /// </summary>
        [DataMember]
        public ICollection<VisitorPicture> VisitorPictures { get; set; }

        /// <summary>
        /// Gets or sets the last visit.
        /// </summary>
        /// <value>
        /// The last visit.
        /// </value>
        [DataMember]
        public Visit LastVisit { get; set; }

        /// <summary>
        /// Visit
        /// </summary>
        [DataMember]
        public ICollection<Visit> Visits { get; set; }

    }
}
