using JetBrains.Application;
using JetBrains.ReSharper.Feature.Services.Explanatory;

namespace ReSharperPlugin.Dehungarianiser;

[ShellComponent]
public class WikiDataProvider : ICodeInspectionWikiDataProvider
{
    public bool TryGetValue(string attributeId, out string url)
    {
        url = attributeId switch
        {
            HungarianHighlighting.SeverityId => "https://youtu.be/dQw4w9WgXcQ",
            _ => null
        };
        return url != null;
    }
}
