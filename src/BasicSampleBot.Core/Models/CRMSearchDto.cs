namespace BasicSampleBot.BusinessCore
{
    using Microsoft.Spatial;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Deprecated, use specific index types in stead
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

    public class CustomerSearchDto
    {
        public string EntityId { get; set; }
        public string Name { get; set; }
        // As in Mr., Ms. etc.
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string SalesPerson { get; set; }
        public string[] Tags { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public GeographyPoint Location { get; set; }
    }

    public class ProductSearchDto
    {
        public string EntityId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ListPrice { get; set; }
    }
}
