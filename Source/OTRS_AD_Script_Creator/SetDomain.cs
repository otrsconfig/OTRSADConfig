using OTRS_AD_Script_Creator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OTRS_AD_Script_Creator
{
	public class SetDomain : Form
	{
		private const int EM_GETLINECOUNT = 186;

		private intro intropic = new intro();

		private Point PanelMouseDownLocation;

		private Settings settings = new Settings();

		private IContainer components;

		private Label HostAddress_label;

		private Label BaseDN_label;

		private Label UserAttribute_label;

		private Label SearchUserDN_label;

		private Label SearchUserPassword_label;

		private TextBox Note_textBox;

		private TextBox BaseDN_textBox;

		private TextBox UserAttribute_textBox;

		private TextBox SearchUserDN_textBox;

		private TextBox SearchUserPassword_textBox;

		private Button OK_button;

		private Button TestConnection_button;

		private Label HostAddressExample_label;

		private Label BaseDNExample_label;

		private Label UserAttributeExample_label;

		private Label SearchUserDNExample_label;

		private Label SearchUserPasswordExample_label;

		private ToolTip Help_toolTip;

		private Panel HelpReq_Panel;

		private Label HelpReqCaption_label;

		private TextBox HelpReqText_textBox;

		private Button HelpReqClose_button;

		private Panel ViewPassword_panel;

		private CheckBox AnonymousRead_checkBox;

		private Button GetBaseDN_button;

		private Button GetDomainController_button;

		private ComboBox HostAddress_textBox;

		private Button OpenBaseDNSetter_button;

		[DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "SendMessageA", ExactSpelling = true, SetLastError = true)]
		private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

		public SetDomain()
		{
			this.InitializeComponent();
			this.intropic.StartPosition = FormStartPosition.CenterParent;
			this.intropic.ShowDialog();
		}

		private void GetBaseDN_button_Click(object sender, EventArgs e)
		{
			if (this.HostAddress_textBox.Text != "" && ((this.SearchUserDN_textBox.Text != "" && this.SearchUserPassword_textBox.Text != "") || this.AnonymousRead_checkBox.Checked))
			{
				this.GetDefaultNamingContext();
				return;
			}
			MessageBox.Show("All textboxes except BaseDN have to be filled\r\nIf anonymous authentication is allowed check the box\r\n'Anonymous users have permission...'", "Fill in...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		private void GetDefaultNamingContext()
		{
			try
			{
				if (this.AnonymousRead_checkBox.Checked)
				{
					string text = new DirectoryEntry("LDAP://RootDSE")
					{
						AuthenticationType = AuthenticationTypes.Anonymous
					}.Properties["defaultNamingContext"].Value.ToString();
					this.BaseDN_textBox.Text = text.ToString();
				}
				else
				{
					DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://RootDSE", this.SearchUserDN_textBox.Text, this.SearchUserPassword_textBox.Text, AuthenticationTypes.Secure);
					string text = directoryEntry.Properties["defaultNamingContext"].Value.ToString();
					this.BaseDN_textBox.Text = text.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private string GetUserName(string identity)
		{
			if (identity.Contains("\\"))
			{
				string[] array = identity.Split(new char[]
				{
					'\\'
				});
				return array[1];
			}
			return identity;
		}

		public string GetUserDn(string identity)
		{
			string userName = this.GetUserName(identity);
			using (new DirectoryEntry("LDAP://" + this.HostAddress_textBox.Text, this.SearchUserDN_textBox.Text, this.SearchUserPassword_textBox.Text, AuthenticationTypes.Secure))
			{
				using (DirectorySearcher directorySearcher = new DirectorySearcher(string.Format("(" + this.UserAttribute_textBox.Text + "={0})", userName)))
				{
					try
					{
						SearchResult searchResult = directorySearcher.FindOne();
						if (searchResult != null)
						{
							using (DirectoryEntry directoryEntry2 = searchResult.GetDirectoryEntry())
							{
								return (string)directoryEntry2.Properties["distinguishedName"].Value;
							}
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				}
			}
			return null;
		}

		private void HostAdresse_label_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.HostAddress_textBox.Location.X, this.HostAddress_textBox.Location.Y + this.HostAddress_textBox.Height);
			this.HelpReqCaption_label.Text = "Host Address";
			this.HelpReqText_textBox.Text = "The Fully Qualified Domain Name(FQDN) is required to locate your Domain Controller in the DNS.\r\nIt contains the hostname as well as the domain\r\ne.g. something.domain.com";
			this.ChangeHelpReqSize();
		}

		private void ChangeHelpReqSize()
		{
			int num = SetDomain.SendMessage(this.HelpReqText_textBox.Handle.ToInt32(), 186, 0, 0);
			this.HelpReqText_textBox.Height = (this.HelpReqText_textBox.Font.Height + 2) * num;
			this.HelpReq_Panel.Height = this.HelpReqText_textBox.Height + this.HelpReqCaption_label.Height + 10;
		}

		private void BaseDN_label_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.BaseDN_textBox.Location.X, this.BaseDN_textBox.Location.Y + this.BaseDN_textBox.Height);
			this.HelpReqCaption_label.Text = "Base Distinguished  Name";
			this.HelpReqText_textBox.Text = "The BaseDN defines the location to search for objects. It represents an object in the hierarchically structured directory. There are three keywords\r\ncn:Common Name\r\nou:Organisational Unit\r\ndc: Domain Component";
			this.ChangeHelpReqSize();
		}

		private void UserAttribute_label_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.UserAttribute_textBox.Location.X, this.UserAttribute_textBox.Location.Y + this.UserAttribute_textBox.Height);
			this.HelpReqCaption_label.Text = "User attribute";
			this.HelpReqText_textBox.Text = "Your attribute to identify the user in your AD. you cannot choose an ambigous attribute like 'accountExpires', it has to be unique to each user.\r\n e.g. sAMAccountName, userPrincipalName";
			this.ChangeHelpReqSize();
		}

		private void SearchUserDN_label_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.SearchUserDN_textBox.Location.X, this.SearchUserDN_textBox.Location.Y + this.SearchUserDN_textBox.Height);
			this.HelpReqCaption_label.Text = "Seach User DN";
			this.HelpReqText_textBox.Text = "Your qualified user to read the Active Directory. All the credentials are saved plain onto the config.pm! To avoid security issues create a user wihtout any rights beside reading the AD";
			this.ChangeHelpReqSize();
		}

		private void AnonymousRead_checkBox_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			this.HelpReq_Panel.Show();
			this.HelpReq_Panel.Location = new Point(this.AnonymousRead_checkBox.Location.X, this.AnonymousRead_checkBox.Location.Y + this.AnonymousRead_checkBox.Height);
			this.HelpReqCaption_label.Text = "Anonymous user permission";
			this.HelpReqText_textBox.Text = "Only check this box if you are sure that everyone can read your ldap tree. To check your settings launch ADSIEdit and navigate to CN=Directory Service, CN= Windows NT, CN = Services, CN= Configuration, DC=<domain name>, DC=com.\r\nRight-click the CN=Directory Service container, choose Properties from the Context menu, and scroll down to the dsHeuristics attribute and check if '0000002' is set.\r\nNOTE: To grant anonymous access you have to change the seventh character and fill the rest with zeros.\r\nE.g. 001 -> 0010002.";
			this.ChangeHelpReqSize();
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

		private void HelpReqClose_button_Click(object sender, EventArgs e)
		{
			this.HelpReq_Panel.Hide();
		}

		private void TestConnection_button_Click(object sender, EventArgs e)
		{
			if (this.Authenticate())
			{
				MessageBox.Show("Connection successfully established", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			MessageBox.Show("Connection could not be established", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private bool Authenticate()
		{
			bool result = false;
			try
			{
				DirectoryEntry directoryEntry;
				if (this.AnonymousRead_checkBox.CheckState == CheckState.Checked)
				{
					directoryEntry = new DirectoryEntry("LDAP://" + this.HostAddress_textBox.Text);
					directoryEntry.AuthenticationType = AuthenticationTypes.Anonymous;
				}
				else
				{
					directoryEntry = new DirectoryEntry("LDAP://" + this.HostAddress_textBox.Text, this.SearchUserDN_textBox.Text, this.SearchUserPassword_textBox.Text);
				}
				try
				{
					object arg_6E_0 = directoryEntry.NativeObject;
					result = true;
				}
				catch (Exception ex)
				{
					if (MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand) == DialogResult.Retry)
					{
						this.Authenticate();
					}
				}
			}
			catch (DirectoryServicesCOMException)
			{
			}
			return result;
		}

		private void OK_button_Click(object sender, EventArgs e)
		{
			if (this.HostAddress_textBox.Text != "" && this.BaseDN_textBox.Text != "" && this.UserAttribute_textBox.Text != "")
			{
				if (this.AnonymousRead_checkBox.Checked || (this.SearchUserDN_textBox.Text != "" && this.SearchUserPassword_textBox.Text != ""))
				{
					this.settings.HostAddress = this.HostAddress_textBox.Text;
					this.settings.BaseDNSearch = this.BaseDN_textBox.Text;
					this.settings.SeachUserDN = this.SearchUserDN_textBox.Text;
					this.settings.SearchUserPassword = this.SearchUserPassword_textBox.Text;
					this.settings.AnonymousAllowed = this.AnonymousRead_checkBox.Checked;
					this.settings.UserAttributeSearch = this.UserAttribute_textBox.Text;
					this.settings.Save();
					base.DialogResult = DialogResult.OK;
					base.Close();
					return;
				}
				if (!this.AnonymousRead_checkBox.Checked)
				{
					if (this.SearchUserPassword_textBox.Text == "")
					{
						this.SearchUserPassword_textBox.BackColor = Color.LightCoral;
					}
					else
					{
						this.SearchUserPassword_textBox.BackColor = SystemColors.Window;
					}
					if (this.SearchUserDN_textBox.Text == "")
					{
						this.SearchUserDN_textBox.BackColor = Color.LightCoral;
						return;
					}
					this.SearchUserDN_textBox.BackColor = SystemColors.Window;
					return;
				}
			}
			else
			{
				if (this.HostAddress_textBox.Text == "")
				{
					this.HostAddress_textBox.BackColor = Color.LightCoral;
				}
				else
				{
					this.HostAddress_textBox.BackColor = SystemColors.Window;
				}
				if (this.BaseDN_textBox.Text == "")
				{
					this.BaseDN_textBox.BackColor = Color.LightCoral;
				}
				else
				{
					this.BaseDN_textBox.BackColor = SystemColors.Window;
				}
				if (this.UserAttribute_textBox.Text == "")
				{
					this.UserAttribute_textBox.BackColor = Color.LightCoral;
					return;
				}
				this.UserAttribute_textBox.BackColor = SystemColors.Window;
			}
		}

		private void ViewPassword_panel_MouseEnter(object sender, EventArgs e)
		{
			this.SearchUserPassword_textBox.PasswordChar = '\0';
		}

		private void ViewPassword_panel_MouseLeave(object sender, EventArgs e)
		{
			this.SearchUserPassword_textBox.PasswordChar = '*';
		}

		private void AnonymousRead_checkBox_CheckedChanged(object sender, EventArgs e)
		{
			if (this.AnonymousRead_checkBox.Checked)
			{
				this.SearchUserDN_textBox.Enabled = false;
				this.SearchUserPassword_textBox.Enabled = false;
				this.ViewPassword_panel.Enabled = false;
				this.SearchUserDN_textBox.BackColor = SystemColors.Window;
				this.SearchUserPassword_textBox.BackColor = SystemColors.Window;
				return;
			}
			this.SearchUserDN_textBox.Enabled = true;
			this.SearchUserPassword_textBox.Enabled = true;
			this.ViewPassword_panel.Enabled = true;
		}

		private void GetDomainController_button_Click(object sender, EventArgs e)
		{
			this.HostAddress_textBox.Items.Clear();
			foreach (string current in this.GetDCs())
			{
				this.HostAddress_textBox.Items.Add(current);
			}
			this.HostAddress_textBox.DroppedDown = true;
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

		private void OpenBaseDNSetter_button_Click(object sender, EventArgs e)
		{
			BaseDNSetter baseDNSetter = new BaseDNSetter(this.HostAddress_textBox.Text, this.SearchUserDN_textBox.Text, this.SearchUserPassword_textBox.Text);
			try
			{
				if (baseDNSetter.ShowDialog() == DialogResult.OK)
				{
					this.BaseDN_textBox.Text = baseDNSetter.GetBaseDN();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SetDomain));
			this.HostAddress_label = new Label();
			this.BaseDN_label = new Label();
			this.UserAttribute_label = new Label();
			this.SearchUserDN_label = new Label();
			this.SearchUserPassword_label = new Label();
			this.Note_textBox = new TextBox();
			this.BaseDN_textBox = new TextBox();
			this.UserAttribute_textBox = new TextBox();
			this.SearchUserDN_textBox = new TextBox();
			this.SearchUserPassword_textBox = new TextBox();
			this.OK_button = new Button();
			this.TestConnection_button = new Button();
			this.HostAddressExample_label = new Label();
			this.BaseDNExample_label = new Label();
			this.UserAttributeExample_label = new Label();
			this.SearchUserDNExample_label = new Label();
			this.SearchUserPasswordExample_label = new Label();
			this.Help_toolTip = new ToolTip(this.components);
			this.HelpReq_Panel = new Panel();
			this.HelpReqClose_button = new Button();
			this.HelpReqCaption_label = new Label();
			this.HelpReqText_textBox = new TextBox();
			this.AnonymousRead_checkBox = new CheckBox();
			this.HostAddress_textBox = new ComboBox();
			this.OpenBaseDNSetter_button = new Button();
			this.ViewPassword_panel = new Panel();
			this.GetDomainController_button = new Button();
			this.GetBaseDN_button = new Button();
			this.HelpReq_Panel.SuspendLayout();
			base.SuspendLayout();
			this.HostAddress_label.AutoSize = true;
			this.HostAddress_label.Location = new Point(12, 9);
			this.HostAddress_label.Name = "HostAddress_label";
			this.HostAddress_label.Size = new Size(73, 13);
			this.HostAddress_label.TabIndex = 0;
			this.HostAddress_label.Text = "Host-Address:";
			this.Help_toolTip.SetToolTip(this.HostAddress_label, "asda");
			this.HostAddress_label.HelpRequested += new HelpEventHandler(this.HostAdresse_label_HelpRequested);
			this.BaseDN_label.AutoSize = true;
			this.BaseDN_label.Location = new Point(12, 33);
			this.BaseDN_label.Name = "BaseDN_label";
			this.BaseDN_label.Size = new Size(50, 13);
			this.BaseDN_label.TabIndex = 1;
			this.BaseDN_label.Text = "BaseDN:";
			this.BaseDN_label.HelpRequested += new HelpEventHandler(this.BaseDN_label_HelpRequested);
			this.UserAttribute_label.AutoSize = true;
			this.UserAttribute_label.Location = new Point(12, 57);
			this.UserAttribute_label.Name = "UserAttribute_label";
			this.UserAttribute_label.Size = new Size(73, 13);
			this.UserAttribute_label.TabIndex = 2;
			this.UserAttribute_label.Text = "User attribute:";
			this.UserAttribute_label.HelpRequested += new HelpEventHandler(this.UserAttribute_label_HelpRequested);
			this.SearchUserDN_label.AutoSize = true;
			this.SearchUserDN_label.Location = new Point(12, 109);
			this.SearchUserDN_label.Name = "SearchUserDN_label";
			this.SearchUserDN_label.Size = new Size(86, 13);
			this.SearchUserDN_label.TabIndex = 3;
			this.SearchUserDN_label.Text = "Search user DN:";
			this.SearchUserDN_label.HelpRequested += new HelpEventHandler(this.SearchUserDN_label_HelpRequested);
			this.SearchUserPassword_label.AutoSize = true;
			this.SearchUserPassword_label.Location = new Point(12, 134);
			this.SearchUserPassword_label.Name = "SearchUserPassword_label";
			this.SearchUserPassword_label.Size = new Size(92, 13);
			this.SearchUserPassword_label.TabIndex = 4;
			this.SearchUserPassword_label.Text = "Search password:";
			this.Note_textBox.Enabled = false;
			this.Note_textBox.Location = new Point(15, 166);
			this.Note_textBox.Multiline = true;
			this.Note_textBox.Name = "Note_textBox";
			this.Note_textBox.ReadOnly = true;
			this.Note_textBox.Size = new Size(419, 74);
			this.Note_textBox.TabIndex = 5;
			this.Note_textBox.TabStop = false;
			this.Note_textBox.Text = componentResourceManager.GetString("Note_textBox.Text");
			this.BaseDN_textBox.Location = new Point(110, 30);
			this.BaseDN_textBox.Name = "BaseDN_textBox";
			this.BaseDN_textBox.Size = new Size(144, 20);
			this.BaseDN_textBox.TabIndex = 7;
			this.BaseDN_textBox.HelpRequested += new HelpEventHandler(this.BaseDN_label_HelpRequested);
			this.UserAttribute_textBox.Location = new Point(110, 54);
			this.UserAttribute_textBox.Name = "UserAttribute_textBox";
			this.UserAttribute_textBox.Size = new Size(203, 20);
			this.UserAttribute_textBox.TabIndex = 8;
			this.UserAttribute_textBox.HelpRequested += new HelpEventHandler(this.UserAttribute_label_HelpRequested);
			this.SearchUserDN_textBox.Location = new Point(110, 105);
			this.SearchUserDN_textBox.Name = "SearchUserDN_textBox";
			this.SearchUserDN_textBox.Size = new Size(170, 20);
			this.SearchUserDN_textBox.TabIndex = 9;
			this.SearchUserDN_textBox.HelpRequested += new HelpEventHandler(this.SearchUserDN_label_HelpRequested);
			this.SearchUserPassword_textBox.Location = new Point(110, 131);
			this.SearchUserPassword_textBox.Name = "SearchUserPassword_textBox";
			this.SearchUserPassword_textBox.PasswordChar = '*';
			this.SearchUserPassword_textBox.Size = new Size(138, 20);
			this.SearchUserPassword_textBox.TabIndex = 10;
			this.OK_button.Location = new Point(359, 246);
			this.OK_button.Name = "OK_button";
			this.OK_button.Size = new Size(75, 23);
			this.OK_button.TabIndex = 12;
			this.OK_button.Text = "OK";
			this.OK_button.UseVisualStyleBackColor = true;
			this.OK_button.Click += new EventHandler(this.OK_button_Click);
			this.TestConnection_button.Location = new Point(15, 246);
			this.TestConnection_button.Name = "TestConnection_button";
			this.TestConnection_button.Size = new Size(106, 23);
			this.TestConnection_button.TabIndex = 11;
			this.TestConnection_button.Text = "Test connection";
			this.TestConnection_button.UseVisualStyleBackColor = true;
			this.TestConnection_button.Click += new EventHandler(this.TestConnection_button_Click);
			this.HostAddressExample_label.AutoSize = true;
			this.HostAddressExample_label.Location = new Point(315, 9);
			this.HostAddressExample_label.Name = "HostAddressExample_label";
			this.HostAddressExample_label.Size = new Size(125, 13);
			this.HostAddressExample_label.TabIndex = 13;
			this.HostAddressExample_label.Text = "e.g.: controller.domain.tld";
			this.BaseDNExample_label.AutoSize = true;
			this.BaseDNExample_label.Location = new Point(315, 33);
			this.BaseDNExample_label.Name = "BaseDNExample_label";
			this.BaseDNExample_label.Size = new Size(118, 13);
			this.BaseDNExample_label.TabIndex = 14;
			this.BaseDNExample_label.Text = "e.g.: dc=domain, dc=tld";
			this.UserAttributeExample_label.AutoSize = true;
			this.UserAttributeExample_label.Location = new Point(315, 57);
			this.UserAttributeExample_label.Name = "UserAttributeExample_label";
			this.UserAttributeExample_label.Size = new Size(120, 13);
			this.UserAttributeExample_label.TabIndex = 15;
			this.UserAttributeExample_label.Text = "e.g.: sAMAccountName";
			this.SearchUserDNExample_label.AutoSize = true;
			this.SearchUserDNExample_label.Location = new Point(286, 109);
			this.SearchUserDNExample_label.Name = "SearchUserDNExample_label";
			this.SearchUserDNExample_label.Size = new Size(104, 13);
			this.SearchUserDNExample_label.TabIndex = 16;
			this.SearchUserDNExample_label.Text = "Your authorized user";
			this.SearchUserPasswordExample_label.AutoSize = true;
			this.SearchUserPasswordExample_label.Location = new Point(286, 134);
			this.SearchUserPasswordExample_label.Name = "SearchUserPasswordExample_label";
			this.SearchUserPasswordExample_label.Size = new Size(106, 13);
			this.SearchUserPasswordExample_label.TabIndex = 17;
			this.SearchUserPasswordExample_label.Text = "Password of the user";
			this.HelpReq_Panel.BackColor = Color.Gold;
			this.HelpReq_Panel.Controls.Add(this.HelpReqClose_button);
			this.HelpReq_Panel.Controls.Add(this.HelpReqCaption_label);
			this.HelpReq_Panel.Controls.Add(this.HelpReqText_textBox);
			this.HelpReq_Panel.Location = new Point(129, 246);
			this.HelpReq_Panel.Name = "HelpReq_Panel";
			this.HelpReq_Panel.Size = new Size(282, 183);
			this.HelpReq_Panel.TabIndex = 18;
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
			this.HelpReqText_textBox.Size = new Size(276, 159);
			this.HelpReqText_textBox.TabIndex = 0;
			this.AnonymousRead_checkBox.AutoSize = true;
			this.AnonymousRead_checkBox.Location = new Point(18, 81);
			this.AnonymousRead_checkBox.Name = "AnonymousRead_checkBox";
			this.AnonymousRead_checkBox.Size = new Size(317, 17);
			this.AnonymousRead_checkBox.TabIndex = 20;
			this.AnonymousRead_checkBox.Text = "Anonymous users have permission to read from the LDAP tree";
			this.AnonymousRead_checkBox.UseVisualStyleBackColor = true;
			this.AnonymousRead_checkBox.CheckedChanged += new EventHandler(this.AnonymousRead_checkBox_CheckedChanged);
			this.AnonymousRead_checkBox.HelpRequested += new HelpEventHandler(this.AnonymousRead_checkBox_HelpRequested);
			this.HostAddress_textBox.FormattingEnabled = true;
			this.HostAddress_textBox.Location = new Point(110, 6);
			this.HostAddress_textBox.Name = "HostAddress_textBox";
			this.HostAddress_textBox.Size = new Size(170, 21);
			this.HostAddress_textBox.TabIndex = 6;
			this.OpenBaseDNSetter_button.FlatStyle = FlatStyle.Popup;
			this.OpenBaseDNSetter_button.Location = new Point(255, 30);
			this.OpenBaseDNSetter_button.Name = "OpenBaseDNSetter_button";
			this.OpenBaseDNSetter_button.Size = new Size(24, 20);
			this.OpenBaseDNSetter_button.TabIndex = 23;
			this.OpenBaseDNSetter_button.Text = "...";
			this.OpenBaseDNSetter_button.UseVisualStyleBackColor = true;
			this.OpenBaseDNSetter_button.Click += new EventHandler(this.OpenBaseDNSetter_button_Click);
			this.ViewPassword_panel.BackgroundImage = Resources.ViewPassword;
			this.ViewPassword_panel.BorderStyle = BorderStyle.FixedSingle;
			this.ViewPassword_panel.Location = new Point(251, 131);
			this.ViewPassword_panel.Name = "ViewPassword_panel";
			this.ViewPassword_panel.Size = new Size(29, 20);
			this.ViewPassword_panel.TabIndex = 19;
			this.ViewPassword_panel.MouseEnter += new EventHandler(this.ViewPassword_panel_MouseEnter);
			this.ViewPassword_panel.MouseLeave += new EventHandler(this.ViewPassword_panel_MouseLeave);
			this.GetDomainController_button.FlatStyle = FlatStyle.Popup;
			this.GetDomainController_button.Image = Resources.Search;
			this.GetDomainController_button.Location = new Point(284, 6);
			this.GetDomainController_button.Name = "GetDomainController_button";
			this.GetDomainController_button.RightToLeft = RightToLeft.Yes;
			this.GetDomainController_button.Size = new Size(29, 20);
			this.GetDomainController_button.TabIndex = 22;
			this.GetDomainController_button.UseVisualStyleBackColor = true;
			this.GetDomainController_button.Click += new EventHandler(this.GetDomainController_button_Click);
			this.GetBaseDN_button.FlatStyle = FlatStyle.Popup;
			this.GetBaseDN_button.Image = Resources.Search;
			this.GetBaseDN_button.Location = new Point(284, 30);
			this.GetBaseDN_button.Name = "GetBaseDN_button";
			this.GetBaseDN_button.Size = new Size(29, 20);
			this.GetBaseDN_button.TabIndex = 21;
			this.GetBaseDN_button.UseVisualStyleBackColor = true;
			this.GetBaseDN_button.Click += new EventHandler(this.GetBaseDN_button_Click);
			base.AcceptButton = this.OK_button;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(446, 276);
			base.Controls.Add(this.HelpReq_Panel);
			base.Controls.Add(this.SearchUserPasswordExample_label);
			base.Controls.Add(this.SearchUserDNExample_label);
			base.Controls.Add(this.UserAttributeExample_label);
			base.Controls.Add(this.BaseDNExample_label);
			base.Controls.Add(this.HostAddressExample_label);
			base.Controls.Add(this.TestConnection_button);
			base.Controls.Add(this.OK_button);
			base.Controls.Add(this.SearchUserPassword_textBox);
			base.Controls.Add(this.SearchUserDN_textBox);
			base.Controls.Add(this.UserAttribute_textBox);
			base.Controls.Add(this.BaseDN_textBox);
			base.Controls.Add(this.Note_textBox);
			base.Controls.Add(this.SearchUserPassword_label);
			base.Controls.Add(this.SearchUserDN_label);
			base.Controls.Add(this.UserAttribute_label);
			base.Controls.Add(this.BaseDN_label);
			base.Controls.Add(this.HostAddress_label);
			base.Controls.Add(this.ViewPassword_panel);
			base.Controls.Add(this.AnonymousRead_checkBox);
			base.Controls.Add(this.HostAddress_textBox);
			base.Controls.Add(this.GetDomainController_button);
			base.Controls.Add(this.GetBaseDN_button);
			base.Controls.Add(this.OpenBaseDNSetter_button);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.HelpButton = true;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SetDomain";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Set Domain/Credentials for authentication";
			this.HelpReq_Panel.ResumeLayout(false);
			this.HelpReq_Panel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
