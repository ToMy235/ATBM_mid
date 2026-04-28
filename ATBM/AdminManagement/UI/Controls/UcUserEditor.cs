using OracleDBAdmin.Services;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcUserEditor : UserControl
    {
        private readonly UserService _service = new UserService();
        private readonly string _editingUsername;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private ComboBox cboProfile;
        private ComboBox cboDefaultTs;
        private ComboBox cboTempTs;
        private ComboBox cboStatus;
        private DateTimePicker dtpExpiry;
        private CheckBox chkUseExpiry;
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

        public UcUserEditor(string editingUsername = null)
        {
            _editingUsername = editingUsername;
            InitializeUi();
            LoadCombos();

            if (!string.IsNullOrWhiteSpace(_editingUsername))
                LoadUserInfo();
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
                Text = string.IsNullOrWhiteSpace(_editingUsername) ? "Tạo User" : "Chỉnh sửa User",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(0, 0)
            };

            lblHint = new Label
            {
                Text = "Thiết lập thông tin tài khoản, tablespace, profile và trạng thái bảo mật trước khi lưu.",
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

            sectionBasicInfo = CreateSectionPanel("Thông tin tài khoản", 300);
            sectionSecurity = CreateSectionPanel("Bảo mật và tùy chọn", 190);

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
                RowCount = 8,
                BackColor = Color.Transparent
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            for (int i = 0; i < 8; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            txtUsername = CreateTextBox();
            txtPassword = CreateTextBox();
            txtPassword.UseSystemPasswordChar = true;

            txtConfirmPassword = CreateTextBox();
            txtConfirmPassword.UseSystemPasswordChar = true;

            cboProfile = CreateComboBox();
            cboDefaultTs = CreateComboBox();
            cboTempTs = CreateComboBox();

            cboStatus = CreateComboBox();
            cboStatus.Items.AddRange(new object[] { "UNLOCK", "LOCK" });
            cboStatus.SelectedIndex = 0;

            AddField(layout, "Username *", txtUsername, 0, 0);
            AddField(layout, "Mật khẩu *", txtPassword, 1, 0);

            AddField(layout, "Xác nhận mật khẩu *", txtConfirmPassword, 0, 2);
            AddField(layout, "Profile", cboProfile, 1, 2);

            AddField(layout, "Default Tablespace", cboDefaultTs, 0, 4);
            AddField(layout, "Temp Tablespace", cboTempTs, 1, 4);

            AddField(layout, "Trạng thái tài khoản", cboStatus, 0, 6);

            sectionBasicInfo.Controls.Add(layout);
            layout.BringToFront();
        }

        private void InitializeSecuritySection()
        {
            chkUseExpiry = new CheckBox
            {
                Text = "Đánh dấu password expire",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(22, 60)
            };

            var lblExpiry = new Label
            {
                Text = "Ngày hết hạn mật khẩu",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(22, 96)
            };

            dtpExpiry = new DateTimePicker
            {
                Width = 280,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10.5F),
                Location = new Point(22, 124)
            };

            var infoPanel = new Panel
            {
                Location = new Point(340, 92),
                Size = new Size(sectionSecurity.ClientSize.Width - 362, 70),
                BackColor = HintBgColor,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblInfo = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Gợi ý: khi tạo user mới, bạn nên nhập mật khẩu và xác nhận mật khẩu. Khi sửa user, có thể để trống mật khẩu nếu không muốn đổi.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = PrimaryColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 12, 0)
            };

            infoPanel.Controls.Add(lblInfo);

            chkUseExpiry.CheckedChanged += (s, e) =>
            {
                dtpExpiry.Enabled = chkUseExpiry.Checked;
            };
            dtpExpiry.Enabled = false;

            sectionSecurity.Resize += (s, e) =>
            {
                infoPanel.Width = Math.Max(220, sectionSecurity.ClientSize.Width - 362);
            };

            sectionSecurity.Controls.Add(chkUseExpiry);
            sectionSecurity.Controls.Add(lblExpiry);
            sectionSecurity.Controls.Add(dtpExpiry);
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

        private void LoadCombos()
        {
            var profiles = _service.GetProfiles();
            cboProfile.DataSource = profiles;
            cboProfile.DisplayMember = "PROFILE";
            cboProfile.ValueMember = "PROFILE";

            var ts = _service.GetTablespaces();
            cboDefaultTs.DataSource = ts.Copy();
            cboDefaultTs.DisplayMember = "TABLESPACE_NAME";
            cboDefaultTs.ValueMember = "TABLESPACE_NAME";

            cboTempTs.DataSource = ts;
            cboTempTs.DisplayMember = "TABLESPACE_NAME";
            cboTempTs.ValueMember = "TABLESPACE_NAME";
        }

        private void LoadUserInfo()
        {
            var row = _service.GetUserByName(_editingUsername);
            if (row == null) return;

            txtUsername.Text = row["USERNAME"].ToString();
            txtUsername.Enabled = false;

            cboDefaultTs.SelectedValue = row["DEFAULT_TABLESPACE"].ToString();
            cboTempTs.SelectedValue = row["TEMPORARY_TABLESPACE"].ToString();
            cboProfile.SelectedValue = row["PROFILE"].ToString();
            cboStatus.SelectedItem = row["ACCOUNT_STATUS"].ToString().Contains("LOCK") ? "LOCK" : "UNLOCK";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;
                string confirm = txtConfirmPassword.Text;
                string profile = cboProfile.SelectedValue?.ToString();
                string defaultTs = cboDefaultTs.SelectedValue?.ToString();
                string tempTs = cboTempTs.SelectedValue?.ToString();
                bool unlock = cboStatus.SelectedItem?.ToString() == "UNLOCK";

                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Username không được rỗng.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_editingUsername))
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show("Mật khẩu không được rỗng.");
                        return;
                    }

                    if (password != confirm)
                    {
                        MessageBox.Show("Confirm mật khẩu không khớp.");
                        return;
                    }

                    _service.CreateUser(
                        username,
                        password,
                        defaultTs,
                        tempTs,
                        profile,
                        unlock,
                        chkUseExpiry.Checked ? dtpExpiry.Value : (DateTime?)null);

                    MessageBox.Show("Tạo user thành công.");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(password) || !string.IsNullOrWhiteSpace(confirm))
                    {
                        if (password != confirm)
                        {
                            MessageBox.Show("Confirm mật khẩu không khớp.");
                            return;
                        }
                    }

                    _service.AlterUser(
                        username,
                        password,
                        defaultTs,
                        tempTs,
                        profile,
                        unlock);

                    MessageBox.Show("Cập nhật user thành công.");
                }

                // FindForm()?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu user: " + ex.Message);
            }
        }
    }
}