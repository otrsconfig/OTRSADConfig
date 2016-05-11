using OTRS_AD_Script_Creator.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace OTRS_AD_Script_Creator
{
	public class Main : Form
	{
		private const int EM_GETLINECOUNT = 186;

		private Point PanelMouseDownLocation;

		private int currentStep = 1;

		private Settings settings = new Settings();

		private string Host;

		private string Username;

		private string Password;

		private bool Anonymous;

		private string strFilter = "";

		private string strMapping = "";

		private DirectoryEntry Base;

		private IContainer components;

		private ToolStrip upperBar_toolStrip;

		private ToolStripButton previousStep_toolStripButton;

		private ToolStripButton nextStep_toolStripButton;

		private ToolStripButton nextStep_toolStripButton2;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripSeparator toolStripSeparator2;

		private Label step1_label;

		private Label step2_label;

		private Label step3_label;

		private Label currentStep_label;

		private Label sourcename_label;

		private TextBox sourcename_textBox;

		private Panel HelpReq_Panel;

		private Button HelpReqClose_button;

		private Label HelpReqCaption_label;

		private TextBox HelpReqText_textBox;

		private Panel step1_panel;

		private Panel differenceHostearchuser_panel;

		private TextBox searchPassword_textBox;

		private TextBox searchUser_textBox;

		private Label searchPassword_label;

		private Label searchUser_label;

		private Label host_label;

		private CheckBox differenceHostSearchuser_checkBox;

		private Panel status_panel;

		private GroupBox getBaseDN_groupBox;

		private TextBox baseDN_textBox;

		private Label baseDN_label;

		private Button refreshBaseDNBrowser_button;

		private GroupBox filter_groupBox;

		private Button step1Accept_button;

		private TextBox filterNote_textBox;

		private DataGridView filter_dataGridView;

		private ContextMenuStrip filter_dataGridView_contextMenuStrip;

		private ToolStripMenuItem new_filter_ToolStripMenuItem;

		private ToolStripMenuItem delete_filter_ToolStripMenuItem;

		private SplitContainer getBaseDN_splitContainer;

		private TreeView ctr_treeView;

		private Button setBaseDN_button;

		private ListView ctr_listView;

		private ComboBox scope_comboBox;

		private Button scopeHelp_button;

		private TextBox scope_textBox;

		private Label scope_label;

		private Panel step2_panel;

		private DataGridViewTextBoxColumn FilterAttribute;

		private DataGridViewTextBoxColumn Filter_logicalOperator;

		private DataGridViewTextBoxColumn Filter_Value;

		private Button resetctr_treeView_button;

		private Label customerID_label;

		private Label customerKey_label;

		private Label customerID_Info_label;

		private Label customerKey_Info_label;

		private Label customerUserNameFields_label;

		private Label customerUserPostMasterSearchFields_label;

		private Label customerUserSearchFields_label;

		private Label customerUserListFields_label;

		private ComboBox customerKey_comboBox;

		private TextBox customerID_textBox;

		private TextBox customerUserPostMasterSearchFields_textBox;

		private TextBox customerUserNameFields_textBox;

		private TextBox customerUserSearchFields_textBox;

		private TextBox customerUserListFields_textBox;

		private Label customerUserNameFields_Info_label;

		private Label customerUserPostMasterSearchFields_Info_label;

		private Label customerUserSearchFields_Info_label;

		private Label customerUserListFields_Info_label;

		private Button step2Accept_button;

		private GroupBox mapping_groupBox;

		private DataGridView mapping_dataGridView;

		private Label mapping_Info_label;

		private ComboBox host_comboBox;

		private Button GetDomainController_button;

		private ContextMenuStrip mapping_dataGridView_contextMenuStrip;

		private ToolStripMenuItem new_mapping_ToolStripMenuItem;

		private ToolStripMenuItem delete_mapping_ToolStripMenuItem1;

		private DataGridViewCheckBoxColumn activate_Col_mapping;

		private DataGridViewTextBoxColumn var_Col_mapping;

		private DataGridViewTextBoxColumn frontend_Col_mapping;

		private DataGridViewTextBoxColumn storage_Col_Mapping;

		private DataGridViewTextBoxColumn shown_Col_mapping;

		private DataGridViewTextBoxColumn required_Col_mapping;

		private DataGridViewTextBoxColumn storagetype_Col_mapping;

		private RichTextBox mapping_Info_richTextBox;

		private CheckBox anonymousRead_checkBox;

		private Panel step3_panel;

		private TextBox config_textBox;

		private Button saveConfig_button;

		private Button copyClipboard_button;

		private Button closeProgram_button;

		[DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "SendMessageA", ExactSpelling = true, SetLastError = true)]
		private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

		public Main()
		{
			this.InitializeComponent();
			this.currentStep_label.Location = this.step1_label.Location;
			SetDomain setDomain = new SetDomain();
			if (setDomain.ShowDialog() != DialogResult.OK)
			{
				Environment.Exit(0);
			}
			else
			{
				this.Host = this.settings.HostAddress;
				this.Username = this.settings.SeachUserDN;
				this.Password = this.settings.SearchUserPassword;
				this.Anonymous = this.settings.AnonymousAllowed;
			}
			this.fillMappingDataGridView();
		}

		private void nextStep_toolStripButton_MouseClick(object sender, EventArgs e)
		{
			this.hideStepPanels();
			this.nextStep();
		}

		private void nextStep()
		{
			if (this.currentStep <= 3)
			{
				this.currentStep++;
				switch (this.currentStep)
				{
				case 1:
					this.step1_panel.Visible = true;
					return;
				case 2:
					this.step2_panel.Visible = true;
					this.moveCurrentLocation(this.step1_label.Location.X, this.step2_label.Location.X);
					return;
				case 3:
					this.step3_panel.Visible = true;
					this.moveCurrentLocation(this.step2_label.Location.X, this.step3_label.Location.X);
					break;
				default:
					return;
				}
			}
		}

		private void previouStep()
		{
			if (this.currentStep != 1)
			{
				this.currentStep--;
				switch (this.currentStep)
				{
				case 1:
					this.step1_panel.Visible = true;
					this.moveCurrentLocation(this.step2_label.Location.X, this.step1_label.Location.X);
					return;
				case 2:
					this.step2_panel.Visible = true;
					this.moveCurrentLocation(this.step3_label.Location.X, this.step2_label.Location.X);
					break;
				case 3:
					break;
				default:
					return;
				}
			}
		}

		private void moveCurrentLocation(int pCurrPosX, int pFuturePosX)
		{
			this.changeEnabledStepButtons(false);
			while (pCurrPosX < pFuturePosX)
			{
				pCurrPosX++;
				this.currentStep_label.Location = new Point(pCurrPosX, this.currentStep_label.Location.Y);
				Thread.Sleep(1);
			}
			while (pCurrPosX > pFuturePosX)
			{
				pCurrPosX--;
				this.currentStep_label.Location = new Point(pCurrPosX, this.currentStep_label.Location.Y);
				Thread.Sleep(1);
			}
			this.changeEnabledStepButtons(true);
		}

		private void changeEnabledStepButtons(bool enabled)
		{
			foreach (ToolStripItem toolStripItem in this.upperBar_toolStrip.Items)
			{
				if (toolStripItem is ToolStripButton)
				{
					toolStripItem.Enabled = enabled;
				}
			}
		}

		private void previousStep_toolStripButton_Click(object sender, EventArgs e)
		{
			this.hideStepPanels();
			this.previouStep();
		}

		private void hideStepPanels()
		{
			this.step1_panel.Visible = false;
			this.step2_panel.Visible = false;
			this.step3_panel.Visible = false;
		}

		private void anonymousRead_checkBox_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.anonymousRead_checkBox.Location.X, this.anonymousRead_checkBox.Location.Y + this.anonymousRead_checkBox.Height);
			this.HelpReqCaption_label.Text = "Anonymous user permission";
			this.HelpReqText_textBox.Text = "Only check this box if you are sure that everyone can read your ldap tree. To check your settings launch ADSIEdit and navigate to CN=Directory Service, CN= Windows NT, CN = Services, CN= Configuration, DC=<domain name>, DC=com.\r\nRight-click the CN=Directory Service container, choose Properties from the Context menu, and scroll down to the dsHeuristics attribute and check if '0000002' is set.\r\nNOTE: To grant anonymous access you have to change the seventh character and fill the rest with zeros.\r\nE.g. 001 -> 0010002.";
			this.ChangeHelpReqSize();
		}

		private void sourcename_textBox_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.sourcename_textBox.Location.X, this.sourcename_textBox.Location.Y + this.sourcename_textBox.Height);
			this.HelpReqCaption_label.Text = "Sourcename";
			this.HelpReqText_textBox.Text = "The sourcename is showed in your OTRS customer module on the left side. It is most useful if you have multiple authentication sources e.g. another database or ldap-source";
			this.ChangeHelpReqSize();
		}

		private void host_label_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.host_comboBox.Location.X, this.host_comboBox.Location.Y + this.host_comboBox.Height + this.differenceHostearchuser_panel.Location.Y);
			this.HelpReqCaption_label.Text = "Host Address";
			this.HelpReqText_textBox.Text = "The Fully Qualified Domain Name(FQDN) is required to locate your Domain Controller in the DNS.\r\nIt contains the hostname as well as the domain\r\ne.g. something.domain.com";
			this.ChangeHelpReqSize();
		}

		private void searchUser_label_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.searchUser_textBox.Location.X, this.searchUser_textBox.Location.Y + this.searchUser_textBox.Height + this.differenceHostearchuser_panel.Location.Y);
			this.HelpReqCaption_label.Text = "Seach User DN";
			this.HelpReqText_textBox.Text = "Your qualified user to read the Active Directory. All the credentials are saved plain onto the config.pm! To avoid security issues create a user wihtout any rights beside reading the AD";
			this.ChangeHelpReqSize();
		}

		private void baseDN_textBox_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.baseDN_textBox.Location.X, this.baseDN_textBox.Location.Y + this.baseDN_textBox.Height + this.status_panel.Location.Y);
			this.HelpReqCaption_label.Text = "Base Distinguished  Name";
			this.HelpReqText_textBox.Text = "The BaseDN defines the location to search for objects. It represents an object in the hierarchically structured directory. There are three keywords\r\ncn:Common Name\r\nou:Organisational Unit\r\ndc: Domain Component";
			this.ChangeHelpReqSize();
		}

		private void baseDNScope_comboBox_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.baseDN_textBox.Location.X, this.baseDN_textBox.Location.Y + this.baseDN_textBox.Height + this.status_panel.Location.Y);
			this.HelpReqCaption_label.Text = "Scope";
			this.HelpReqText_textBox.Text = "BASE:\r\nThis value is used to indicate searching only the entry at the base DN, resulting in only that entry being returned (keeping in mind that it also has to meet the search filter criteria!).\r\nONE:\r\nThis value is used to indicate searching all entries one level under the base DN - but not including the base DN and not including any entries under that one level under the base DN.\r\nSUBTREE:\r\nThis value is used to indicate searching of all entries at all levels under and including the specified base DN.";
			this.ChangeHelpReqSize();
		}

		private void scopeHelp_button_Click(object sender, EventArgs e)
		{
			HelpEventArgs hlpevent = new HelpEventArgs(Cursor.Position);
			this.baseDNScope_comboBox_HelpRequested(sender, hlpevent);
		}

		private void filter_groupBox_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.baseDN_textBox.Location.X, this.baseDN_textBox.Location.Y + this.baseDN_textBox.Height + this.status_panel.Location.Y);
			this.HelpReqCaption_label.Text = "Filter";
			this.HelpReqText_textBox.Text = "Equalitiy:\t(attribute=abc) e.g. (&(objectclass=user)(displayName=Föckeler))\r\nDisparity:\t(!(attribute=abc)) e.g. (!objectClass=group)\r\nAvailability:(attribute=*) e.g. (mailNickName=*)\r\nAbsence:(!(attribute=*)) e.g. (!proxyAddresses=*)\r\nGreater-comparison:(attribute>=abc) e.g. (mdbStorageQuota>=100000)\r\nSmaller-comparison:(attribute<=abc) e.g. (mdbStorageQuota<=100000)\r\nWildcards: e.g. (sn=F*) or (mail=*@cerrotorre.de) or (givenName=*Paul*)";
			this.ChangeHelpReqSize();
		}

		private void HelpReqClose_button_Click(object sender, EventArgs e)
		{
			this.HelpReq_Panel.Visible = false;
		}

		private void HelpReq_Panel_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.PanelMouseDownLocation = e.Location;
			}
		}

		private void HelpReq_Panel_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.HelpReq_Panel.Left += e.X - this.PanelMouseDownLocation.X;
				this.HelpReq_Panel.Top += e.Y - this.PanelMouseDownLocation.Y;
			}
		}

		private void ChangeHelpReqSize()
		{
			int num = Main.SendMessage(this.HelpReqText_textBox.Handle.ToInt32(), 186, 0, 0);
			this.HelpReqText_textBox.Height = (this.HelpReqText_textBox.Font.Height + 2) * num;
			this.HelpReq_Panel.Height = this.HelpReqText_textBox.Height + this.HelpReqCaption_label.Height + 10;
		}

		private void anonymousRead_checkBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.anonymousRead_checkBox.CheckState == CheckState.Checked)
			{
				this.searchUser_textBox.Enabled = false;
				this.searchPassword_textBox.Enabled = false;
				return;
			}
			this.searchUser_textBox.Enabled = true;
			this.searchPassword_textBox.Enabled = true;
		}

		private void differenceHostSearchuser_checkBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.differenceHostSearchuser_checkBox.CheckState == CheckState.Checked)
			{
				this.differenceHostearchuser_panel.Visible = true;
				this.differenceHostearchuser_panel.Location = new Point(6, -this.differenceHostearchuser_panel.Size.Height);
				int num = -this.differenceHostearchuser_panel.Size.Height;
				while (this.differenceHostearchuser_panel.Location.Y < 54)
				{
					num++;
					this.differenceHostearchuser_panel.Location = new Point(6, num);
					Thread.Sleep(1);
				}
				this.status_panel.Location = new Point(6, this.status_panel.Location.Y + this.differenceHostearchuser_panel.Height + 2);
				return;
			}
			this.status_panel.Location = new Point(6, 54);
			this.differenceHostearchuser_panel.Visible = false;
		}

		private void step1Accept_button_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.sourcename_textBox.Text) || string.IsNullOrEmpty(this.baseDN_textBox.Text) || string.IsNullOrEmpty(this.scope_textBox.Text))
			{
				MessageBox.Show("Please fill in at least Sourcename, BaseDN and Scope", "Fill in...");
				return;
			}
			if (!this.differenceHostSearchuser_checkBox.Checked)
			{
				this.step1_panel.Visible = false;
				this.step1_label.BackColor = Color.Green;
				this.nextStep();
				return;
			}
			if (this.host_comboBox.Text != "" && this.searchUser_textBox.Text != "" && this.searchPassword_textBox.Text != "")
			{
				this.step1_label.BackColor = Color.Green;
				this.step1_panel.Visible = false;
				this.nextStep();
				return;
			}
			MessageBox.Show("Either choose to use the old credentials or fill in your host and new credentials");
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.filter_dataGridView.Rows.Add();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				this.filter_dataGridView.Rows.Remove(this.filter_dataGridView.Rows[this.filter_dataGridView.SelectedCells[0].RowIndex]);
			}
			catch
			{
			}
		}

		private void refreshBaseDNBrowser_button_Click(object sender, EventArgs e)
		{
			this.ctr_treeView.Nodes.Clear();
			if (this.differenceHostSearchuser_checkBox.CheckState == CheckState.Checked)
			{
				if (this.anonymousRead_checkBox.CheckState == CheckState.Unchecked)
				{
					this.Base = new DirectoryEntry("LDAP://" + this.host_comboBox.Text, this.searchUser_textBox.Text, this.searchPassword_textBox.Text);
				}
				else
				{
					this.Base = new DirectoryEntry("LDAP://" + this.host_comboBox.Text);
				}
			}
			else if (!this.settings.AnonymousAllowed)
			{
				this.Base = new DirectoryEntry("LDAP://" + this.settings.HostAddress, this.settings.SeachUserDN, this.settings.SearchUserPassword);
			}
			else
			{
				this.Base = new DirectoryEntry("LDAP://" + this.settings.HostAddress);
			}
			if (this.Base != null)
			{
				this.ctr_treeView.Nodes.Clear();
				this.ctr_treeView.BeginUpdate();
				try
				{
					TreeNode treeNode = this.ctr_treeView.Nodes.Add(this.Base.Name);
					treeNode.Tag = this.Base;
					try
					{
						foreach (DirectoryEntry directoryEntry in this.Base.Children)
						{
							TreeNode treeNode2 = treeNode.Nodes.Add(directoryEntry.Name);
							treeNode2.Tag = directoryEntry;
						}
					}
					finally
					{
						treeNode.Expand();
						this.ctr_treeView.EndUpdate();
					}
				}
				catch (Exception ex)
				{
					this.Base = new DirectoryEntry("LDAP://" + this.host_comboBox.Text, "", "");
					this.Base.AuthenticationType = AuthenticationTypes.Anonymous;
					MessageBox.Show(ex.Message, ex.Source);
				}
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
					this.ctr_listView.Clear();
					this.ctr_listView.Columns.Add("Attribute", 90, HorizontalAlignment.Left);
					this.ctr_listView.Columns.Add("Value", 350, HorizontalAlignment.Left);
					foreach (object current in directoryEntry3.Properties.PropertyNames)
					{
						foreach (object current2 in directoryEntry3.Properties[current.ToString()])
						{
							ListViewItem listViewItem = new ListViewItem(current.ToString(), 0);
							listViewItem.SubItems.Add(current2.ToString());
							this.ctr_listView.Items.AddRange(new ListViewItem[]
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

		private void setBaseDN_button_Click(object sender, EventArgs e)
		{
			ListViewItem listViewItem = this.ctr_listView.FindItemWithText("distinguishedName");
			try
			{
				this.baseDN_textBox.Text = this.ctr_listView.Items[listViewItem.Index].SubItems[1].Text;
				if (this.scope_comboBox.Text == "")
				{
					MessageBox.Show("Please fill in the SCOPE-Parameter");
				}
				else
				{
					this.scope_textBox.Text = this.scope_comboBox.Text;
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Please select one Organisation Unit", "Select...");
			}
		}

		private void GetDomainController_button_Click(object sender, EventArgs e)
		{
			this.host_comboBox.Items.Clear();
			foreach (string current in this.GetDCs())
			{
				this.host_comboBox.Items.Add(current);
			}
			this.host_comboBox.DroppedDown = true;
		}

		public List<string> GetDCs()
		{
			List<string> list = new List<string>();
			List<string> result;
			try
			{
				Domain computerDomain = Domain.GetComputerDomain();
				foreach (DomainController domainController in computerDomain.DomainControllers)
				{
					list.Add(domainController.Name);
				}
				result = list;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				result = list;
			}
			return result;
		}

		private void resetctr_treeView_button_Click(object sender, EventArgs e)
		{
			this.step1_panel.Controls.Remove(this.ctr_treeView);
			this.ctr_treeView.Dispose();
			this.CreateNewTreeView();
		}

		private void CreateNewTreeView()
		{
			this.ctr_treeView = new TreeView();
			this.getBaseDN_splitContainer.Panel1.Controls.Add(this.ctr_treeView);
			this.ctr_treeView.Location = new Point(0, 0);
			this.ctr_treeView.Dock = DockStyle.Fill;
		}

		private void fillMappingDataGridView()
		{
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				false,
				"UserSalutation",
				"Title",
				"title",
				1,
				0,
				"var"
			});
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				true,
				"UserFirstname",
				"Firstname",
				"givenname",
				1,
				1,
				"var"
			});
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				true,
				"UserLastname",
				"Lastname",
				"sn",
				1,
				1,
				"var"
			});
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				true,
				"UserLogin",
				"Login",
				"sAMAccountName",
				1,
				1,
				"var"
			});
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				true,
				"UserEmail",
				"Email",
				"mail",
				1,
				1,
				"var"
			});
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				true,
				"UserCustomerID",
				"CustomerID",
				"mail",
				0,
				1,
				"var"
			});
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				false,
				"UserPhone",
				"Phone",
				"telephonenumber",
				1,
				0,
				"var"
			});
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				false,
				"UserAddress",
				"Address",
				"postaladdress",
				1,
				0,
				"var"
			});
			this.mapping_dataGridView.Rows.Add(new object[]
			{
				false,
				"UserComment",
				"Comment",
				"description",
				1,
				0,
				"var"
			});
			foreach (DataGridViewRow dataGridViewRow in ((IEnumerable)this.mapping_dataGridView.Rows))
			{
				if (dataGridViewRow.Cells[5].Value.ToString() == "1")
				{
					dataGridViewRow.Cells[0].ReadOnly = true;
				}
			}
			this.mapping_Info_richTextBox.SelectionBackColor = Color.Black;
			string text = this.mapping_Info_richTextBox.Text;
			this.mapping_Info_richTextBox.Text = "";
			this.mapping_Info_richTextBox.SelectedText = text;
			this.mapping_Info_richTextBox.SelectionColor = Color.Red;
			this.mapping_Info_richTextBox.SelectedText = Environment.NewLine + "\r\n*may not be accurate or true\r\nIf you don't know what to do here, better do not modify";
		}

		private void step2Accept_button_Click(object sender, EventArgs e)
		{
			int num = 0;
			foreach (Control control in this.step2_panel.Controls)
			{
				if (control.Text == "")
				{
					num++;
				}
			}
			if (num > 0)
			{
				MessageBox.Show("Please fill every Box", "Fill in...");
				return;
			}
			this.step2_label.BackColor = Color.Green;
			this.step2_panel.Visible = false;
			this.GenerateFilterAndMapping();
			this.ReplaceConfigItems();
			this.nextStep();
		}

		private void ReplaceConfigItems()
		{
			this.config_textBox.Text = this.config_textBox.Text.Replace("%searchHostAuth%", this.settings.HostAddress);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%searchBaseDNAuth%", this.settings.BaseDNSearch);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%searchUIDAuth%", this.settings.UserAttributeSearch);
			if (!this.settings.AnonymousAllowed)
			{
				this.config_textBox.Text = this.config_textBox.Text.Replace("%searchUserAuth%", this.settings.SeachUserDN);
				this.config_textBox.Text = this.config_textBox.Text.Replace("%searchPWAuth%", this.settings.SearchUserPassword);
			}
			else
			{
				this.config_textBox.Text = this.config_textBox.Text.Replace("%searchUserAuth%", "");
				this.config_textBox.Text = this.config_textBox.Text.Replace("%searchPWAuth%", "");
			}
			this.config_textBox.Text = this.config_textBox.Text.Replace("%Sourcename%", this.sourcename_textBox.Text);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%BaseDN%", this.baseDN_textBox.Text);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%Scope%", this.scope_textBox.Text);
			if (this.differenceHostSearchuser_checkBox.CheckState == CheckState.Checked)
			{
				if (this.anonymousRead_checkBox.CheckState == CheckState.Checked)
				{
					this.config_textBox.Text = this.config_textBox.Text.Replace("%Host%", this.host_comboBox.Text);
					this.config_textBox.Text = this.config_textBox.Text.Replace("%UserDN%", this.searchUser_textBox.Text);
					this.config_textBox.Text = this.config_textBox.Text.Replace("%UserPW%", this.searchPassword_textBox.Text);
				}
				else
				{
					this.config_textBox.Text = this.config_textBox.Text.Replace("%Host%", "");
					this.config_textBox.Text = this.config_textBox.Text.Replace("%UserDN%", "");
					this.config_textBox.Text = this.config_textBox.Text.Replace("%UserPW%", "");
				}
			}
			else if (!this.settings.AnonymousAllowed)
			{
				this.config_textBox.Text = this.config_textBox.Text.Replace("%Host%", this.settings.HostAddress);
				this.config_textBox.Text = this.config_textBox.Text.Replace("%UserDN%", this.settings.SeachUserDN);
				this.config_textBox.Text = this.config_textBox.Text.Replace("%UserPW%", this.settings.SearchUserPassword);
			}
			else
			{
				this.config_textBox.Text = this.config_textBox.Text.Replace("%Host%", "");
				this.config_textBox.Text = this.config_textBox.Text.Replace("%UserDN%", "");
				this.config_textBox.Text = this.config_textBox.Text.Replace("%UserPW%", "");
			}
			this.config_textBox.Text = this.config_textBox.Text.Replace("%Filter%", this.strFilter);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%CustomerKey%", this.customerKey_comboBox.Text);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%CustomerID%", this.customerID_textBox.Text);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%CustomerUserListFields%", this.customerUserListFields_textBox.Text);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%CustomerUserSearchFields%", this.customerUserSearchFields_textBox.Text);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%CustomerUserPostMasterSearchFields%", this.customerUserPostMasterSearchFields_textBox.Text);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%CustomerUserNameFields%", this.customerUserNameFields_textBox.Text);
			this.config_textBox.Text = this.config_textBox.Text.Replace("%Mapping%", this.strMapping);
		}

		private void GenerateFilterAndMapping()
		{
			foreach (DataGridViewRow dataGridViewRow in ((IEnumerable)this.filter_dataGridView.Rows))
			{
				this.strFilter = this.strFilter + "(" + dataGridViewRow.Cells[0].Value;
				this.strFilter += dataGridViewRow.Cells[1].Value;
				this.strFilter = this.strFilter + dataGridViewRow.Cells[2].Value + ")";
			}
			foreach (DataGridViewRow dataGridViewRow2 in ((IEnumerable)this.mapping_dataGridView.Rows))
			{
				if (!(bool)dataGridViewRow2.Cells[0].Value)
				{
					this.strMapping += "#";
				}
				object obj = this.strMapping;
				this.strMapping = string.Concat(new object[]
				{
					obj,
					"['",
					dataGridViewRow2.Cells[1].Value,
					"', "
				});
				object obj2 = this.strMapping;
				this.strMapping = string.Concat(new object[]
				{
					obj2,
					"'",
					dataGridViewRow2.Cells[2].Value,
					"', "
				});
				object obj3 = this.strMapping;
				this.strMapping = string.Concat(new object[]
				{
					obj3,
					"'",
					dataGridViewRow2.Cells[3].Value,
					"', "
				});
				object obj4 = this.strMapping;
				this.strMapping = string.Concat(new object[]
				{
					obj4,
					"'",
					dataGridViewRow2.Cells[4].Value,
					"', "
				});
				object obj5 = this.strMapping;
				this.strMapping = string.Concat(new object[]
				{
					obj5,
					"'",
					dataGridViewRow2.Cells[5].Value,
					"', "
				});
				object obj6 = this.strMapping;
				this.strMapping = string.Concat(new object[]
				{
					obj6,
					"'",
					dataGridViewRow2.Cells[6].Value,
					"'], "
				});
				this.strMapping += Environment.NewLine;
			}
		}

		private void new_mapping_ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.mapping_dataGridView.Rows.Add();
		}

		private void delete_mapping_ToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (this.mapping_dataGridView.SelectedCells[0].RowIndex > 8)
			{
				this.mapping_dataGridView.Rows.RemoveAt(this.mapping_dataGridView.SelectedCells[0].RowIndex);
			}
		}

		private void mapping_dataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			this.mapping_dataGridView.CurrentCell = this.mapping_dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
		}

		private void closeProgram_button_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (MessageBox.Show("Do you really want to close the program?", "Close?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		private void copyClipboard_button_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(this.config_textBox.Text);
			MessageBox.Show("Config successfully loaded into clipboard");
		}

		private void saveConfig_button_Click(object sender, EventArgs e)
		{
			string text = this.config_textBox.Text;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Textfile (*.txt) |*.txt";
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName);
				streamWriter.Write(text);
				streamWriter.Close();
			}
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Main));
			this.upperBar_toolStrip = new ToolStrip();
			this.previousStep_toolStripButton = new ToolStripButton();
			this.nextStep_toolStripButton = new ToolStripButton();
			this.nextStep_toolStripButton2 = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.step1_label = new Label();
			this.step2_label = new Label();
			this.step3_label = new Label();
			this.currentStep_label = new Label();
			this.sourcename_label = new Label();
			this.sourcename_textBox = new TextBox();
			this.HelpReq_Panel = new Panel();
			this.HelpReqClose_button = new Button();
			this.HelpReqCaption_label = new Label();
			this.HelpReqText_textBox = new TextBox();
			this.step1_panel = new Panel();
			this.step1Accept_button = new Button();
			this.getBaseDN_groupBox = new GroupBox();
			this.resetctr_treeView_button = new Button();
			this.scopeHelp_button = new Button();
			this.scope_comboBox = new ComboBox();
			this.setBaseDN_button = new Button();
			this.getBaseDN_splitContainer = new SplitContainer();
			this.ctr_treeView = new TreeView();
			this.ctr_listView = new ListView();
			this.refreshBaseDNBrowser_button = new Button();
			this.status_panel = new Panel();
			this.scope_textBox = new TextBox();
			this.scope_label = new Label();
			this.filter_groupBox = new GroupBox();
			this.filter_dataGridView = new DataGridView();
			this.FilterAttribute = new DataGridViewTextBoxColumn();
			this.Filter_logicalOperator = new DataGridViewTextBoxColumn();
			this.Filter_Value = new DataGridViewTextBoxColumn();
			this.filter_dataGridView_contextMenuStrip = new ContextMenuStrip(this.components);
			this.new_filter_ToolStripMenuItem = new ToolStripMenuItem();
			this.delete_filter_ToolStripMenuItem = new ToolStripMenuItem();
			this.filterNote_textBox = new TextBox();
			this.baseDN_textBox = new TextBox();
			this.baseDN_label = new Label();
			this.differenceHostearchuser_panel = new Panel();
			this.anonymousRead_checkBox = new CheckBox();
			this.GetDomainController_button = new Button();
			this.host_comboBox = new ComboBox();
			this.searchPassword_textBox = new TextBox();
			this.searchUser_textBox = new TextBox();
			this.searchPassword_label = new Label();
			this.searchUser_label = new Label();
			this.host_label = new Label();
			this.differenceHostSearchuser_checkBox = new CheckBox();
			this.step2_panel = new Panel();
			this.mapping_Info_richTextBox = new RichTextBox();
			this.mapping_Info_label = new Label();
			this.mapping_groupBox = new GroupBox();
			this.mapping_dataGridView = new DataGridView();
			this.activate_Col_mapping = new DataGridViewCheckBoxColumn();
			this.var_Col_mapping = new DataGridViewTextBoxColumn();
			this.frontend_Col_mapping = new DataGridViewTextBoxColumn();
			this.storage_Col_Mapping = new DataGridViewTextBoxColumn();
			this.shown_Col_mapping = new DataGridViewTextBoxColumn();
			this.required_Col_mapping = new DataGridViewTextBoxColumn();
			this.storagetype_Col_mapping = new DataGridViewTextBoxColumn();
			this.mapping_dataGridView_contextMenuStrip = new ContextMenuStrip(this.components);
			this.new_mapping_ToolStripMenuItem = new ToolStripMenuItem();
			this.delete_mapping_ToolStripMenuItem1 = new ToolStripMenuItem();
			this.step2Accept_button = new Button();
			this.customerUserNameFields_Info_label = new Label();
			this.customerUserPostMasterSearchFields_Info_label = new Label();
			this.customerUserSearchFields_Info_label = new Label();
			this.customerUserListFields_Info_label = new Label();
			this.customerUserPostMasterSearchFields_textBox = new TextBox();
			this.customerUserNameFields_textBox = new TextBox();
			this.customerUserSearchFields_textBox = new TextBox();
			this.customerUserListFields_textBox = new TextBox();
			this.customerUserNameFields_label = new Label();
			this.customerUserPostMasterSearchFields_label = new Label();
			this.customerUserSearchFields_label = new Label();
			this.customerUserListFields_label = new Label();
			this.customerKey_comboBox = new ComboBox();
			this.customerID_Info_label = new Label();
			this.customerKey_Info_label = new Label();
			this.customerID_textBox = new TextBox();
			this.customerID_label = new Label();
			this.customerKey_label = new Label();
			this.step3_panel = new Panel();
			this.copyClipboard_button = new Button();
			this.closeProgram_button = new Button();
			this.saveConfig_button = new Button();
			this.config_textBox = new TextBox();
			this.upperBar_toolStrip.SuspendLayout();
			this.HelpReq_Panel.SuspendLayout();
			this.step1_panel.SuspendLayout();
			this.getBaseDN_groupBox.SuspendLayout();
			this.getBaseDN_splitContainer.Panel1.SuspendLayout();
			this.getBaseDN_splitContainer.Panel2.SuspendLayout();
			this.getBaseDN_splitContainer.SuspendLayout();
			this.status_panel.SuspendLayout();
			this.filter_groupBox.SuspendLayout();
			((ISupportInitialize)this.filter_dataGridView).BeginInit();
			this.filter_dataGridView_contextMenuStrip.SuspendLayout();
			this.differenceHostearchuser_panel.SuspendLayout();
			this.step2_panel.SuspendLayout();
			this.mapping_groupBox.SuspendLayout();
			((ISupportInitialize)this.mapping_dataGridView).BeginInit();
			this.mapping_dataGridView_contextMenuStrip.SuspendLayout();
			this.step3_panel.SuspendLayout();
			base.SuspendLayout();
			this.upperBar_toolStrip.BackColor = SystemColors.ControlLight;
			this.upperBar_toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			this.upperBar_toolStrip.Items.AddRange(new ToolStripItem[]
			{
				this.previousStep_toolStripButton,
				this.nextStep_toolStripButton,
				this.nextStep_toolStripButton2,
				this.toolStripSeparator1,
				this.toolStripSeparator2
			});
			this.upperBar_toolStrip.Location = new Point(0, 0);
			this.upperBar_toolStrip.Name = "upperBar_toolStrip";
			this.upperBar_toolStrip.Size = new Size(831, 25);
			this.upperBar_toolStrip.TabIndex = 0;
			this.upperBar_toolStrip.Text = "toolStrip1";
			this.previousStep_toolStripButton.Image = Resources.Previous;
			this.previousStep_toolStripButton.ImageTransparentColor = Color.Magenta;
			this.previousStep_toolStripButton.Name = "previousStep_toolStripButton";
			this.previousStep_toolStripButton.Size = new Size(100, 22);
			this.previousStep_toolStripButton.Text = ":Previous step";
			this.previousStep_toolStripButton.ToolTipText = "One step back";
			this.previousStep_toolStripButton.Click += new EventHandler(this.previousStep_toolStripButton_Click);
			this.nextStep_toolStripButton.Alignment = ToolStripItemAlignment.Right;
			this.nextStep_toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.nextStep_toolStripButton.Image = Resources.Next;
			this.nextStep_toolStripButton.ImageTransparentColor = Color.Magenta;
			this.nextStep_toolStripButton.Name = "nextStep_toolStripButton";
			this.nextStep_toolStripButton.Size = new Size(23, 22);
			this.nextStep_toolStripButton.Text = "Next step";
			this.nextStep_toolStripButton.ToolTipText = "One step forward";
			this.nextStep_toolStripButton.Click += new EventHandler(this.nextStep_toolStripButton_MouseClick);
			this.nextStep_toolStripButton2.Alignment = ToolStripItemAlignment.Right;
			this.nextStep_toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.nextStep_toolStripButton2.Image = (Image)componentResourceManager.GetObject("nextStep_toolStripButton2.Image");
			this.nextStep_toolStripButton2.ImageTransparentColor = Color.Magenta;
			this.nextStep_toolStripButton2.Name = "nextStep_toolStripButton2";
			this.nextStep_toolStripButton2.Size = new Size(63, 22);
			this.nextStep_toolStripButton2.Text = "Next step:";
			this.nextStep_toolStripButton2.ToolTipText = "One step forward";
			this.nextStep_toolStripButton2.Click += new EventHandler(this.nextStep_toolStripButton_MouseClick);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(6, 25);
			this.toolStripSeparator2.Alignment = ToolStripItemAlignment.Right;
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(6, 25);
			this.step1_label.AutoSize = true;
			this.step1_label.BackColor = Color.Red;
			this.step1_label.Location = new Point(116, 7);
			this.step1_label.Name = "step1_label";
			this.step1_label.Size = new Size(13, 13);
			this.step1_label.TabIndex = 1;
			this.step1_label.Text = "1";
			this.step2_label.AutoSize = true;
			this.step2_label.BackColor = Color.Red;
			this.step2_label.Location = new Point(398, 7);
			this.step2_label.Name = "step2_label";
			this.step2_label.Size = new Size(13, 13);
			this.step2_label.TabIndex = 2;
			this.step2_label.Text = "2";
			this.step3_label.AutoSize = true;
			this.step3_label.BackColor = Color.Red;
			this.step3_label.Location = new Point(716, 7);
			this.step3_label.Name = "step3_label";
			this.step3_label.Size = new Size(13, 13);
			this.step3_label.TabIndex = 3;
			this.step3_label.Text = "3";
			this.currentStep_label.AutoSize = true;
			this.currentStep_label.BackColor = Color.Yellow;
			this.currentStep_label.Location = new Point(166, 7);
			this.currentStep_label.Name = "currentStep_label";
			this.currentStep_label.Size = new Size(14, 13);
			this.currentStep_label.TabIndex = 4;
			this.currentStep_label.Text = "X";
			this.sourcename_label.AutoSize = true;
			this.sourcename_label.Location = new Point(3, 8);
			this.sourcename_label.Name = "sourcename_label";
			this.sourcename_label.Size = new Size(70, 13);
			this.sourcename_label.TabIndex = 0;
			this.sourcename_label.Text = "Sourcename:";
			this.sourcename_label.HelpRequested += new HelpEventHandler(this.sourcename_textBox_HelpRequested);
			this.sourcename_textBox.Location = new Point(76, 5);
			this.sourcename_textBox.Name = "sourcename_textBox";
			this.sourcename_textBox.Size = new Size(125, 20);
			this.sourcename_textBox.TabIndex = 2;
			this.sourcename_textBox.HelpRequested += new HelpEventHandler(this.sourcename_textBox_HelpRequested);
			this.HelpReq_Panel.BackColor = Color.Gold;
			this.HelpReq_Panel.Controls.Add(this.HelpReqClose_button);
			this.HelpReq_Panel.Controls.Add(this.HelpReqCaption_label);
			this.HelpReq_Panel.Controls.Add(this.HelpReqText_textBox);
			this.HelpReq_Panel.Location = new Point(0, 457);
			this.HelpReq_Panel.Name = "HelpReq_Panel";
			this.HelpReq_Panel.Size = new Size(282, 238);
			this.HelpReq_Panel.TabIndex = 19;
			this.HelpReq_Panel.Visible = false;
			this.HelpReq_Panel.MouseDown += new MouseEventHandler(this.HelpReq_Panel_MouseDown);
			this.HelpReq_Panel.MouseMove += new MouseEventHandler(this.HelpReq_Panel_MouseMove);
			this.HelpReqClose_button.BackColor = Color.Silver;
			this.HelpReqClose_button.FlatStyle = FlatStyle.Popup;
			this.HelpReqClose_button.Location = new Point(247, 1);
			this.HelpReqClose_button.Name = "HelpReqClose_button";
			this.HelpReqClose_button.Size = new Size(32, 20);
			this.HelpReqClose_button.TabIndex = 2;
			this.HelpReqClose_button.Text = "X";
			this.HelpReqClose_button.UseVisualStyleBackColor = false;
			this.HelpReqClose_button.Click += new EventHandler(this.HelpReqClose_button_Click);
			this.HelpReqCaption_label.AutoSize = true;
			this.HelpReqCaption_label.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.HelpReqCaption_label.Location = new Point(3, 5);
			this.HelpReqCaption_label.Name = "HelpReqCaption_label";
			this.HelpReqCaption_label.Size = new Size(41, 13);
			this.HelpReqCaption_label.TabIndex = 1;
			this.HelpReqCaption_label.Text = "label1";
			this.HelpReqText_textBox.BackColor = Color.Gold;
			this.HelpReqText_textBox.Cursor = Cursors.Arrow;
			this.HelpReqText_textBox.Location = new Point(3, 21);
			this.HelpReqText_textBox.Multiline = true;
			this.HelpReqText_textBox.Name = "HelpReqText_textBox";
			this.HelpReqText_textBox.ReadOnly = true;
			this.HelpReqText_textBox.Size = new Size(276, 214);
			this.HelpReqText_textBox.TabIndex = 0;
			this.step1_panel.BackColor = SystemColors.Control;
			this.step1_panel.BorderStyle = BorderStyle.FixedSingle;
			this.step1_panel.Controls.Add(this.step1Accept_button);
			this.step1_panel.Controls.Add(this.getBaseDN_groupBox);
			this.step1_panel.Controls.Add(this.status_panel);
			this.step1_panel.Controls.Add(this.differenceHostearchuser_panel);
			this.step1_panel.Controls.Add(this.differenceHostSearchuser_checkBox);
			this.step1_panel.Controls.Add(this.sourcename_textBox);
			this.step1_panel.Controls.Add(this.sourcename_label);
			this.step1_panel.Location = new Point(12, 28);
			this.step1_panel.Name = "step1_panel";
			this.step1_panel.Size = new Size(807, 500);
			this.step1_panel.TabIndex = 5;
			this.step1Accept_button.Location = new Point(721, 472);
			this.step1Accept_button.Name = "step1Accept_button";
			this.step1Accept_button.Size = new Size(75, 23);
			this.step1Accept_button.TabIndex = 24;
			this.step1Accept_button.Text = "OK/Next";
			this.step1Accept_button.UseVisualStyleBackColor = true;
			this.step1Accept_button.Click += new EventHandler(this.step1Accept_button_Click);
			this.getBaseDN_groupBox.Controls.Add(this.resetctr_treeView_button);
			this.getBaseDN_groupBox.Controls.Add(this.scopeHelp_button);
			this.getBaseDN_groupBox.Controls.Add(this.scope_comboBox);
			this.getBaseDN_groupBox.Controls.Add(this.setBaseDN_button);
			this.getBaseDN_groupBox.Controls.Add(this.getBaseDN_splitContainer);
			this.getBaseDN_groupBox.Controls.Add(this.refreshBaseDNBrowser_button);
			this.getBaseDN_groupBox.Location = new Point(259, 5);
			this.getBaseDN_groupBox.Name = "getBaseDN_groupBox";
			this.getBaseDN_groupBox.Size = new Size(543, 464);
			this.getBaseDN_groupBox.TabIndex = 23;
			this.getBaseDN_groupBox.TabStop = false;
			this.getBaseDN_groupBox.Text = "BaseDN";
			this.resetctr_treeView_button.Location = new Point(87, 19);
			this.resetctr_treeView_button.Name = "resetctr_treeView_button";
			this.resetctr_treeView_button.Size = new Size(99, 23);
			this.resetctr_treeView_button.TabIndex = 5;
			this.resetctr_treeView_button.TabStop = false;
			this.resetctr_treeView_button.Text = "Reset left View";
			this.resetctr_treeView_button.UseVisualStyleBackColor = true;
			this.resetctr_treeView_button.Click += new EventHandler(this.resetctr_treeView_button_Click);
			this.scopeHelp_button.FlatStyle = FlatStyle.Flat;
			this.scopeHelp_button.Location = new Point(325, 440);
			this.scopeHelp_button.Name = "scopeHelp_button";
			this.scopeHelp_button.Size = new Size(23, 21);
			this.scopeHelp_button.TabIndex = 4;
			this.scopeHelp_button.Text = "?";
			this.scopeHelp_button.UseVisualStyleBackColor = true;
			this.scopeHelp_button.Click += new EventHandler(this.scopeHelp_button_Click);
			this.scope_comboBox.FormattingEnabled = true;
			this.scope_comboBox.Items.AddRange(new object[]
			{
				"base",
				"one",
				"sub"
			});
			this.scope_comboBox.Location = new Point(354, 440);
			this.scope_comboBox.Name = "scope_comboBox";
			this.scope_comboBox.Size = new Size(101, 21);
			this.scope_comboBox.TabIndex = 3;
			this.scope_comboBox.HelpRequested += new HelpEventHandler(this.baseDNScope_comboBox_HelpRequested);
			this.setBaseDN_button.Location = new Point(461, 438);
			this.setBaseDN_button.Name = "setBaseDN_button";
			this.setBaseDN_button.Size = new Size(75, 23);
			this.setBaseDN_button.TabIndex = 2;
			this.setBaseDN_button.Text = "Set BaseDN";
			this.setBaseDN_button.UseVisualStyleBackColor = true;
			this.setBaseDN_button.Click += new EventHandler(this.setBaseDN_button_Click);
			this.getBaseDN_splitContainer.BorderStyle = BorderStyle.FixedSingle;
			this.getBaseDN_splitContainer.Location = new Point(6, 48);
			this.getBaseDN_splitContainer.Name = "getBaseDN_splitContainer";
			this.getBaseDN_splitContainer.Panel1.Controls.Add(this.ctr_treeView);
			this.getBaseDN_splitContainer.Panel2.Controls.Add(this.ctr_listView);
			this.getBaseDN_splitContainer.Size = new Size(531, 390);
			this.getBaseDN_splitContainer.SplitterDistance = 205;
			this.getBaseDN_splitContainer.TabIndex = 1;
			this.ctr_treeView.Dock = DockStyle.Fill;
			this.ctr_treeView.Location = new Point(0, 0);
			this.ctr_treeView.Name = "ctr_treeView";
			this.ctr_treeView.Size = new Size(203, 388);
			this.ctr_treeView.TabIndex = 1;
			this.ctr_treeView.AfterSelect += new TreeViewEventHandler(this.ctr_tree_AfterSelect);
			this.ctr_listView.Dock = DockStyle.Fill;
			this.ctr_listView.Location = new Point(0, 0);
			this.ctr_listView.Name = "ctr_listView";
			this.ctr_listView.Size = new Size(320, 388);
			this.ctr_listView.TabIndex = 3;
			this.ctr_listView.TabStop = false;
			this.ctr_listView.UseCompatibleStateImageBehavior = false;
			this.ctr_listView.View = View.Details;
			this.refreshBaseDNBrowser_button.Location = new Point(6, 19);
			this.refreshBaseDNBrowser_button.Name = "refreshBaseDNBrowser_button";
			this.refreshBaseDNBrowser_button.Size = new Size(75, 23);
			this.refreshBaseDNBrowser_button.TabIndex = 0;
			this.refreshBaseDNBrowser_button.Text = "Refresh";
			this.refreshBaseDNBrowser_button.UseVisualStyleBackColor = true;
			this.refreshBaseDNBrowser_button.Click += new EventHandler(this.refreshBaseDNBrowser_button_Click);
			this.status_panel.BorderStyle = BorderStyle.FixedSingle;
			this.status_panel.Controls.Add(this.scope_textBox);
			this.status_panel.Controls.Add(this.scope_label);
			this.status_panel.Controls.Add(this.filter_groupBox);
			this.status_panel.Controls.Add(this.baseDN_textBox);
			this.status_panel.Controls.Add(this.baseDN_label);
			this.status_panel.Location = new Point(6, 54);
			this.status_panel.Name = "status_panel";
			this.status_panel.Size = new Size(247, 279);
			this.status_panel.TabIndex = 22;
			this.scope_textBox.Location = new Point(59, 28);
			this.scope_textBox.Name = "scope_textBox";
			this.scope_textBox.Size = new Size(170, 20);
			this.scope_textBox.TabIndex = 4;
			this.scope_label.AutoSize = true;
			this.scope_label.Location = new Point(9, 32);
			this.scope_label.Name = "scope_label";
			this.scope_label.Size = new Size(41, 13);
			this.scope_label.TabIndex = 3;
			this.scope_label.Text = "Scope:";
			this.filter_groupBox.Controls.Add(this.filter_dataGridView);
			this.filter_groupBox.Controls.Add(this.filterNote_textBox);
			this.filter_groupBox.Location = new Point(6, 54);
			this.filter_groupBox.Name = "filter_groupBox";
			this.filter_groupBox.Size = new Size(236, 220);
			this.filter_groupBox.TabIndex = 2;
			this.filter_groupBox.TabStop = false;
			this.filter_groupBox.Text = "Filter";
			this.filter_groupBox.HelpRequested += new HelpEventHandler(this.filter_groupBox_HelpRequested);
			this.filter_dataGridView.AllowUserToAddRows = false;
			this.filter_dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.filter_dataGridView.BorderStyle = BorderStyle.None;
			this.filter_dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.filter_dataGridView.Columns.AddRange(new DataGridViewColumn[]
			{
				this.FilterAttribute,
				this.Filter_logicalOperator,
				this.Filter_Value
			});
			this.filter_dataGridView.ContextMenuStrip = this.filter_dataGridView_contextMenuStrip;
			this.filter_dataGridView.Location = new Point(6, 88);
			this.filter_dataGridView.Name = "filter_dataGridView";
			this.filter_dataGridView.RowHeadersVisible = false;
			this.filter_dataGridView.Size = new Size(224, 126);
			this.filter_dataGridView.TabIndex = 1;
			this.filter_dataGridView.HelpRequested += new HelpEventHandler(this.filter_groupBox_HelpRequested);
			this.FilterAttribute.HeaderText = "Attribute";
			this.FilterAttribute.Name = "FilterAttribute";
			this.Filter_logicalOperator.HeaderText = "Logical operator(=,<...)";
			this.Filter_logicalOperator.Name = "Filter_logicalOperator";
			this.Filter_Value.HeaderText = "Value";
			this.Filter_Value.Name = "Filter_Value";
			this.filter_dataGridView_contextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				this.new_filter_ToolStripMenuItem,
				this.delete_filter_ToolStripMenuItem
			});
			this.filter_dataGridView_contextMenuStrip.Name = "filter_dataGridView_contextMenuStrip";
			this.filter_dataGridView_contextMenuStrip.Size = new Size(108, 48);
			this.new_filter_ToolStripMenuItem.Name = "new_filter_ToolStripMenuItem";
			this.new_filter_ToolStripMenuItem.Size = new Size(107, 22);
			this.new_filter_ToolStripMenuItem.Text = "New";
			this.new_filter_ToolStripMenuItem.Click += new EventHandler(this.newToolStripMenuItem_Click);
			this.delete_filter_ToolStripMenuItem.Name = "delete_filter_ToolStripMenuItem";
			this.delete_filter_ToolStripMenuItem.Size = new Size(107, 22);
			this.delete_filter_ToolStripMenuItem.Text = "Delete";
			this.delete_filter_ToolStripMenuItem.Click += new EventHandler(this.deleteToolStripMenuItem_Click);
			this.filterNote_textBox.Enabled = false;
			this.filterNote_textBox.Location = new Point(6, 17);
			this.filterNote_textBox.Multiline = true;
			this.filterNote_textBox.Name = "filterNote_textBox";
			this.filterNote_textBox.Size = new Size(224, 65);
			this.filterNote_textBox.TabIndex = 0;
			this.filterNote_textBox.Text = "In case you want to add always one filter to each ldap query, use this option. e. g. AlwaysFilter => '(mail=*)' or AlwaysFilter => '(objectclass=user)'";
			this.filterNote_textBox.HelpRequested += new HelpEventHandler(this.filter_groupBox_HelpRequested);
			this.baseDN_textBox.Location = new Point(59, 6);
			this.baseDN_textBox.Name = "baseDN_textBox";
			this.baseDN_textBox.Size = new Size(170, 20);
			this.baseDN_textBox.TabIndex = 1;
			this.baseDN_textBox.HelpRequested += new HelpEventHandler(this.baseDN_textBox_HelpRequested);
			this.baseDN_label.AutoSize = true;
			this.baseDN_label.Location = new Point(3, 9);
			this.baseDN_label.Name = "baseDN_label";
			this.baseDN_label.Size = new Size(50, 13);
			this.baseDN_label.TabIndex = 0;
			this.baseDN_label.Text = "BaseDN:";
			this.baseDN_label.HelpRequested += new HelpEventHandler(this.baseDN_textBox_HelpRequested);
			this.differenceHostearchuser_panel.BorderStyle = BorderStyle.FixedSingle;
			this.differenceHostearchuser_panel.Controls.Add(this.anonymousRead_checkBox);
			this.differenceHostearchuser_panel.Controls.Add(this.GetDomainController_button);
			this.differenceHostearchuser_panel.Controls.Add(this.host_comboBox);
			this.differenceHostearchuser_panel.Controls.Add(this.searchPassword_textBox);
			this.differenceHostearchuser_panel.Controls.Add(this.searchUser_textBox);
			this.differenceHostearchuser_panel.Controls.Add(this.searchPassword_label);
			this.differenceHostearchuser_panel.Controls.Add(this.searchUser_label);
			this.differenceHostearchuser_panel.Controls.Add(this.host_label);
			this.differenceHostearchuser_panel.Location = new Point(6, 54);
			this.differenceHostearchuser_panel.Name = "differenceHostearchuser_panel";
			this.differenceHostearchuser_panel.Size = new Size(247, 104);
			this.differenceHostearchuser_panel.TabIndex = 21;
			this.differenceHostearchuser_panel.Visible = false;
			this.anonymousRead_checkBox.AutoSize = true;
			this.anonymousRead_checkBox.Location = new Point(6, 28);
			this.anonymousRead_checkBox.Name = "anonymousRead_checkBox";
			this.anonymousRead_checkBox.Size = new Size(158, 17);
			this.anonymousRead_checkBox.TabIndex = 24;
			this.anonymousRead_checkBox.Text = "Anonymous reading allowed";
			this.anonymousRead_checkBox.UseVisualStyleBackColor = true;
			this.anonymousRead_checkBox.CheckedChanged += new EventHandler(this.anonymousRead_checkBox_CheckedChanged);
			this.anonymousRead_checkBox.HelpRequested += new HelpEventHandler(this.anonymousRead_checkBox_HelpRequested);
			this.GetDomainController_button.FlatStyle = FlatStyle.Popup;
			this.GetDomainController_button.Image = Resources.Search;
			this.GetDomainController_button.Location = new Point(200, 3);
			this.GetDomainController_button.Name = "GetDomainController_button";
			this.GetDomainController_button.RightToLeft = RightToLeft.Yes;
			this.GetDomainController_button.Size = new Size(29, 21);
			this.GetDomainController_button.TabIndex = 23;
			this.GetDomainController_button.UseVisualStyleBackColor = true;
			this.GetDomainController_button.Click += new EventHandler(this.GetDomainController_button_Click);
			this.host_comboBox.FormattingEnabled = true;
			this.host_comboBox.Location = new Point(65, 3);
			this.host_comboBox.Name = "host_comboBox";
			this.host_comboBox.Size = new Size(129, 21);
			this.host_comboBox.TabIndex = 6;
			this.searchPassword_textBox.Location = new Point(65, 76);
			this.searchPassword_textBox.Name = "searchPassword_textBox";
			this.searchPassword_textBox.PasswordChar = '*';
			this.searchPassword_textBox.Size = new Size(164, 20);
			this.searchPassword_textBox.TabIndex = 5;
			this.searchUser_textBox.Location = new Point(65, 51);
			this.searchUser_textBox.Name = "searchUser_textBox";
			this.searchUser_textBox.Size = new Size(164, 20);
			this.searchUser_textBox.TabIndex = 4;
			this.searchUser_textBox.HelpRequested += new HelpEventHandler(this.searchUser_label_HelpRequested);
			this.searchPassword_label.AutoSize = true;
			this.searchPassword_label.Location = new Point(3, 79);
			this.searchPassword_label.Name = "searchPassword_label";
			this.searchPassword_label.Size = new Size(56, 13);
			this.searchPassword_label.TabIndex = 2;
			this.searchPassword_label.Text = "Password:";
			this.searchUser_label.AutoSize = true;
			this.searchUser_label.Location = new Point(3, 54);
			this.searchUser_label.Name = "searchUser_label";
			this.searchUser_label.Size = new Size(32, 13);
			this.searchUser_label.TabIndex = 1;
			this.searchUser_label.Text = "User:";
			this.searchUser_label.HelpRequested += new HelpEventHandler(this.searchUser_label_HelpRequested);
			this.host_label.AutoSize = true;
			this.host_label.Location = new Point(3, 6);
			this.host_label.Name = "host_label";
			this.host_label.Size = new Size(32, 13);
			this.host_label.TabIndex = 0;
			this.host_label.Text = "Host:";
			this.host_label.HelpRequested += new HelpEventHandler(this.host_label_HelpRequested);
			this.differenceHostSearchuser_checkBox.AutoSize = true;
			this.differenceHostSearchuser_checkBox.Location = new Point(6, 31);
			this.differenceHostSearchuser_checkBox.Name = "differenceHostSearchuser_checkBox";
			this.differenceHostSearchuser_checkBox.Size = new Size(247, 17);
			this.differenceHostSearchuser_checkBox.TabIndex = 20;
			this.differenceHostSearchuser_checkBox.Text = "Host or searchuser differ from the formerly ones";
			this.differenceHostSearchuser_checkBox.UseVisualStyleBackColor = true;
			this.differenceHostSearchuser_checkBox.CheckedChanged += new EventHandler(this.differenceHostSearchuser_checkBox_CheckedChanged);
			this.step2_panel.BackColor = SystemColors.Control;
			this.step2_panel.Controls.Add(this.mapping_Info_richTextBox);
			this.step2_panel.Controls.Add(this.mapping_Info_label);
			this.step2_panel.Controls.Add(this.mapping_groupBox);
			this.step2_panel.Controls.Add(this.step2Accept_button);
			this.step2_panel.Controls.Add(this.customerUserNameFields_Info_label);
			this.step2_panel.Controls.Add(this.customerUserPostMasterSearchFields_Info_label);
			this.step2_panel.Controls.Add(this.customerUserSearchFields_Info_label);
			this.step2_panel.Controls.Add(this.customerUserListFields_Info_label);
			this.step2_panel.Controls.Add(this.customerUserPostMasterSearchFields_textBox);
			this.step2_panel.Controls.Add(this.customerUserNameFields_textBox);
			this.step2_panel.Controls.Add(this.customerUserSearchFields_textBox);
			this.step2_panel.Controls.Add(this.customerUserListFields_textBox);
			this.step2_panel.Controls.Add(this.customerUserNameFields_label);
			this.step2_panel.Controls.Add(this.customerUserPostMasterSearchFields_label);
			this.step2_panel.Controls.Add(this.customerUserSearchFields_label);
			this.step2_panel.Controls.Add(this.customerUserListFields_label);
			this.step2_panel.Controls.Add(this.customerKey_comboBox);
			this.step2_panel.Controls.Add(this.customerID_Info_label);
			this.step2_panel.Controls.Add(this.customerKey_Info_label);
			this.step2_panel.Controls.Add(this.customerID_textBox);
			this.step2_panel.Controls.Add(this.customerID_label);
			this.step2_panel.Controls.Add(this.customerKey_label);
			this.step2_panel.Location = new Point(12, 28);
			this.step2_panel.Name = "step2_panel";
			this.step2_panel.Size = new Size(807, 499);
			this.step2_panel.TabIndex = 20;
			this.step2_panel.Visible = false;
			this.mapping_Info_richTextBox.Location = new Point(391, 320);
			this.mapping_Info_richTextBox.Name = "mapping_Info_richTextBox";
			this.mapping_Info_richTextBox.ReadOnly = true;
			this.mapping_Info_richTextBox.Size = new Size(382, 135);
			this.mapping_Info_richTextBox.TabIndex = 22;
			this.mapping_Info_richTextBox.Text = componentResourceManager.GetString("mapping_Info_richTextBox.Text");
			this.mapping_Info_label.AutoSize = true;
			this.mapping_Info_label.Location = new Point(388, 230);
			this.mapping_Info_label.Name = "mapping_Info_label";
			this.mapping_Info_label.Size = new Size(414, 208);
			this.mapping_Info_label.TabIndex = 21;
			this.mapping_Info_label.Text = componentResourceManager.GetString("mapping_Info_label.Text");
			this.mapping_groupBox.Controls.Add(this.mapping_dataGridView);
			this.mapping_groupBox.Location = new Point(14, 211);
			this.mapping_groupBox.Name = "mapping_groupBox";
			this.mapping_groupBox.Size = new Size(368, 273);
			this.mapping_groupBox.TabIndex = 20;
			this.mapping_groupBox.TabStop = false;
			this.mapping_groupBox.Text = "Mapping:";
			this.mapping_dataGridView.AllowUserToAddRows = false;
			this.mapping_dataGridView.AllowUserToDeleteRows = false;
			this.mapping_dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			this.mapping_dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.mapping_dataGridView.Columns.AddRange(new DataGridViewColumn[]
			{
				this.activate_Col_mapping,
				this.var_Col_mapping,
				this.frontend_Col_mapping,
				this.storage_Col_Mapping,
				this.shown_Col_mapping,
				this.required_Col_mapping,
				this.storagetype_Col_mapping
			});
			this.mapping_dataGridView.ContextMenuStrip = this.mapping_dataGridView_contextMenuStrip;
			this.mapping_dataGridView.Location = new Point(6, 19);
			this.mapping_dataGridView.MultiSelect = false;
			this.mapping_dataGridView.Name = "mapping_dataGridView";
			this.mapping_dataGridView.RowHeadersVisible = false;
			this.mapping_dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
			this.mapping_dataGridView.Size = new Size(356, 248);
			this.mapping_dataGridView.TabIndex = 0;
			this.mapping_dataGridView.CellMouseDown += new DataGridViewCellMouseEventHandler(this.mapping_dataGridView_CellMouseDown);
			this.activate_Col_mapping.HeaderText = "Activate";
			this.activate_Col_mapping.Name = "activate_Col_mapping";
			this.activate_Col_mapping.Width = 52;
			this.var_Col_mapping.HeaderText = "Var";
			this.var_Col_mapping.Name = "var_Col_mapping";
			this.var_Col_mapping.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.var_Col_mapping.Width = 29;
			this.frontend_Col_mapping.HeaderText = "Frontend";
			this.frontend_Col_mapping.Name = "frontend_Col_mapping";
			this.frontend_Col_mapping.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.frontend_Col_mapping.Width = 55;
			this.storage_Col_Mapping.HeaderText = "Storage";
			this.storage_Col_Mapping.Name = "storage_Col_Mapping";
			this.storage_Col_Mapping.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.storage_Col_Mapping.Width = 50;
			this.shown_Col_mapping.HeaderText = "Shown";
			this.shown_Col_mapping.Name = "shown_Col_mapping";
			this.shown_Col_mapping.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.shown_Col_mapping.Width = 46;
			this.required_Col_mapping.HeaderText = "Required";
			this.required_Col_mapping.Name = "required_Col_mapping";
			this.required_Col_mapping.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.required_Col_mapping.Width = 56;
			this.storagetype_Col_mapping.HeaderText = "Storage-type";
			this.storagetype_Col_mapping.Name = "storagetype_Col_mapping";
			this.storagetype_Col_mapping.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.storagetype_Col_mapping.Width = 73;
			this.mapping_dataGridView_contextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				this.new_mapping_ToolStripMenuItem,
				this.delete_mapping_ToolStripMenuItem1
			});
			this.mapping_dataGridView_contextMenuStrip.Name = "mapping_dataGridView_contextMenuStrip";
			this.mapping_dataGridView_contextMenuStrip.Size = new Size(108, 48);
			this.new_mapping_ToolStripMenuItem.Name = "new_mapping_ToolStripMenuItem";
			this.new_mapping_ToolStripMenuItem.Size = new Size(107, 22);
			this.new_mapping_ToolStripMenuItem.Text = "New";
			this.new_mapping_ToolStripMenuItem.Click += new EventHandler(this.new_mapping_ToolStripMenuItem_Click);
			this.delete_mapping_ToolStripMenuItem1.Name = "delete_mapping_ToolStripMenuItem1";
			this.delete_mapping_ToolStripMenuItem1.Size = new Size(107, 22);
			this.delete_mapping_ToolStripMenuItem1.Text = "Delete";
			this.delete_mapping_ToolStripMenuItem1.Click += new EventHandler(this.delete_mapping_ToolStripMenuItem1_Click);
			this.step2Accept_button.Location = new Point(707, 461);
			this.step2Accept_button.Name = "step2Accept_button";
			this.step2Accept_button.Size = new Size(90, 23);
			this.step2Accept_button.TabIndex = 19;
			this.step2Accept_button.Text = "OK/Next step";
			this.step2Accept_button.UseVisualStyleBackColor = true;
			this.step2Accept_button.Click += new EventHandler(this.step2Accept_button_Click);
			this.customerUserNameFields_Info_label.AutoSize = true;
			this.customerUserNameFields_Info_label.Location = new Point(347, 188);
			this.customerUserNameFields_Info_label.Name = "customerUserNameFields_Info_label";
			this.customerUserNameFields_Info_label.Size = new Size(120, 13);
			this.customerUserNameFields_Info_label.TabIndex = 18;
			this.customerUserNameFields_Info_label.Text = "No information available";
			this.customerUserPostMasterSearchFields_Info_label.AutoSize = true;
			this.customerUserPostMasterSearchFields_Info_label.Location = new Point(347, 162);
			this.customerUserPostMasterSearchFields_Info_label.Name = "customerUserPostMasterSearchFields_Info_label";
			this.customerUserPostMasterSearchFields_Info_label.Size = new Size(120, 13);
			this.customerUserPostMasterSearchFields_Info_label.TabIndex = 17;
			this.customerUserPostMasterSearchFields_Info_label.Text = "No information available";
			this.customerUserSearchFields_Info_label.AutoSize = true;
			this.customerUserSearchFields_Info_label.Location = new Point(347, 136);
			this.customerUserSearchFields_Info_label.Name = "customerUserSearchFields_Info_label";
			this.customerUserSearchFields_Info_label.Size = new Size(120, 13);
			this.customerUserSearchFields_Info_label.TabIndex = 16;
			this.customerUserSearchFields_Info_label.Text = "No information available";
			this.customerUserListFields_Info_label.AutoSize = true;
			this.customerUserListFields_Info_label.Location = new Point(347, 110);
			this.customerUserListFields_Info_label.Name = "customerUserListFields_Info_label";
			this.customerUserListFields_Info_label.Size = new Size(120, 13);
			this.customerUserListFields_Info_label.TabIndex = 15;
			this.customerUserListFields_Info_label.Text = "No information available";
			this.customerUserPostMasterSearchFields_textBox.Location = new Point(202, 159);
			this.customerUserPostMasterSearchFields_textBox.Name = "customerUserPostMasterSearchFields_textBox";
			this.customerUserPostMasterSearchFields_textBox.Size = new Size(139, 20);
			this.customerUserPostMasterSearchFields_textBox.TabIndex = 14;
			this.customerUserPostMasterSearchFields_textBox.Text = "'mail'";
			this.customerUserNameFields_textBox.Location = new Point(144, 185);
			this.customerUserNameFields_textBox.Name = "customerUserNameFields_textBox";
			this.customerUserNameFields_textBox.Size = new Size(197, 20);
			this.customerUserNameFields_textBox.TabIndex = 13;
			this.customerUserNameFields_textBox.Text = "'givenname', 'sn'";
			this.customerUserSearchFields_textBox.Location = new Point(144, 133);
			this.customerUserSearchFields_textBox.Name = "customerUserSearchFields_textBox";
			this.customerUserSearchFields_textBox.Size = new Size(197, 20);
			this.customerUserSearchFields_textBox.TabIndex = 12;
			this.customerUserSearchFields_textBox.Text = "'sAMAccountName', 'cn', 'mail'";
			this.customerUserListFields_textBox.Location = new Point(144, 107);
			this.customerUserListFields_textBox.Name = "customerUserListFields_textBox";
			this.customerUserListFields_textBox.Size = new Size(197, 20);
			this.customerUserListFields_textBox.TabIndex = 11;
			this.customerUserListFields_textBox.Text = "'sAMAccountName', 'cn', 'mail'";
			this.customerUserNameFields_label.AutoSize = true;
			this.customerUserNameFields_label.Location = new Point(11, 188);
			this.customerUserNameFields_label.Name = "customerUserNameFields_label";
			this.customerUserNameFields_label.Size = new Size(131, 13);
			this.customerUserNameFields_label.TabIndex = 10;
			this.customerUserNameFields_label.Text = "CustomerUserNameFields:";
			this.customerUserPostMasterSearchFields_label.AutoSize = true;
			this.customerUserPostMasterSearchFields_label.Location = new Point(11, 162);
			this.customerUserPostMasterSearchFields_label.Name = "customerUserPostMasterSearchFields_label";
			this.customerUserPostMasterSearchFields_label.Size = new Size(190, 13);
			this.customerUserPostMasterSearchFields_label.TabIndex = 9;
			this.customerUserPostMasterSearchFields_label.Text = "CustomerUserPostMasterSearchFields:";
			this.customerUserSearchFields_label.AutoSize = true;
			this.customerUserSearchFields_label.Location = new Point(11, 136);
			this.customerUserSearchFields_label.Name = "customerUserSearchFields_label";
			this.customerUserSearchFields_label.Size = new Size(137, 13);
			this.customerUserSearchFields_label.TabIndex = 8;
			this.customerUserSearchFields_label.Text = "CustomerUserSearchFields:";
			this.customerUserListFields_label.AutoSize = true;
			this.customerUserListFields_label.Location = new Point(11, 110);
			this.customerUserListFields_label.Name = "customerUserListFields_label";
			this.customerUserListFields_label.Size = new Size(119, 13);
			this.customerUserListFields_label.TabIndex = 7;
			this.customerUserListFields_label.Text = "CustomerUserListFields:";
			this.customerKey_comboBox.FormattingEnabled = true;
			this.customerKey_comboBox.Items.AddRange(new object[]
			{
				"mail",
				"sAMAccountName",
				"userPrincipalName",
				"displayName"
			});
			this.customerKey_comboBox.Location = new Point(89, 10);
			this.customerKey_comboBox.Name = "customerKey_comboBox";
			this.customerKey_comboBox.Size = new Size(100, 21);
			this.customerKey_comboBox.TabIndex = 6;
			this.customerID_Info_label.AutoSize = true;
			this.customerID_Info_label.Location = new Point(195, 39);
			this.customerID_Info_label.Name = "customerID_Info_label";
			this.customerID_Info_label.Size = new Size(559, 52);
			this.customerID_Info_label.TabIndex = 5;
			this.customerID_Info_label.Text = componentResourceManager.GetString("customerID_Info_label.Text");
			this.customerKey_Info_label.AutoSize = true;
			this.customerKey_Info_label.Location = new Point(195, 9);
			this.customerKey_Info_label.Name = "customerKey_Info_label";
			this.customerKey_Info_label.Size = new Size(511, 26);
			this.customerKey_Info_label.TabIndex = 4;
			this.customerKey_Info_label.Text = "Usualy sAMAccountName. If you have multiple ADs, you should better change it to a absolutely unique key\r\n(e.g. userPrincipalName). This key will be used to login as a customer.";
			this.customerID_textBox.Location = new Point(89, 39);
			this.customerID_textBox.Name = "customerID_textBox";
			this.customerID_textBox.Size = new Size(100, 20);
			this.customerID_textBox.TabIndex = 3;
			this.customerID_label.AutoSize = true;
			this.customerID_label.Location = new Point(11, 42);
			this.customerID_label.Name = "customerID_label";
			this.customerID_label.Size = new Size(65, 13);
			this.customerID_label.TabIndex = 1;
			this.customerID_label.Text = "CustomerID:";
			this.customerKey_label.AutoSize = true;
			this.customerKey_label.Location = new Point(11, 13);
			this.customerKey_label.Name = "customerKey_label";
			this.customerKey_label.Size = new Size(72, 13);
			this.customerKey_label.TabIndex = 0;
			this.customerKey_label.Text = "CustomerKey:";
			this.step3_panel.BackColor = SystemColors.ControlLight;
			this.step3_panel.Controls.Add(this.copyClipboard_button);
			this.step3_panel.Controls.Add(this.closeProgram_button);
			this.step3_panel.Controls.Add(this.saveConfig_button);
			this.step3_panel.Controls.Add(this.config_textBox);
			this.step3_panel.Location = new Point(12, 28);
			this.step3_panel.Name = "step3_panel";
			this.step3_panel.Size = new Size(807, 500);
			this.step3_panel.TabIndex = 21;
			this.step3_panel.Visible = false;
			this.copyClipboard_button.Location = new Point(272, 473);
			this.copyClipboard_button.Name = "copyClipboard_button";
			this.copyClipboard_button.Size = new Size(266, 23);
			this.copyClipboard_button.TabIndex = 3;
			this.copyClipboard_button.Text = "Copy config to clipboard";
			this.copyClipboard_button.UseVisualStyleBackColor = true;
			this.copyClipboard_button.Click += new EventHandler(this.copyClipboard_button_Click);
			this.closeProgram_button.Location = new Point(541, 473);
			this.closeProgram_button.Name = "closeProgram_button";
			this.closeProgram_button.Size = new Size(261, 23);
			this.closeProgram_button.TabIndex = 2;
			this.closeProgram_button.Text = "Close Programm";
			this.closeProgram_button.UseVisualStyleBackColor = true;
			this.closeProgram_button.Click += new EventHandler(this.closeProgram_button_Click);
			this.saveConfig_button.Location = new Point(4, 473);
			this.saveConfig_button.Name = "saveConfig_button";
			this.saveConfig_button.Size = new Size(266, 23);
			this.saveConfig_button.TabIndex = 1;
			this.saveConfig_button.Text = "Save config to file";
			this.saveConfig_button.UseVisualStyleBackColor = true;
			this.saveConfig_button.Click += new EventHandler(this.saveConfig_button_Click);
			this.config_textBox.Location = new Point(4, 4);
			this.config_textBox.Multiline = true;
			this.config_textBox.Name = "config_textBox";
			this.config_textBox.Size = new Size(798, 466);
			this.config_textBox.TabIndex = 0;
			this.config_textBox.Text = componentResourceManager.GetString("config_textBox.Text");
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.ControlDark;
			base.ClientSize = new Size(831, 537);
			base.Controls.Add(this.HelpReq_Panel);
			base.Controls.Add(this.step3_panel);
			base.Controls.Add(this.step2_panel);
			base.Controls.Add(this.step1_panel);
			base.Controls.Add(this.currentStep_label);
			base.Controls.Add(this.step3_label);
			base.Controls.Add(this.step2_label);
			base.Controls.Add(this.step1_label);
			base.Controls.Add(this.upperBar_toolStrip);
			base.FormBorderStyle = FormBorderStyle.Fixed3D;
			base.HelpButton = true;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Main";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Read customer data // OTRS Active Directory Script Creator";
			base.FormClosing += new FormClosingEventHandler(this.Main_FormClosing);
			this.upperBar_toolStrip.ResumeLayout(false);
			this.upperBar_toolStrip.PerformLayout();
			this.HelpReq_Panel.ResumeLayout(false);
			this.HelpReq_Panel.PerformLayout();
			this.step1_panel.ResumeLayout(false);
			this.step1_panel.PerformLayout();
			this.getBaseDN_groupBox.ResumeLayout(false);
			this.getBaseDN_splitContainer.Panel1.ResumeLayout(false);
			this.getBaseDN_splitContainer.Panel2.ResumeLayout(false);
			this.getBaseDN_splitContainer.ResumeLayout(false);
			this.status_panel.ResumeLayout(false);
			this.status_panel.PerformLayout();
			this.filter_groupBox.ResumeLayout(false);
			this.filter_groupBox.PerformLayout();
			((ISupportInitialize)this.filter_dataGridView).EndInit();
			this.filter_dataGridView_contextMenuStrip.ResumeLayout(false);
			this.differenceHostearchuser_panel.ResumeLayout(false);
			this.differenceHostearchuser_panel.PerformLayout();
			this.step2_panel.ResumeLayout(false);
			this.step2_panel.PerformLayout();
			this.mapping_groupBox.ResumeLayout(false);
			((ISupportInitialize)this.mapping_dataGridView).EndInit();
			this.mapping_dataGridView_contextMenuStrip.ResumeLayout(false);
			this.step3_panel.ResumeLayout(false);
			this.step3_panel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
