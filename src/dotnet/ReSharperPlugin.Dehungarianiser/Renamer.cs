using System;
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
using JetBrains.TextControl;

namespace ReSharperPlugin.Dehungarianiser;

public static class Renamer
{
    public static Action<ITextControl> RenameDeclaredElement(IDeclaration declaration, bool field, ISolution solution)
    {
        IDeclaredElement declaredElement = declaration.DeclaredElement;
        if (declaredElement == null) return null;
        Match match = Resources.RegexPattern.Match(declaration.DeclaredName);
        if (!match.Success) return null;
        string newName = NewName(match, field);

        return textControl => RenameRefactoringService.Rename(solution,
            new RenameDataProvider(declaredElement, newName), textControl);
    }

    private static void QuickRename(IDeclaration declaration, string newName)
    {
        IDeclaredElement declaredElement = declaration.DeclaredElement;
        if (declaredElement == null) return;
        Match match = Resources.RegexPattern.Match(declaration.DeclaredName);
        if (!match.Success) return;

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

    private static string NewName(Match match, bool field)
    {
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
        return newName;
    }

    public static Action<ITextControl> RemoveHungarianNotationInFile(IFile file, ISolution solution)
    {
        Dictionary<IDeclaredElement, string> newNames = GetRenamesForFile(file);

        return textControl => RenameRefactoringService.Rename(solution,
            new RenameDataProvider(newNames) { Model = { Bulk = true } }, textControl);
    }

    private static Dictionary<IDeclaredElement, string> GetRenamesForFile(IFile file)
    {
        TreeNodeExtensions.FilteredDescendantsEnumerator<ILocalVariableDeclaration> localVarDeclarations =
            file.Descendants<ILocalVariableDeclaration>();
        TreeNodeExtensions.FilteredDescendantsEnumerator<IParameterDeclaration> parameterDeclarations =
            file.Descendants<IParameterDeclaration>();
        TreeNodeExtensions.FilteredDescendantsEnumerator<IFieldDeclaration> fieldDeclarations =
            file.Descendants<IFieldDeclaration>();

        var newNames = new Dictionary<IDeclaredElement, string>();

        foreach (ILocalVariableDeclaration dec in localVarDeclarations)
        {
            Match match = Resources.RegexPattern.Match(dec.DeclaredName);
            if (match.Success) newNames.Add(dec.DeclaredElement, NewName(match, false));
        }

        foreach (IParameterDeclaration dec in parameterDeclarations)
        {
            Match match = Resources.RegexPattern.Match(dec.DeclaredName);
            if (match.Success && dec.DeclaredElement != null) newNames.Add(dec.DeclaredElement, NewName(match, false));
        }

        foreach (IFieldDeclaration dec in fieldDeclarations)
        {
            Match match = Resources.RegexPattern.Match(dec.DeclaredName);
            if (match.Success && dec.DeclaredElement != null) newNames.Add(dec.DeclaredElement, NewName(match, true));
        }

        return newNames;
    }

    public static Action<ITextControl> RemoveHungarianNotationInProject(IProject project, ISolution solution,
        IProgressIndicator progress, bool checkConflicts)
    {
        List<IProjectFile> files = project.GetAllProjectFiles().ToList();
        progress.Start(files.Count * 2);
        var newNames = new Dictionary<IDeclaredElement, string>();

        if (!checkConflicts)
        {
            foreach (IProjectFile file in files)
            {
                progress.CurrentItemText = $"Removing Hungarian notation in {file.Name}";
                Dictionary<IDeclaredElement, string> newNamesForFile = GetRenamesForFile(file.GetPrimaryPsiFile());
                foreach (KeyValuePair<IDeclaredElement, string> kvp in newNamesForFile)
                {
                    QuickRename(kvp.Key.GetSingleDeclaration(), kvp.Value);
                }

                progress.Advance();
            }

            return null;
        }

        foreach (IProjectFile file in files)
        {
            progress.CurrentItemText = $"Detecting Hungarian notation in {file.Name}";
            newNames = newNames.Concat(GetRenamesForFile(file.GetPrimaryPsiFile()))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            progress.Advance();
        }

        progress.CurrentItemText = $"Executing {newNames.Count} rename refactorings";

        return textControl => RenameRefactoringService.Rename(solution,
            new RenameDataProvider(newNames) { Model = { Bulk = true } }, textControl);
    }
}