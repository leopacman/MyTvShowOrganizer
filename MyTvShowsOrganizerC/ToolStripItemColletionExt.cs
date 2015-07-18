using System.Collections.Generic;
using System.Windows.Forms;

namespace MyTvShowsOrganizer
{ //it is not been used
    public static class ToolStripItemColletionExtraction
    {

        private static IEnumerable<ToolStripItem> GetSubItems(ToolStripItem item)
        {
            if (item is ToolStripMenuItem)
            {
                foreach (ToolStripItem tsi in (item as ToolStripMenuItem).DropDownItems)
                {
                    if (tsi is ToolStripMenuItem)
                    {
                        if ((tsi as ToolStripMenuItem).HasDropDownItems)
                        {
                            foreach (ToolStripItem subItem in GetSubItems((tsi as ToolStripMenuItem)))
                                yield return subItem;
                        }
                        yield return (tsi as ToolStripMenuItem);
                    }
                    else if (tsi is ToolStripSeparator)
                    {
                        yield return (tsi as ToolStripSeparator);
                    }
                }
            }
            else if (item is ToolStripSeparator)
            {
                yield return (item as ToolStripSeparator);
            }
        }


        public static List<ToolStripItem> GetAllMenuItems(this ToolStripItemCollection items)
        {


            List<ToolStripItem> allItems = new List<ToolStripItem>();
            foreach (ToolStripItem toolItem in items)
            {
                allItems.Add(toolItem);
                //add sub items
                allItems.AddRange(GetSubItems(toolItem));
            }

            return allItems;

        }


        //Loop the list:






    }
}
