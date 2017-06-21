namespace MyCompany.Visitors.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Image Crop
    /// </summary>
    public class ImageCrop
    {
        /// <summary>
        /// Gets or sets the big selecton.
        /// </summary>
        public Selection bigSelection { get; set; }

        /// <summary>
        /// Gets or sets the small selecton.
        /// </summary>
        public Selection smallSelection { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double w { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public double h { get; set; }
    }

    /// <summary>
    /// Selection
    /// </summary>
    public class Selection
    {
        /// <summary>
        /// Gets or sets the x1.
        /// </summary>
        public int x1 { get; set; }

        /// <summary>
        /// Gets or sets the x2.
        /// </summary>
        public int x2 { get; set; }

        /// <summary>
        /// Gets or sets the y1.
        /// </summary>
        public int y1 { get; set; }

        /// <summary>
        /// Gets or sets the y2.
        /// </summary>
        public int y2 { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int height { get; set; }
    }
}