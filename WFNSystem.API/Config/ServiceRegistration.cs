using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using WFNSystem.API.Models;
using WFNSystem.API.Repository;
using WFNSystem.API.Repository.Interfaces;

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
        services.AddScoped<IRepository<Persona>, PersonaRepository>();
        services.AddScoped<IRepository<Empleado>, EmpleadoRepository>();
        services.AddScoped<IRepository<Banking>, BankingRepository>();
        services.AddScoped<IRepository<Departamento>, DepartamentoRepository>();
        services.AddScoped<IRepository<Egresos>, EgresosRepository>();
        services.AddScoped<IRepository<Ingresos>, IngresosRepository>();
        services.AddScoped<IRepository<Nomina>, NominaRepository>();
        services.AddScoped<IRepository<Novedad>, NovedadRepository>();
        services.AddScoped<IRepository<Parametro>, ParametroRepository>();
        services.AddScoped<IRepository<Provision>, ProvisionRepository>();
        services.AddScoped<IRepository<Workspace>, WorkspaceRepository>();
        
        services.AddScoped<IDireccionRepository, DireccionRepository>();

        return services;
    }
}