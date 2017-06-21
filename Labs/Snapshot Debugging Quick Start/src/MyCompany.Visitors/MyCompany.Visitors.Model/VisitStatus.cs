namespace MyCompany.Visitors.Model
{
    /// <summary>
    /// Visit Status
    /// </summary>
    public enum VisitStatus
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Visitor has not arrived yet
        /// </summary>
        Pending = 1,
        /// <summary>
        /// Visitor has arrived
        /// </summary>
        Arrived = 2,
    }
}
