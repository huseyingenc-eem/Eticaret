using AutoMapper;

namespace ETicaret.Application.Common.Mappings;

/// <summary>
/// Entity'den DTO'ya mapping için (T -> current type).
/// </summary>
public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}

/// <summary>
/// İki yönlü mapping için (T <-> current type).
/// </summary>
public interface IMapFromTo<T> : IMapFrom<T>, IMapTo<T>
{
    void IMapFrom<T>.Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    void IMapTo<T>.Mapping(Profile profile) => profile.CreateMap(GetType(), typeof(T));
}