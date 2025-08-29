using AutoMapper;
using static Legal.Service.Helper.ApplicationHelper;

namespace Legal.Service.Infrastructure.Helper;

public class MapperHelper
{
    private IMapper? _mapper;
    private readonly MapperConfigurationExpression _configExpression = new();
    private MapperConfiguration? _mapperConfiguration;

    // Dictionary to store MapperHelpers for each module
    public static Dictionary<ModuleName, MapperHelper> MapperHelpers { get; } = new();

    private MapperHelper()
    {
    }

    public static MapperHelper Instance(ModuleName moduleName)
    {
        if (!MapperHelpers.TryGetValue(moduleName, out var helper))
        {
            helper = new MapperHelper();
            MapperHelpers[moduleName] = helper;
        }

        return helper;
    }

    // Add profile to the configuration
    public void AddProfile(Profile profile)
    {
        _configExpression.AddProfile(profile);
    }

    // Cumulative CreateMap method to configure mappings for each module
    public void CreateMap<TDestination, TResult>(Action<IMappingExpression<TDestination, TResult>>? mappingConfig = null)
    {
        var map = _configExpression.CreateMap<TDestination, TResult>();
        mappingConfig?.Invoke(map);
    }

    // Initialize the mapper after all mappings have been configured
    public void InitializeMapper()
    {
        if (_mapper == null)
        {
            _mapperConfiguration = new MapperConfiguration(_configExpression);

            // _mapperConfiguration.AssertConfigurationIsValid(); // Ensures mappings are valid
            _mapper = _mapperConfiguration.CreateMapper();
        }
    }

    // Method to map objects, throws exception if mapper is not initialized
    public TResult Map<TResult>(object domainModel)
    {
        if (_mapper == null)
        {
            throw new InvalidOperationException("Mapper has not been initialized. Call InitializeMapper() after configuring all mappings.");
        }

        return _mapper.Map<TResult>(domainModel);
    }

    // Method to map objects, throws exception if mapper is not initialized
    public List<TResult> MapList<TResult>(object domainModel)
    {
        if (_mapper == null)
        {
            throw new InvalidOperationException("Mapper has not been initialized. Call InitializeMapper() after configuring all mappings.");
        }

        return _mapper.Map<List<TResult>>(domainModel);
    }

    public object Map(Type itemType, object item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_mapper == null)
        {
            throw new InvalidOperationException("Mapper has not been initialized. Call InitializeMapper() after configuring all mappings.");
        }

        return _mapper.Map(item, item.GetType(), itemType);
    }

    public IMapper? GetMapper() => _mapper;
}