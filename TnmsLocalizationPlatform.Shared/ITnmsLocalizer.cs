using System.Globalization;
using Microsoft.Extensions.Localization;
using Sharp.Shared.Objects;

namespace TnmsLocalizationPlatform.Shared;

public interface ITnmsLocalizer: IStringLocalizer
{
    public LocalizedString this[string name, CultureInfo culture] { get; }
    
    public LocalizedString this[string name, CultureInfo culture, params object[] arguments] { get; }
    
    public CultureInfo GetClientCulture(IGameClient client);
    
    public LocalizedString ForClient(IGameClient client, string name, params object[] arguments);
    
    public IEnumerable<LocalizedString> GetAllStringsByCulture(CultureInfo culture);
}