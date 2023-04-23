using System.Text.RegularExpressions;

namespace ReSharperPlugin.Dehungarianiser
{
  using System;
  using JetBrains.Application.I18n;
  using JetBrains.DataFlow;
  using JetBrains.Diagnostics;
  using JetBrains.Lifetimes;
  using JetBrains.Util;
  using JetBrains.Util.Logging;
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  public static class Resources
  {
    private static readonly ILogger ourLog = Logger.GetLogger("ReSharperPlugin.CodeInspections.Resources");

    static Resources()
    {
      CultureContextComponent.Instance.WhenNotNull(Lifetime.Eternal, (lifetime, instance) =>
      {
        lifetime.Bracket(() =>
          {
            ourResourceManager = new Lazy<JetResourceManager>(
              () =>
              {
                return instance
                  .CreateResourceManager("ReSharperPlugin.CodeInspections.Resources", typeof(Resources).Assembly);
              });
          },
          () =>
          {
            ourResourceManager = null;
          });
      });
    }
    
    private static Lazy<JetResourceManager> ourResourceManager = null;
    
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    public static JetResourceManager ResourceManager
    {
      get
      {
        var resourceManager = ourResourceManager;
        if (resourceManager == null)
        {
          return ErrorJetResourceManager.Instance;
        }
        return resourceManager.Value;
      }
    }

    public static string HungarianHighlightingCompoundName => ResourceManager.GetString("HungarianHighlightingCompoundName");
    public static string HungarianHighlightingTitle => ResourceManager.GetString("HungarianHighlightingTitle");
    public static string HungarianHighlightingDescription => ResourceManager.GetString("HungarianHighlightingDescription");
    public static string HungarianHighlightingToolTipFormat => ResourceManager.GetString("HungarianHighlightingToolTipFormat");
    public static string HungarianHighlightingToolTip => ResourceManager.GetString("HungarianHighlightingToolTip");
    public static string HungarianHighlightingErrorStripeToolTip => ResourceManager.GetString("HungarianHighlightingErrorStripeToolTip");
    public static string HungarianQuickFixText => ResourceManager.GetString("HungarianQuickFixText");
    // public static string HungarianQuickFixText => "Remove hungarian notation";


    public static string PatternString =
      "\\b((p)|(?<field>m))(int|str|lst|dt|lng|dbl|dict|b|fn|div|hsh|hash|arr|row|dct|ih|ts|act)?(?<startingcaps>[A-Z]+)(?<rest>\\w+)?";
    public static Regex RegexPattern = new Regex(PatternString);
  }
}