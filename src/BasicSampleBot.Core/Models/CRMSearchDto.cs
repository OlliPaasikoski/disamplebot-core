namespace BasicSampleBot.BusinessCore
{
    using Microsoft.Spatial;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// The DATA TRANSFER OBJECT knows the correct search parameters
    /// </summary>
    public class CRMSearchDto
    {
        public string EntityId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public GeographyPoint Location { get; set; }
    }
}
