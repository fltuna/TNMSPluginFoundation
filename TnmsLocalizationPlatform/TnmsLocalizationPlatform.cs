using Sharp.Shared;
using TnmsLocalizationPlatform.Shared;

namespace TnmsLocalizationPlatform;

public class TnmsLocalizationPlatform: IModSharpModule, ITnmsLocalizationPlatform
{
    public string DisplayName => "TnmsLocalizationPlatform";
    public string DisplayAuthor => "faketuna";
    
    
    public bool Init()
    {
        throw new NotImplementedException();
    }

    public void Shutdown()
    {
        throw new NotImplementedException();
    }
}