using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;

namespace fileCrawlerWPF.Extensions
{
    public static class RichTextBoxExtensions
    {
        private static readonly Regex _regex = new Regex(@"[^a-zA-Z0-9 -]");


        public static string GetText(this RichTextBox rtb)
        {

            string txt = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
                ).Text;
            return _regex.Replace(txt, string.Empty);
        }
    }
}
