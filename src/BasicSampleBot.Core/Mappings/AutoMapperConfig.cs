namespace BasicSampleBot.BusinessCore
{
    using AutoMapper;
    public class AutomapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(config: cfg =>
            {
                cfg.AddProfile<CRMEntityMappings>();
            });
        }
    }

    class CRMEntityMappings : Profile
    {
        public CRMEntityMappings()
        {
			// CRM entity to person
			// Person to person dto
            CreateMap<CRMSearchEntity, CRMSearchDto>();
			// CRM entity to company
			// Company to company dto

			// Todo to create to todo entity
			// Todo entity to todo dto
        }

        public override string ProfileName
        {
            get { return "CRMEntityMappings"; }
        }
    }
}