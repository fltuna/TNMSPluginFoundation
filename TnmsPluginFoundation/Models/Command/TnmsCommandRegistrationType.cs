namespace TnmsPluginFoundation.Models.Command;

[Flags]
public enum TnmsCommandRegistrationType
{
    Client = 1 << 0,
    Server = 1 << 1,
}