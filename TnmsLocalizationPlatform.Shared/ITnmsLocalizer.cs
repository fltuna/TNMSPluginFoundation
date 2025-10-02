using System.Globalization;
using Microsoft.Extensions.Localization;
using Sharp.Shared.Objects;

namespace TnmsLocalizationPlatform.Shared;

public interface ITnmsLocalizer: IStringLocalizer
{
    public LocalizedString ForClient(IGameClient client, string name, params object[] arguments);
    
    public IEnumerable<LocalizedString> GetAllStringsByCulture(CultureInfo culture);
}