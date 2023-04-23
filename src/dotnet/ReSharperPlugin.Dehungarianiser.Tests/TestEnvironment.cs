using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.Dehungarianiser.Tests
{
    [ZoneDefinition]
    public class DehungarianiserTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IDehungarianiserZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<DehungarianiserTestEnvironmentZone> { }

    [SetUpFixture]
    public class DehungarianiserTestsAssembly : ExtensionTestEnvironmentAssembly<DehungarianiserTestEnvironmentZone> { }
}
