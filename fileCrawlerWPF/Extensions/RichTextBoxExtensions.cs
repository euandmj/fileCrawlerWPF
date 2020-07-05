using System.Windows.Controls;
using System.Windows.Documents;

namespace fileCrawlerWPF.Extensions
{
    public static class RichTextBoxExtensions
    {



        public static string GetText(this RichTextBox rtb)
        {
            return new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
                ).Text;
        }
    }
}
