using JetBrains.Application.I18n;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharperPlugin.Dehungarianiser;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    null,
    null,
    HighlightingGroupIds.CodeSmell,
    "Hungarian notation should be removed",
    typeof(Resources),
    nameof(Resources.HungarianHighlightingTitle),
    "Hungarian notation is considered bad practice and should be removed",
    typeof(Resources),
    nameof(Resources.HungarianHighlightingDescription),
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    OverlapResolve = OverlapResolveKind.ERROR,
    OverloadResolvePriority = 0)]
public class HungarianHighlighting : IHighlighting
{
    public const string SeverityId = "Sample"; // Appears in suppression comments

    private const string Message = "Hungarian notation is considered bad practice and should be removed";

    public HungarianHighlighting(ICSharpDeclaration declaration)
    {
        Declaration = declaration;
    }

    public ICSharpDeclaration Declaration { get; }

    public string ToolTip => Message.NON_LOCALIZABLE();

    public string ErrorStripeToolTip => ToolTip;

    public bool IsValid()
    {
        return Declaration.IsValid();
    }

    public DocumentRange CalculateRange()
    {
        return Declaration.NameIdentifier.NotNull().GetHighlightingRange();
    }
}