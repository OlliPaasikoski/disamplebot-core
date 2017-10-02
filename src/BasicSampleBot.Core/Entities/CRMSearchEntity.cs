namespace BasicSampleBot.BusinessCore
{
    using System;
    using Microsoft.Azure.Search;
    using Microsoft.Azure.Search.Models;
    using Microsoft.Spatial;
    using System.ComponentModel.DataAnnotations;

    // The SerializePropertyNamesAsCamelCase attribute is defined in the Azure Search .NET SDK.
    // It ensures that Pascal-case property names in the model class are mapped to camel-case
    // field names in the index.

    [SerializePropertyNamesAsCamelCase]
    public abstract class SearchEntity
    {
        [Key]
        [IsFilterable, IsSearchable]
        public string EntityId { get; set; }
    }

    public class ProductEntity : SearchEntity
    {
        [IsSearchable]
        public string Name { get; set; }

        [IsSearchable, IsFilterable, IsSortable]
        public string Category { get; set; }

        [IsSearchable, IsSortable, IsFilterable]
        public string ListPrice { get; set; }
    }

    public class CustomerEntity : SearchEntity // as in demo
    {
        [IsSearchable]
        public string Name { get; set; }

        // As in Mr., Ms. etc.
        public string Title { get; set; }

        [IsSearchable, IsFilterable, IsSortable]
        public string CompanyName { get; set; }

        // Check if this is returned without [IsSearchable]
        [IsSearchable]
        public string Phone { get; set; }

        [IsSearchable, IsFilterable]
        public string SalesPerson { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        public string[] Tags { get; set; }

        [IsFilterable, IsSortable, IsFacetable]
        public DateTimeOffset? LastModified { get; set; }

        [IsFilterable, IsSortable]
        public GeographyPoint Location { get; set; }
    }

    public class CRMEntity : SearchEntity // as in demo
    {
        [IsSearchable]
        public string Name { get; set; }

        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Category { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        public string[] Tags { get; set; }

        [IsSearchable]
        public string Description { get; set; }

        [IsFilterable, IsSortable, IsFacetable]
        public DateTimeOffset? DateCreated { get; set; }

        [IsFilterable, IsSortable, IsFacetable]
        public DateTimeOffset? LastModified { get; set; }

        [IsFilterable, IsSortable]
        public GeographyPoint Location { get; set; }
    }
    
}
