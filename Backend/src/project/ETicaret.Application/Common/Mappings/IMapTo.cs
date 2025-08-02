using AutoMapper;

namespace ETicaret.Application.Common.Mappings;

/// <summary>
/// DTO'dan Entity'ye mapping için (current type -> T).
/// </summary>
public interface IMapTo<T>
{
    void Mapping(Profile profile) => profile.CreateMap(GetType(), typeof(T));
}