
namespace MyCompany.Visitors.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// Visitor Picture Entity
    /// </summary>
    [DataContract]
    public class VisitorPicture
    {
        /// <summary>
        /// UniqueId
        /// </summary>
        [Key]
        [DataMember]
        public int VisitorPictureId { get; set; }

        /// <summary>
        /// Picture Type
        /// </summary>
        [DataMember]
        public PictureType PictureType { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        [DataMember]
        public byte[] Content { get; set; }

        /// <summary>
        /// VisitorId
        /// </summary>
        [DataMember]
        public int VisitorId { get; set; }
        /*
        /// <summary>
        /// Visitor
        /// </summary>
        [DataMember]
        public Visitor Visitor { get; set; }*/
    }
}
