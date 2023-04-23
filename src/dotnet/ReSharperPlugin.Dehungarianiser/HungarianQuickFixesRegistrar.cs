using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.Util;

namespace ReSharperPlugin.Dehungarianiser;

[ShellComponent]
internal class HungarianQuickFixesRegistrar : IQuickFixesProvider
{
    public void Register(IQuickFixesRegistrar registrar)
    {
        registrar.RegisterQuickFix<HungarianHighlighting>(Lifetime.Eternal, h => new HungarianQuickFix(h.Declaration), typeof(HungarianQuickFix));
        registrar.RegisterQuickFix<HungarianHighlighting>(Lifetime.Eternal, h => new HungarianQuickFixFile(h.Declaration), typeof(HungarianQuickFixFile));
        registrar.RegisterQuickFix<HungarianHighlighting>(Lifetime.Eternal, h => new HungarianQuickFixProject(h.Declaration), typeof(HungarianQuickFixProject));
    }

    public IEnumerable<Type> Dependencies => EmptyArray<Type>.Instance;
}
