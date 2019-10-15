using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColumnBinding
{
    public partial class Form1 : Form
    {

        // A ContextMenuStrip as member variable.We need to use this instead of the
        // context menus available in each ListGroup:
        ContextMenuStrip _ListGroupContextMenu;
        ToolStripMenuItem _addOption = new ToolStripMenuItem("Add");
        ToolStripMenuItem _editOption = new ToolStripMenuItem("Edit");
        ToolStripMenuItem _deleteOption = new ToolStripMenuItem("Delete");


        public Form1()
        {
            InitializeComponent();

            //  Set up the context menu to use with the GroupedList Items:
            _ListGroupContextMenu = new ContextMenuStrip();

            // Add some sample ContextMenuStrip Items:
            _addOption = new ToolStripMenuItem("Add");
            _addOption.Click += new EventHandler(btnAddNew_Click);
            _ListGroupContextMenu.Items.Add(_addOption);

            _editOption = new ToolStripMenuItem("Edit");
            _editOption.Click += new EventHandler(btnEdit_Click);
            _ListGroupContextMenu.Items.Add(_editOption);

            _deleteOption = new ToolStripMenuItem("Delete");
            _deleteOption.Click += new EventHandler(btnDelete_Click);
            _ListGroupContextMenu.Items.Add(_deleteOption);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //// Create a GroupedListControl instance:
            //ListView lv1 = new ListView();
            lv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));

            // Add some sample columns:
            lv1.Columns.Add(new MyColumnHeader() { Text = "StudentID", Name = "CptEnvE", Width = 80, TextAlign = HorizontalAlignment.Left, Control = textBox1 });
            lv1.Columns.Add(new MyColumnHeader() { Text = "Faculty", Name = "CptEnvE", Width = 160, TextAlign = HorizontalAlignment.Left, Control = textBox2 });
            lv1.Columns.Add(new MyColumnHeader() { Text = "Enrolled", Name = "CptEnvE", Width = 60, TextAlign = HorizontalAlignment.Left, Control = checkBox1, SelectValues = "Y|N|N/A" });

            Random rnd = new Random();
            string[] EnrolledOptions = { "Y", "N", "N/A" };

            // Now add some sample items:
            for (int j = 1; j <= 5; j++)
            {
                ListViewItem item = lv1.Items.Add("Item " + j.ToString());
                item.SubItems.Add(item.Text + " SubItem 1");
                item.SubItems.Add(EnrolledOptions[rnd.Next(EnrolledOptions.Length)]);
            }

            // Add handling for the RightClick Event for contextual menu
            lv1.MouseClick += new MouseEventHandler(lg_MouseClick);
            // Add handling for the ListViewItem change Event
            lv1.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(lg_ItemSelectionChanged);
        }

        public ListViewItem ModifyItem()
        {
            ListViewItem item = null;
            if (lv1.SelectedItems.Count == 1)
            {
                item = lv1.SelectedItems[0];
                foreach (MyColumnHeader column in lv1.Columns)
                {
                    string text = column.GetTextFromDispatching();
                    if (text != null)
                    {
                        item.SubItems[column.Index].Text = text;
                    }
                }
            }

            return item;
        }


        public ListViewItem AddItem(int itemIndex = -1)
        {
            ListViewItem ClsAddItemRet = default(ListViewItem);
            var subitems = new string[lv1.Columns.Count - 1 + 1];
            foreach (MyColumnHeader column in lv1.Columns)
            {
                string text = column.GetTextFromDispatching();
                if (text != null)
                    subitems[column.Index] = text;
                else
                    subitems[column.Index] = "";
            }
            var item = new ListViewItem(subitems);
            if (itemIndex >= 0)
                ClsAddItemRet = lv1.Items.Insert(itemIndex, item);
            else
            {
                ClsAddItemRet = lv1.Items.Add(item);
                item.EnsureVisible();
            }
            return ClsAddItemRet;
        }

        private void Dispatching()
        {
            if (lv1.SelectedItems.Count == 1)
            {
                foreach (ColumnHeader column in lv1.Columns)
                {
                    if (column.GetType().Equals(typeof(MyColumnHeader)))
                    {
                        var uColumn = (MyColumnHeader)column;
                        Control controler = uColumn.Control;
                        if (controler != null)
                        {
                            ListViewItem item = lv1.SelectedItems[0];
                            if (controler.GetType().GetProperty("CheckState") != null)
                            {
                                controler.GetType().GetProperty("CheckState").SetValue(controler, uColumn.GetCheckState(item.SubItems[column.Index].Text));
                            }
                            else { 
                                controler.Text = item.SubItems[column.Index].Text;
                            }
                        }
                    }
                }
            }
        }

        // Events
        void lg_MouseClick(object sender, MouseEventArgs e)
        {

            ListView lv = (ListView)sender;

            ListViewHitTestInfo info = lv.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (e.Button == MouseButtons.Right)
            {
                // Tuck the Active ListGroup into the Tag property:
                _ListGroupContextMenu.Tag = lv;

                // Make sure the Delete and Edit options are enabled:
                _editOption.Enabled = true;
                _deleteOption.Enabled = true;

                // Because we are not using the GroupedList's own ContextMenuStrip, 
                // we need to use the PointToClient method so that the menu appears 
                // in the correct position relative to the control:
                _ListGroupContextMenu.Show(lv, lv.PointToClient(MousePosition));
            }

            //Dispatching();
        }

        void lg_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Dispatching();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            AddItem();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModifyItem();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem item in lv1.SelectedItems)
            { 
                lv1.Items.Remove(item);
            }

        }
    }
}
