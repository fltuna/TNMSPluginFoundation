using System.Globalization;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;

namespace TnmsPluginFoundation.Extensions;

public static class ConVarExtensions
{
    private const string NoneValueString = "<none>";

    public static string GetCvarValueString(this IConVar cvar)
    {
        switch (cvar.Type)
        {
            case ConVarType.Bool:
                return cvar.Get().AsBool.ToString();
            case ConVarType.Int16:
                return cvar.Get().AsInt16.ToString();
            case ConVarType.UInt16:
                return cvar.Get().AsUInt16.ToString();
            case ConVarType.Int32:
                return cvar.Get().AsInt32.ToString();
            case ConVarType.UInt32:
                return cvar.Get().AsUInt32.ToString();
            case ConVarType.Int64:
                return cvar.Get().AsInt64.ToString();
            case ConVarType.UInt64:
                return cvar.Get().AsUInt64.ToString();
            case ConVarType.Float32:
                return cvar.Get().AsFloat.ToString(CultureInfo.InvariantCulture);
            case ConVarType.Float64:
                return cvar.Get().AsDouble.ToString(CultureInfo.InvariantCulture);
            case ConVarType.String:
                return cvar.Get().AsString;
            case ConVarType.Color:
                return cvar.Get().AsColor.ToString() ?? NoneValueString;
            case ConVarType.Vector2:
                return cvar.Get().AsVector2D.ToString() ?? NoneValueString;
            case ConVarType.Vector3:
                return cvar.Get().AsVector3D.ToString();
            case ConVarType.Vector4:
                return cvar.Get().AsVector4D.ToString() ?? NoneValueString;
            case ConVarType.QAngle:
                return cvar.Get().AsVector3D.ToString();
            case ConVarType.Invalid:
            default:
                return "Invalid";
        }
    } 
}