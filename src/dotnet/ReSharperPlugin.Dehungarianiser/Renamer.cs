using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Refactorings.Specific.Rename;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Naming.Interfaces;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace ReSharperPlugin.Dehungarianiser;

public static class Renamer
{
    public static Action<ITextControl> RenameDeclaredElement(IDeclaration declaration, ISolution solution)
    {
        IDeclaredElement declaredElement = declaration.DeclaredElement;
        if (declaredElement == null) return null;
        Match match = Resources.RegexPattern.Match(declaration.DeclaredName);
        if (!match.Success) return null;
        string newName = NewName(match);

        string fixedName = FixupName(declaration, newName);

        return textControl => RenameRefactoringService.Rename(solution,
            new RenameDataProvider(declaredElement, fixedName), textControl);
    }

    private static string FixupName(IDeclaration declaration, string newName)
    {
        IDeclaredElement declaredElement = declaration.DeclaredElement;
        if (declaredElement == null) return newName;
        INamingPolicyProvider namingPolicyProvider = declaration.GetPsiServices().Naming.Policy
            .GetPolicyProvider(declaration.GetKnownLanguage(), declaration.GetSourceFile());
        NamingPolicy policy = namingPolicyProvider.GetPolicy(declaredElement);
        NamingManager naming = declaredElement.GetPsiServices().Naming;
        Name name = naming.Parsing.Parse(newName, policy.NamingRule, namingPolicyProvider);
        if (!name.HasErrors || declaredElement is IOverridableMember overridableMember &&
            overridableMember.GetAccessRights() == AccessRights.PUBLIC &&
            overridableMember.GetRootSuperMembers().Count > 0)
            return newName;
        if (policy.ExtraRules.Any(
                extraRule => !naming.Parsing.Parse(newName, extraRule, namingPolicyProvider).HasErrors))
        {
            return newName;
        }

        string canonicalName = name.GetCanonicalName();
        return canonicalName == string.Empty ? newName : canonicalName;
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

    private static string NewName(Match match)
    {
        string basename = match.Groups["basename"].Value;
        basename = basename switch
        {
            "Params" => "parameters",
            "Return" => "result",
            "Default" => "defaultValue",
            _ => basename
        };

        return basename;
    }

    public static Action<ITextControl> RemoveHungarianNotationInFile(IFile file, ISolution solution)
    {
        Dictionary<IDeclaredElement, string> newNames = GetRenamesForFile(file);

        return textControl => RenameRefactoringService.Rename(solution,
            new RenameDataProvider(newNames) { Model = { Bulk = true } }, textControl);
    }

    private static Dictionary<IDeclaredElement, string> GetRenamesForFile(IFile file)
    {
        TreeNodeExtensions.FilteredDescendantsEnumerator<IDeclaration> declarations =
            file.Descendants<IDeclaration>();

        var newNames = new Dictionary<IDeclaredElement, string>();

        foreach (IDeclaration declaration in declarations)
        {
            Match match = Resources.RegexPattern.Match(declaration.DeclaredName);
            if (declaration.DeclaredElement != null && match.Success)
                newNames.Add(declaration.DeclaredElement, FixupName(declaration, NewName(match)));
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