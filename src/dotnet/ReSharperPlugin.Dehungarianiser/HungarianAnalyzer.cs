using System.Linq;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharperPlugin.Dehungarianiser;

// Types mentioned in this attribute are used for performance optimizations
[ElementProblemAnalyzer(
    typeof(ICSharpDeclaration),
    HighlightingTypes = new[] { typeof(HungarianHighlighting) })]
public class HungarianAnalyzer : ElementProblemAnalyzer<ICSharpDeclaration>
{
    protected override void Run(
        ICSharpDeclaration element,
        ElementProblemAnalyzerData data,
        IHighlightingConsumer consumer)
    {
        string name = element.NameIdentifier?.Name;
        if (name == null || !Resources.RegexPattern.Match(name).Success )
            return;

        consumer.AddHighlighting(new HungarianHighlighting(element));
    }
}
