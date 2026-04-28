using OracleDBAdmin.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcRoleEditor : UserControl
    {
        private readonly RoleService _service = new RoleService();
        private readonly string _editingRole;

        private TextBox txtRoleName;
        private ComboBox cboAuth;
        private TextBox txtPassword;
        private Button btnSave;
        private Button btnCancel;

        private Panel outerCard;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel sectionBasicInfo;
        private Panel sectionSecurity;

        private Label lblTitle;
        private Label lblHint;

        private readonly Color BgColor = Color.FromArgb(236, 246, 255);
        private readonly Color SurfaceColor = Color.White;
        private readonly Color HeaderBgColor = Color.FromArgb(219, 234, 254);
        private readonly Color SectionBgColor = Color.FromArgb(248, 252, 255);
        private readonly Color PrimaryColor = Color.FromArgb(37, 99, 235);
        private readonly Color PrimaryHoverColor = Color.FromArgb(29, 78, 216);
        private readonly Color BorderColor = Color.FromArgb(191, 219, 254);
        private readonly Color TextColor = Color.FromArgb(15, 23, 42);
        private readonly Color MutedTextColor = Color.FromArgb(100, 116, 139);
        private readonly Color HintBgColor = Color.FromArgb(239, 246, 255);

        public UcRoleEditor(string editingRole = null)
        {
            _editingRole = editingRole;
            InitializeUi();

            if (!string.IsNullOrWhiteSpace(_editingRole))
                LoadRole();
        }

        private void InitializeUi()
        {
            SuspendLayout();

            BackColor = BgColor;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            Padding = new Padding(16);

            outerCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 98,
                BackColor = HeaderBgColor,
                Padding = new Padding(28, 20, 28, 16)
            };

            lblTitle = new Label
            {
                Text = string.IsNullOrWhiteSpace(_editingRole) ? "Tạo Role" : "Chỉnh sửa Role",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(0, 0)
            };

            lblHint = new Label
            {
                Text = "Thiết lập tên role, kiểu xác thực và mật khẩu nếu role dùng cơ chế PASSWORD.",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                ForeColor = MutedTextColor,
                Location = new Point(0, 40)
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblHint);

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(22),
                AutoScroll = true,
                BackColor = SurfaceColor
            };

            sectionBasicInfo = CreateSectionPanel("Thông tin role", 240);
            sectionSecurity = CreateSectionPanel("Xác thực và bảo mật", 180);

            InitializeBasicInfoSection();
            InitializeSecuritySection();

            var footerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 74,
                Padding = new Padding(0, 14, 0, 0)
            };

            btnSave = CreatePrimaryButton("Lưu");
            btnSave.Size = new Size(120, 42);
            btnSave.Click += BtnSave_Click;

            btnCancel = CreateSecondaryButton("Hủy");
            btnCancel.Size = new Size(120, 42);
            btnCancel.Click += (s, e) => FindForm()?.Close();

            footerPanel.Controls.Add(btnSave);
            footerPanel.Controls.Add(btnCancel);

            footerPanel.Resize += (s, e) =>
            {
                btnSave.Left = footerPanel.ClientSize.Width - btnSave.Width;
                btnSave.Top = 10;

                btnCancel.Left = btnSave.Left - btnCancel.Width - 12;
                btnCancel.Top = 10;
            };

            sectionBasicInfo.Dock = DockStyle.Top;
            sectionSecurity.Dock = DockStyle.Top;
            footerPanel.Dock = DockStyle.Top;

            contentPanel.Controls.Add(footerPanel);
            contentPanel.Controls.Add(sectionSecurity);
            contentPanel.Controls.Add(sectionBasicInfo);

            outerCard.Controls.Add(contentPanel);
            outerCard.Controls.Add(headerPanel);

            Controls.Clear();
            Controls.Add(outerCard);

            ResumeLayout(false);
        }

        private Panel CreateSectionPanel(string title, int height)
        {
            var panel = new Panel
            {
                Height = height,
                BackColor = SectionBgColor,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(22, 54, 22, 20),
                Margin = new Padding(0, 0, 0, 16)
            };

            var lblSectionTitle = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(22, 16)
            };

            panel.Controls.Add(lblSectionTitle);
            return panel;
        }

        private void InitializeBasicInfoSection()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                BackColor = Color.Transparent
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            for (int i = 0; i < 4; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            txtRoleName = CreateTextBox();

            cboAuth = CreateComboBox();
            cboAuth.Items.AddRange(new object[] { "NONE", "PASSWORD" });
            cboAuth.SelectedIndex = 0;
            cboAuth.SelectedIndexChanged += (s, e) =>
            {
                txtPassword.Enabled = cboAuth.SelectedItem?.ToString() == "PASSWORD";
            };

            AddField(layout, "Role Name *", txtRoleName, 0, 0);
            AddField(layout, "Authentication", cboAuth, 1, 0);

            sectionBasicInfo.Controls.Add(layout);
            layout.BringToFront();
        }

        private void InitializeSecuritySection()
        {
            var lblPassword = new Label
            {
                Text = "Password",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(22, 64)
            };

            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
                Enabled = false,
                Location = new Point(22, 92),
                Size = new Size(280, 32)
            };

            var infoPanel = new Panel
            {
                Location = new Point(340, 60),
                Size = new Size(sectionSecurity.ClientSize.Width - 362, 80),
                BackColor = HintBgColor,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblInfo = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Gợi ý: nếu chọn PASSWORD thì phải nhập mật khẩu cho role. Nếu chọn NONE thì role sẽ không dùng xác thực bằng mật khẩu.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = PrimaryColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 12, 0)
            };

            infoPanel.Controls.Add(lblInfo);

            sectionSecurity.Resize += (s, e) =>
            {
                infoPanel.Width = Math.Max(220, sectionSecurity.ClientSize.Width - 362);
            };

            sectionSecurity.Controls.Add(lblPassword);
            sectionSecurity.Controls.Add(txtPassword);
            sectionSecurity.Controls.Add(infoPanel);
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10.5F),
                Margin = new Padding(8, 0, 8, 18),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10.5F),
                Margin = new Padding(8, 0, 8, 18),
                FlatStyle = FlatStyle.Flat
            };
        }

        private Button CreatePrimaryButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = PrimaryHoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = PrimaryColor;

            return btn;
        }

        private Button CreateSecondaryButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = PrimaryColor,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderColor = BorderColor;
            btn.FlatAppearance.BorderSize = 1;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(239, 246, 255);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.White;

            return btn;
        }

        private void AddField(TableLayoutPanel layout, string labelText, Control input, int col, int rowBase)
        {
            var lbl = new Label
            {
                Text = labelText,
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = TextColor,
                Margin = new Padding(8, 0, 8, 6)
            };

            input.Margin = new Padding(8, 0, 8, 18);

            layout.Controls.Add(lbl, col, rowBase);
            layout.Controls.Add(input, col, rowBase + 1);
        }

        private void LoadRole()
        {
            var row = _service.GetRoleByName(_editingRole);
            if (row == null) return;

            txtRoleName.Text = row["ROLE"].ToString();
            txtRoleName.Enabled = false;

            string passwordRequired = row["PASSWORD_REQUIRED"].ToString();
            cboAuth.SelectedItem = passwordRequired == "YES" ? "PASSWORD" : "NONE";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string roleName = txtRoleName.Text.Trim();
                string auth = cboAuth.SelectedItem?.ToString();
                string password = txtPassword.Text;

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    MessageBox.Show("Role name không được rỗng.");
                    return;
                }

                if (auth == "PASSWORD" && string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Role kiểu PASSWORD phải nhập mật khẩu.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_editingRole))
                {
                    _service.CreateRole(roleName, auth, password);
                    MessageBox.Show("Tạo role thành công.");
                }
                else
                {
                    _service.AlterRole(roleName, auth, password);
                    MessageBox.Show("Cập nhật role thành công.");
                }

                // FindForm()?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu role: " + ex.Message);
            }
        }
    }
}