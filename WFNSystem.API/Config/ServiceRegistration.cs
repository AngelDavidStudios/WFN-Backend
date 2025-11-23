using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services;
using WFNSystem.API.Services.Interfaces;
using WFNSystem.API.Services.Strategies;
using WFNSystem.API.Services.Strategies.Interfaces;

namespace WFNSystem.API.Config;

public static class ServiceRegistration
{
    public static IServiceCollection AddProjectDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add AWS Lambda hosting
        services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

        //Services AWS
        var awsOptions = configuration.GetAWSOptions();
        awsOptions.Profile = "AdminAccess";
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddScoped<IDynamoDBContext, DynamoDBContext>();

        // Repositories
        services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
        services.AddScoped<IPersonaRepository, PersonaRepository>();
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<IBankingRepository, BankingRepository>();
        services.AddScoped<IParametroRepository, ParametroRepository>();
        services.AddScoped<INovedadRepository, NovedadRepository>();
        services.AddScoped<IProvisionRepository, ProvisionRepository>();
        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
        services.AddScoped<INominaRepository, NominaRepository>();
        
        // Strategies
        services.AddScoped<IStrategyFactory, StrategyFactory>();
        
        // Services
        services.AddScoped<IDepartamentoService, DepartamentoService>();
        services.AddScoped<IPersonaService, PersonaService>();
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<IBankingService, BankingService>();
        services.AddScoped<IParametroService, ParametroService>();
        services.AddScoped<INovedadService, NovedadService>();
        services.AddScoped<IProvisionService, ProvisionService>();
        services.AddScoped<IWorkspaceService, WorkspaceService>();
        services.AddScoped<INominaService, NominaService>();

        return services;
    }
}