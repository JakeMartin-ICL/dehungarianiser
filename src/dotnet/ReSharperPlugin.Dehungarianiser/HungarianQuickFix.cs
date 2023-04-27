using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.Dehungarianiser;

[QuickFix]
public class HungarianQuickFix : QuickFixBase
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
}