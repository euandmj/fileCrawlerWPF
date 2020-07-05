using System;
using System.Windows.Input;

namespace fileCrawlerWPF.Util
{
    public class WaitCursor : IDisposable
    {
        private Cursor past_cursor; 

        public WaitCursor()
        {
            past_cursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = past_cursor;
        }
    }
}
