using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugin.Dehungarianiser;

[ZoneMarker]
public class ZoneMarker : IRequire<ILanguageCSharpZone> { }
