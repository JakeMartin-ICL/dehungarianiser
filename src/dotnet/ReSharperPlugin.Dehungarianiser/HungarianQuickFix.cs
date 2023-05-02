using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Intentions.Scoped;
using JetBrains.ReSharper.Feature.Services.Intentions.Scoped.Actions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Feature.Services.Refactorings.Specific.Rename;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.Dehungarianiser;

[QuickFix]
public class HungarianQuickFix : QuickFixBase, IHighlightingsSetScopedAction
{
    private readonly ICSharpDeclaration _declaration;

    public HungarianQuickFix(ICSharpDeclaration declaration)
    {
        _declaration = declaration;
    }

    public HungarianQuickFix(HungarianHighlighting highlighting)
    {
        _declaration = highlighting.Declaration;
    }

    public override string Text => "Remove Hungarian notation";

    public override bool IsAvailable(IUserDataHolder cache)
    {
        return _declaration.IsValid();
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        return Dehungarianiser.RenameDeclaredElement(_declaration, solution);
    }

    public string ScopedText => Text;
    public FileCollectorInfo FileCollectorInfo => FileCollectorInfo.Default;

    public Action<ITextControl> ExecuteAction(IEnumerable<HighlightingInfo> highlightings, ISolution solution,
        IProgressIndicator progress)
    {
        HungarianHighlighting[] hungarianHighlightings = highlightings
            .Select(h => h.Highlighting)
            .Cast<HungarianHighlighting>().ToArray();

        progress.Start(hungarianHighlightings.Count());

        Dictionary<IDeclaredElement, string> newNames =
            Dehungarianiser.GetRenamesForDeclarations(hungarianHighlightings.Select(x => x.Declaration));

        return textControl => RenameRefactoringService.Rename(solution,
            new RenameDataProvider(newNames) { Model = { Bulk = true } }, textControl);
    }
}