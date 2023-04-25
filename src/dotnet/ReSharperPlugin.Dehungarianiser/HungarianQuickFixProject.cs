using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.I18n.Services;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.Dehungarianiser;

[QuickFix]
public class HungarianQuickFixProject : QuickFixBase
{
    private readonly ICSharpDeclaration _declaration;

    public HungarianQuickFixProject(ICSharpDeclaration declaration)
    {
        _declaration = declaration;
    }
    
    public HungarianQuickFixProject(HungarianHighlighting highlighting)
    {
        _declaration = highlighting.Declaration;
    }

    public override string Text => "Remove hungarian notation in project";

    public override bool IsAvailable(IUserDataHolder cache)
    {
        return _declaration.IsValid();
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        IProject project = _declaration.GetProject();
        
        Action<ITextControl> textControl = Renamer.RemoveHungarianNotationInProject(project, solution, progress, true);
        
        progress.Stop();
        
        return textControl;
    }
}
