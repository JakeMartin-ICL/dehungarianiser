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
public class HungarianQuickFixFile : QuickFixBase
{
    private readonly ICSharpDeclaration _declaration;

    public HungarianQuickFixFile(ICSharpDeclaration declaration)
    {
        _declaration = declaration;
    }

    public HungarianQuickFixFile(HungarianHighlighting highlighting)
    {
        _declaration = highlighting.Declaration;
    }

    public override string Text => "Remove hungarian notation in file";

    public override bool IsAvailable(IUserDataHolder cache)
    {
        return _declaration.IsValid();
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        IFile file = _declaration.GetContainingFile();

        return Dehungarianiser.RemoveHungarianNotationInFile(file, solution);
    }
}