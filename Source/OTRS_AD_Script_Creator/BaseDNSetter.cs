using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Drawing;
using System.Windows.Forms;

namespace OTRS_AD_Script_Creator
{
	public class BaseDNSetter : Form
	{
		private string BaseDN;

		private DirectoryEntry Base;

		private string Host;

		private string User;

		private string Password;

		private IContainer components;

		private Panel panel1;

		private ListView ctr_list;

		private Splitter splitter1;

		private TreeView ctr_tree;

		private Button Discard_button;

		private Button Accept_button;

		public BaseDNSetter(string pHost, string pUser, string pPassword)
		{
			this.InitializeComponent();
			this.Host = pHost;
			this.User = pUser;
			this.Password = pPassword;
			this.GetPropertiesDisplyinForm();
		}

		public void GetPropertiesDisplyinForm()
		{
			try
			{
				if (string.IsNullOrEmpty(this.User) || string.IsNullOrEmpty(this.Password))
				{
					this.Base = new DirectoryEntry("LDAP://" + this.Host);
				}
				else
				{
					this.Base = new DirectoryEntry("LDAP://" + this.Host, this.User, this.Password);
				}
				if (this.Base != null)
				{
					this.ctr_tree.Nodes.Clear();
					this.ctr_tree.BeginUpdate();
					DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://RootDSE");
					TreeNode treeNode = this.ctr_tree.Nodes.Add(directoryEntry.Properties["defaultNamingContext"].Value.ToString());
					treeNode.Tag = this.Base;
					try
					{
						foreach (DirectoryEntry directoryEntry2 in this.Base.Children)
						{
							TreeNode treeNode2 = treeNode.Nodes.Add(directoryEntry2.Name);
							treeNode2.Tag = directoryEntry2;
						}
					}
					finally
					{
						treeNode.Expand();
						this.ctr_tree.EndUpdate();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.Close();
			}
		}

		private void ctr_tree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Nodes.Count == 0)
			{
				DirectoryEntry directoryEntry = (DirectoryEntry)e.Node.Tag;
				if (directoryEntry != null && directoryEntry.Children != null)
				{
					foreach (DirectoryEntry directoryEntry2 in directoryEntry.Children)
					{
						TreeNode treeNode = e.Node.Nodes.Add(directoryEntry2.Name);
						treeNode.Tag = directoryEntry2;
					}
				}
			}
			try
			{
				DirectoryEntry directoryEntry3 = (DirectoryEntry)e.Node.Tag;
				if (directoryEntry3 != null)
				{
					this.ctr_list.Clear();
					this.ctr_list.Columns.Add("Attribute", 90, HorizontalAlignment.Left);
					this.ctr_list.Columns.Add("Value", 350, HorizontalAlignment.Left);
					foreach (object current in directoryEntry3.Properties.PropertyNames)
					{
						foreach (object current2 in directoryEntry3.Properties[current.ToString()])
						{
							ListViewItem listViewItem = new ListViewItem(current.ToString(), 0);
							listViewItem.SubItems.Add(current2.ToString());
							this.ctr_list.Items.AddRange(new ListViewItem[]
							{
								listViewItem
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void Accept_button_Click(object sender, EventArgs e)
		{
			this.BaseDN = this.ctr_tree.SelectedNode.FullPath;
			string[] array = this.BaseDN.Split(new char[]
			{
				','
			});
			List<string> list = new List<string>(array.Length);
			list.AddRange(array);
			try
			{
				if (list[0].Contains("DC") && list.Count > 1)
				{
					if (list[1].Contains("DC"))
					{
						list.Reverse(0, 2);
						list.Reverse();
					}
				}
				else
				{
					list.Reverse();
				}
			}
			catch (Exception)
			{
				throw;
			}
			this.BaseDN = "";
			foreach (string current in list)
			{
				this.BaseDN += current;
				this.BaseDN += ",";
			}
			this.BaseDN = this.BaseDN.TrimEnd(new char[]
			{
				','
			});
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void Discard_button_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		internal string GetBaseDN()
		{
			return this.BaseDN;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.panel1 = new Panel();
			this.Discard_button = new Button();
			this.Accept_button = new Button();
			this.ctr_list = new ListView();
			this.splitter1 = new Splitter();
			this.ctr_tree = new TreeView();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.panel1.Controls.Add(this.Discard_button);
			this.panel1.Controls.Add(this.Accept_button);
			this.panel1.Controls.Add(this.ctr_list);
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.ctr_tree);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(255, 433);
			this.panel1.TabIndex = 1;
			this.Discard_button.Location = new Point(9, 399);
			this.Discard_button.Name = "Discard_button";
			this.Discard_button.Size = new Size(75, 23);
			this.Discard_button.TabIndex = 4;
			this.Discard_button.Text = "Discard";
			this.Discard_button.UseVisualStyleBackColor = true;
			this.Discard_button.Click += new EventHandler(this.Discard_button_Click);
			this.Accept_button.Location = new Point(168, 399);
			this.Accept_button.Name = "Accept_button";
			this.Accept_button.Size = new Size(75, 23);
			this.Accept_button.TabIndex = 3;
			this.Accept_button.Text = "Accept";
			this.Accept_button.UseVisualStyleBackColor = true;
			this.Accept_button.Click += new EventHandler(this.Accept_button_Click);
			this.ctr_list.Location = new Point(305, 198);
			this.ctr_list.Name = "ctr_list";
			this.ctr_list.Size = new Size(77, 32);
			this.ctr_list.TabIndex = 2;
			this.ctr_list.UseCompatibleStateImageBehavior = false;
			this.ctr_list.View = View.Details;
			this.ctr_list.Visible = false;
			this.splitter1.Location = new Point(0, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new Size(3, 433);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			this.ctr_tree.Location = new Point(0, 0);
			this.ctr_tree.Name = "ctr_tree";
			this.ctr_tree.PathSeparator = ",";
			this.ctr_tree.ShowNodeToolTips = true;
			this.ctr_tree.Size = new Size(253, 393);
			this.ctr_tree.TabIndex = 0;
			this.ctr_tree.AfterSelect += new TreeViewEventHandler(this.ctr_tree_AfterSelect);
			base.AcceptButton = this.Accept_button;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(255, 433);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			base.Name = "BaseDNSetter";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Set Base-DN";
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
