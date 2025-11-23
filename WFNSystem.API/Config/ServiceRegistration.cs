using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository;
using WFNSystem.API.Repository.Interfaces;
using WFNSystem.API.Services;
using WFNSystem.API.Services.Interfaces;

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
        services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
        services.AddScoped<IDynamoDBContext, DynamoDBContext>();

        // Repositories
        services.AddScoped<IBankingService, BankingService>();
        services.AddScoped<IDepartamentoService, DepartamentoService>();
        services.AddScoped<IEmpleadoService, EmpleadoService>();
        services.AddScoped<INovedadService, NovedadService>();
        services.AddScoped<INominaService, NominaService>();
        services.AddScoped<IWorkspaceService, WorkspaceService>();
        services.AddScoped<IParametroService, ParametroService>();
        services.AddScoped<IProvisionService, ProvisionService>();
        services.AddScoped<IPersonaService, PersonaService>();

        return services;
    }
}