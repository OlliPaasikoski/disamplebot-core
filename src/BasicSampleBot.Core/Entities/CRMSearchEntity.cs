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
    public class CRMSearchEntity
    {
        [Key]
        [IsFilterable]
        public string EntityId { get; set; }

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
