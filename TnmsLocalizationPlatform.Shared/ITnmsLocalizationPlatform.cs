using System.Globalization;
using Microsoft.Extensions.Localization;
using Sharp.Shared;
using Sharp.Shared.Objects;

namespace TnmsLocalizationPlatform.Shared;

public interface ITnmsLocalizationPlatform
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="module"></param>
    /// <returns></returns>
    public ITnmsLocalizer CreateStringLocalizer(ILocalizableModule module);
}