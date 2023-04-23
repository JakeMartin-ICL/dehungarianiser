using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
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

    public override string Text => "Remove Hungarian notation";

    public override bool IsAvailable(IUserDataHolder cache)
    {
        return _declaration.IsValid();
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        //var declaredElement = _declaration.DeclaredElement.NotNull();
        
        Renamer.RenameDeclaredElement(_declaration, _declaration.GetType() == typeof(IFieldDeclaration));

        return null;
    }




}
