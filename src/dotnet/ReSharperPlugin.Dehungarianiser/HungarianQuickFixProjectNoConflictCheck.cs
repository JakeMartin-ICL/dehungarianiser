using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.Dehungarianiser;

[QuickFix]
public class HungarianQuickFixProjectNoConflictCheck : QuickFixBase
{
    private readonly ICSharpDeclaration _declaration;

    public HungarianQuickFixProjectNoConflictCheck(ICSharpDeclaration declaration)
    {
        _declaration = declaration;
    }

    public HungarianQuickFixProjectNoConflictCheck(HungarianHighlighting highlighting)
    {
        _declaration = highlighting.Declaration;
    }

    public override string Text => "Remove hungarian notation in project (no conflict check)";

    public override bool IsAvailable(IUserDataHolder cache)
    {
        return _declaration.IsValid();
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        IProject project = _declaration.GetProject();

        Action<ITextControl> textControl = Dehungarianiser.RemoveHungarianNotationInProject(project, solution, progress, false);

        progress.Stop();

        return textControl;
    }
}