using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Refactorings.Specific.Rename;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Refactorings.Rename;

namespace ReSharperPlugin.Dehungarianiser;

public static class Renamer
{
    public static void RenameDeclaredElement(IDeclaration declaration, bool field)
    {
        IDeclaredElement declaredElement = declaration.DeclaredElement;
        if (declaredElement == null) return;
        Match match = Resources.RegexPattern.Match(declaration.DeclaredName);
        if (!match.Success) return;
        string startingCaps = match.Groups["startingcaps"].Value;
        string restOfTheName = match.Groups["rest"].Value;
        restOfTheName = restOfTheName switch
        {
            "arams" => "arameters",
            "eturn" => "esult",
            "efault" => "efaultValue",
            _ => restOfTheName
        };
        string newName = (field ? startingCaps.ToUpper() : startingCaps.ToLower()) + restOfTheName;

        IPsiServices psiServices = declaration.GetPsiServices();
        var references = new List<IReference>();
        psiServices.Finder.FindReferences(
            declaredElement,
            domain: psiServices.SearchDomainFactory.CreateSearchDomain(declaration.GetSolution(), false),
            consumer: references.ConsumeReferences(),
            NullProgressIndicator.Create());
        
        declaration.SetName(newName);
        
        foreach (IReference reference in references)
            reference.BindTo(declaredElement);
    }

    public static void RemoveHungarianNotationInFile(IFile file)
    {
        TreeNodeExtensions.FilteredDescendantsEnumerator<ILocalVariableDeclaration> localVarDeclarations =
            file.Descendants<ILocalVariableDeclaration>();
        TreeNodeExtensions.FilteredDescendantsEnumerator<IParameterDeclaration> parameterDeclarations =
            file.Descendants<IParameterDeclaration>();
        TreeNodeExtensions.FilteredDescendantsEnumerator<IFieldDeclaration> fieldDeclarations =
            file.Descendants<IFieldDeclaration>();

        foreach (ILocalVariableDeclaration dec in localVarDeclarations)
        {
            RenameDeclaredElement(dec, false);
        }

        foreach (IParameterDeclaration dec in parameterDeclarations)
        {
            RenameDeclaredElement(dec, false);
        }

        foreach (IFieldDeclaration dec in fieldDeclarations)
        {
            RenameDeclaredElement(dec, true);
        }
    }

    public static void RemoveHungarianNotationInProject(IProject project, IProgressIndicator progress)
    {
        List<IProjectFile> files = project.GetAllProjectFiles().ToList();
        progress.Start(files.Count);
        foreach (IProjectFile file in files)
        {
            progress.CurrentItemText = $"Removing Hungarian notation in {file.Name}";
            progress.Advance();
            RemoveHungarianNotationInFile(file.GetPrimaryPsiFile());
        }
    }
}