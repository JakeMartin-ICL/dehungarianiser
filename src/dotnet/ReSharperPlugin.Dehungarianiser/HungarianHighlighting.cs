using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharperPlugin.Dehungarianiser;

[RegisterConfigurableSeverity(
    SeverityId,
    CompoundItemName: null,
    CompoundItemNameResourceType: null,
    CompoundItemNameResourceName: null,
    Group: HighlightingGroupIds.CodeSmell,
    Title: "Hungarian notation should be removed",
    TitleResourceType: typeof(Resources),
    TitleResourceName: nameof(Resources.HungarianHighlightingTitle),
    Description: "Hungarian notation is considered bad practice and should be removed.",
    DescriptionResourceType: typeof(Resources),
    DescriptionResourceName: nameof(Resources.HungarianHighlightingDescription),
    DefaultSeverity: Severity.WARNING)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    OverlapResolve = OverlapResolveKind.ERROR,
    OverloadResolvePriority = 0,
    ToolTipFormatStringResourceType = typeof(Resources),
    ToolTipFormatStringResourceName = nameof(Resources.HungarianHighlightingToolTipFormat))]
public class HungarianHighlighting : IHighlighting
{
    public const string SeverityId = "Sample"; // Appears in suppression comments

    public HungarianHighlighting(ICSharpDeclaration declaration)
    {
        Declaration = declaration;
    }

    public ICSharpDeclaration Declaration { get; }

    public bool IsValid()
    {
        return Declaration.IsValid();
    }

    public DocumentRange CalculateRange()
    {
        return Declaration.NameIdentifier.NotNull().GetHighlightingRange();
    }

    public string ToolTip => Resources.HungarianHighlightingToolTipFormat; // Can be used with string.Format
    public string ErrorStripeToolTip => Resources.HungarianHighlightingErrorStripeToolTip;
}