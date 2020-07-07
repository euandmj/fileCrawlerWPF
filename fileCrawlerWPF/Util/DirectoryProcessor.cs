using fileCrawlerWPF.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace fileCrawlerWPF.Util
{
    class DirectoryProcessor
    {
        // bind treeview to media property where it selects enumerable<directry, guid>

        public static TreeViewItem BuildFileTree(IEnumerable<FileDirectory> dirs)
        {
            if (dirs is null) return new TreeViewItem() { Header = "none" };

            var roots = dirs.Select(x => x.DirectoryInfo.Root.Name).Distinct().ToList();

            var tree = BuildRoot(roots);


            foreach (var d in dirs)
            {
                var o = BubbleToRoot(d.DirectoryInfo).Reverse().Skip(1);
                var root_match = MatchRoot(tree, d.DirectoryInfo.Root.Name);
                StepAdd(root_match, o, d.ID);
            }


            //var win = new Window1(tree);
            //win.ShowDialog();

            return tree;
        }


        private static TreeViewItem MatchRoot(TreeViewItem roots, string headerText)
        {
            foreach(TreeViewItem i in roots.Items)
            {
                if (Equals(headerText, i.Header))
                {
                    return i;
                }
            }
            return roots;
        }

        private static void StepAdd(TreeViewItem root, IEnumerable<DirectoryInfo> dirs, Guid tag)
        {
            TreeViewItem curr = root;

            foreach(var d in dirs)
            {
                var existingDirectory = GetChildren(curr).FirstOrDefault(x => Equals(x.Header, d.Name));

                if (existingDirectory != null)
                {
                    curr = existingDirectory;
                }
                else
                {
                    var nn = new TreeViewItem() { Header = d.Name };
                    curr.Items.Add(nn);
                    curr.Tag = tag;
                    curr = nn;
                }
            }
        }

        private static TreeViewItem BuildRoot(List<string> roots)
        {
            TreeViewItem tree = new TreeViewItem();

            if (roots.Count() == 1)
            {
                tree.Header = roots.First();
            }
            else
            {
                roots.ForEach(x =>
                {
                    tree.Items.Add(new TreeViewItem() { Header = x });
                });
            }
            return tree;
        }

        private static IEnumerable<DirectoryInfo> BubbleToRoot(DirectoryInfo i)
        {
            yield return i;

            if(i.Parent != null)
            {
                foreach(var p in BubbleToRoot(i.Parent))
                {
                    yield return p;
                }
            }
        }

        private static IEnumerable<TreeViewItem> GetChildren(TreeViewItem node)
        {
            foreach (TreeViewItem i in node.Items)
                yield return i;
        }
    }
}