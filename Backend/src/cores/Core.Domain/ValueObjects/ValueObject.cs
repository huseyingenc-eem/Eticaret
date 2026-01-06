namespace Core.Domain.ValueObjects;

public abstract class ValueObject
{
    /// <summary>
    /// Değer nesnesinin eşitliğini oluşturan bileşenleri alır.
    /// Türetilen sınıflar, kendi kurucu parçalarını döndürmek için bu metodu uygulamalıdır.
    /// </summary>
    /// <returns>Değer nesnesinin bileşenlerini içeren bir yinelenebilir koleksiyon.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }
        return ReferenceEquals(left, null) || left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}