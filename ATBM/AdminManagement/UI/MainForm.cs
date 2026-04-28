using OracleDBAdmin.UI.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI
{
    public partial class MainForm : Form
    {
		public bool IsReturningToLogin { get; set; } = false;
		
        private Panel pnlTop;
        private Panel pnlContent;

        private Button btnUserList;
        private Button btnUserCreate;
        private Button btnRoleList;
        private Button btnRoleCreate;
        private Button btnGrant;
        private Button btnRevoke;
        private Button btnViewPrivilege;

		public MainForm()
		{
			InitializeUi();
			try
			{
				LoadControl(new UcUserList());
			}
			catch
			{
				this.Close();
			}
		}

		private void InitializeUi()
        {
            Text = "Oracle DB Admin Console";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(244, 242, 236);

            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(15, 15, 15, 10)
            };

            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            btnUserList = CreateTabButton("Danh sách User", (s, e) => LoadControl(new UcUserList()));
            btnUserCreate = CreateTabButton("Tạo mới User", (s, e) => LoadControl(new UcUserEditor()));
            btnRoleList = CreateTabButton("Danh sách Role", (s, e) => LoadControl(new UcRoleList()));
            btnRoleCreate = CreateTabButton("Tạo mới Role", (s, e) => LoadControl(new UcRoleEditor()));
            btnGrant = CreateTabButton("Cấp quyền", (s, e) => LoadControl(new UcGrantPrivilege()));
            btnRevoke = CreateTabButton("Thu hồi quyền", (s, e) => LoadControl(new UcRevokePrivilege()));
            btnViewPrivilege = CreateTabButton("Xem quyền", (s, e) => LoadControl(new UcViewPrivilege()));

            FlowLayoutPanel nav = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                WrapContents = false
            };

            nav.Controls.Add(btnUserList);
            nav.Controls.Add(btnUserCreate);
            nav.Controls.Add(btnRoleList);
            nav.Controls.Add(btnRoleCreate);
            nav.Controls.Add(btnGrant);
            nav.Controls.Add(btnRevoke);
            nav.Controls.Add(btnViewPrivilege);

            pnlTop.Controls.Add(nav);

            Controls.Add(pnlContent);
            Controls.Add(pnlTop);
        }

        private Button CreateTabButton(string text, EventHandler click)
        {
            Button btn = new Button
            {
                Text = text,
                Width = 145,
                Height = 38,
                Margin = new Padding(6, 0, 6, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderColor = Color.Silver;
            btn.Click += click;
            return btn;
        }

        private void LoadControl(UserControl uc)
        {
            try
            {
                pnlContent.Controls.Clear();
                uc.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(uc);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị giao diện: " + ex.Message);
                this.Close();
            }
        }
    }
}