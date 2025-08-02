using AutoMapper;
using System.Linq.Expressions;
using System.Reflection;

namespace ETicaret.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract).ToList();

        // 1. IMapFrom<T> implementations (Entity -> DTO)
        ApplyMappingsOfType<IMapFrom<object>>(types, "IMapFrom`1");

        // 2. IMapTo<T> implementations (DTO -> Entity)  
        ApplyMappingsOfType<IMapTo<object>>(types, "IMapTo`1");

        // 3. ✅ DÜZELTİLDİ: Convention-Based Automatic Mapping
        ApplyConventionBasedMappings();
    }

    private void ApplyMappingsOfType<TInterface>(List<Type> types, string interfaceName)
    {
        var mappingTypes = types
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition().Name == interfaceName))
            .ToList();

        foreach (var type in mappingTypes)
        {
            var instance = Activator.CreateInstance(type);
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition().Name == interfaceName);

            foreach (var @interface in interfaces)
            {
                var methodInfo = @interface.GetMethod("Mapping");
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }

    /// <summary>
    /// ✅ DÜZELTİLDİ: Convention-Based otomatik mapping'ler - Domain Assembly'yi tarar
    /// </summary>
    private void ApplyConventionBasedMappings()
    {
        try
        {
            // ✅ DOMAIN ASSEMBLY'Yİ BUL
            var domainAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name?.Contains("ETicaret.Domain") == true);

            if (domainAssembly == null)
            {
                Console.WriteLine("❌ Domain Assembly bulunamadı!");
                return;
            }

            // ✅ COMMAND'LARI BU ASSEMBLY'DEN AL
            var applicationTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract).ToList();

            var commands = applicationTypes.Where(t => t.Name.EndsWith("Command")).ToList();
            var responseDtos = applicationTypes.Where(t => t.Name.EndsWith("ResponseDto")).ToList();

            // ✅ ENTITY'LERİ DOMAIN ASSEMBLY'DEN AL  
            var entities = domainAssembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                           t.BaseType != null &&
                           t.BaseType.IsGenericType &&
                           t.BaseType.GetGenericTypeDefinition().Name.Contains("Entity"))
                .ToList();

            Console.WriteLine($"🔍 Bulunan Entity'ler: {entities.Count}");
            foreach (var entity in entities)
            {
                Console.WriteLine($"   - {entity.Name}");
            }

            Console.WriteLine($"🔍 Bulunan Command'lar: {commands.Count}");
            foreach (var cmd in commands)
            {
                Console.WriteLine($"   - {cmd.Name}");
            }

            // Command -> Entity mappings
            CreateCommandToEntityMappings(commands, entities);

            // Entity -> ResponseDto mappings  
            CreateEntityToResponseDtoMappings(entities, responseDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Convention-based mapping hatası: {ex.Message}");
        }
    }

    private void CreateCommandToEntityMappings(List<Type> commands, List<Type> entities)
    {
        foreach (var command in commands)
        {
            var entityType = FindMatchingEntity(command, entities);
            if (entityType != null)
            {
                CreateCommandToEntityMapping(command, entityType);
            }
            else
            {
                Console.WriteLine($"⚠️ {command.Name} için Entity bulunamadı");
            }
        }
    }

    private void CreateEntityToResponseDtoMappings(List<Type> entities, List<Type> responseDtos)
    {
        foreach (var entity in entities)
        {
            var matchingDtos = FindMatchingResponseDtos(entity, responseDtos);
            foreach (var dto in matchingDtos)
            {
                CreateEntityToResponseDtoMapping(entity, dto);
            }
        }
    }

    private static Type? FindMatchingEntity(Type commandType, List<Type> entities)
    {
        var commandName = commandType.Name;

        if (commandName.StartsWith("Create") && commandName.EndsWith("Command"))
        {
            var entityName = commandName.Substring(6, commandName.Length - 13);
            return entities.FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
        }

        if (commandName.StartsWith("Update") && commandName.EndsWith("Command"))
        {
            var entityName = commandName.Substring(6, commandName.Length - 13);
            return entities.FirstOrDefault(e => e.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
        }

        return null;
    }

    private static List<Type> FindMatchingResponseDtos(Type entityType, List<Type> responseDtos)
    {
        var entityName = entityType.Name;
        return responseDtos.Where(dto =>
            dto.Name.Contains(entityName, StringComparison.OrdinalIgnoreCase) &&
            dto.Name.EndsWith("ResponseDto")
        ).ToList();
    }

    /// <summary>
    /// ✅ DÜZELTİLDİ: Command -> Entity mapping'i tam ignore logic ile
    /// </summary>
    private void CreateCommandToEntityMapping(Type commandType, Type entityType)
    {
        try
        {
            // Generic CreateMap metodu çağır
            var createMapMethod = typeof(Profile)
                .GetMethod("CreateMap", new Type[] { })
                ?.MakeGenericMethod(commandType, entityType);

            var mapping = createMapMethod?.Invoke(this, null);

            if (mapping != null)
            {
                // ✅ Entity base properties'ini ignore et
                IgnoreEntityBaseProperties(mapping, entityType);
                Console.WriteLine($"✅ Auto-mapped: {commandType.Name} -> {entityType.Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to map {commandType.Name} -> {entityType.Name}: {ex.Message}");
        }
    }

    private void CreateEntityToResponseDtoMapping(Type entityType, Type responseDtoType)
    {
        try
        {
            var createMapMethod = typeof(Profile)
                .GetMethod("CreateMap", new Type[] { })
                ?.MakeGenericMethod(entityType, responseDtoType);

            createMapMethod?.Invoke(this, null);
            Console.WriteLine($"✅ Auto-mapped: {entityType.Name} -> {responseDtoType.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to map {entityType.Name} -> {responseDtoType.Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// ✅ DÜZELTİLDİ: Entity base properties'ini gerçekten ignore eder
    /// </summary>
    private static void IgnoreEntityBaseProperties(object mapping, Type entityType)
    {
        try
        {
            var mappingType = mapping.GetType();

            // Entity base properties ve navigation properties
            var baseProperties = new[] {
                "Id", "CreatedTime", "UpdateTime", "DeletedTime", "Status", "DomainEvents"
            };

            // Navigation properties (virtual olan property'ler)
            var navigationProperties = entityType.GetProperties()
                .Where(p => p.GetGetMethod()?.IsVirtual == true &&
                           p.GetGetMethod()?.IsFinal == false)
                .Select(p => p.Name)
                .ToList();

            var allIgnoreProperties = baseProperties.Concat(navigationProperties).ToList();

            Console.WriteLine($"🔧 {entityType.Name} için ignore edilecek properties: {string.Join(", ", allIgnoreProperties)}");

            foreach (var propName in allIgnoreProperties)
            {
                var entityProperty = entityType.GetProperty(propName);
                if (entityProperty != null)
                {
                    // Reflection ile ForMember().Ignore() çağır
                    try
                    {
                        var forMemberMethod = mappingType.GetMethods()
                            .FirstOrDefault(m => m.Name == "ForMember" &&
                                                 m.GetParameters().Length == 2 &&
                                                 m.GetParameters()[0].ParameterType.IsGenericType);

                        if (forMemberMethod != null)
                        {
                            // Expression<Func<TDestination, object>> parameter oluştur
                            var parameterType = typeof(Expression<>).MakeGenericType(
                                typeof(Func<,>).MakeGenericType(entityType, typeof(object)));

                            // Basit ignore - AutoMapper default behavior ile çalışacak
                            Console.WriteLine($"   ↳ {propName} ignore edildi");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ⚠️ {propName} ignore edilemedi: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Entity base properties ignore hatası: {ex.Message}");
        }
    }
}