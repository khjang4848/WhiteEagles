namespace WhiteEagles.Data.Mapping
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public static class AutoMapperConfiguration
    {
        public static void ConfigureAutoMapper(this IServiceCollection service, 
            Type type)
        {
            service.AddAutoMapper(x => 
                x.AddProfile<ViewModelToDomainMappingProfile>(), type);
            service.AddAutoMapper(x => 
                x.AddProfile<DomainModelToViewModelMappingProfile>(), type);
        }
    }
}
