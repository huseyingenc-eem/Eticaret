namespace Core.Application;

public class CachingConfiguration
{
    public int SlidingExpiration { get; set; } = 10;
    public int AbsoluteExpiration { get; set; } = 60;
    // Diğer önbellek ayarları buraya eklenebilir.
}