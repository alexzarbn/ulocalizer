using System;
using System.Linq;
using ULocalizer.Binding;

namespace ULocalizer.Classes
{
    public static class CDefaults
    {
        public static void SetItem(CTranslationNode translationNode, CTranslationNodeItem nodeItem)
        {
            try
            {
                nodeItem.Translation = Projects.CurrentProject.Translations.First(translation => translation.Culture.ISO == Projects.CurrentProject.SourceCulture.ISO).Nodes.First(node => node.Title == translationNode.Title).Items.First(item => item.Source == nodeItem.Source).Translation;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SetNode(CTranslationNode node)
        {
            node.Items.ToList().ForEach((item)=>SetItem(node,item));
        }

        public static void SetTranslation(CTranslation translation)
        {
            translation.Nodes.ToList().ForEach(SetNode);
        }
    }
}