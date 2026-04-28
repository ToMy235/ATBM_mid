using OracleDBAdmin.Services;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcGrantPrivilege : UserControl
    {
        private readonly UserService _userService = new UserService();
        private readonly RoleService _roleService = new RoleService();
        private readonly PrivilegeService _service = new PrivilegeService();

        private ComboBox cboTargetType;
        private ComboBox cboTargetName;
        private ComboBox cboGrantType;

        private ComboBox cboObjectType;
        private ComboBox cboSchema;
        private ComboBox cboObjectName;
        private ComboBox cboPrivilege;
        private ComboBox cboRole;

        private CheckedListBox clbColumns;
        private CheckBox chkAdminOption;
        private CheckBox chkGrantOption;
        private Button btnExecute;

        private Panel outerCard;
        private Panel headerPanel;
        private Panel contentPanel;

        private Panel sectionGrantInfo;
        private Panel sectionColumns;
        private Panel sectionOptions;

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

        public UcGrantPrivilege()
        {
            InitializeUi();
            LoadBaseData();
            RefreshVisibleState();
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
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(0)
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
                Text = "Cấp quyền hệ thống",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(0, 0)
            };

            lblHint = new Label
            {
                Text = "Thiết lập quyền cho User hoặc Role, chọn đúng loại GRANT và tùy chọn nâng cao trước khi thực hiện.",
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

            sectionGrantInfo = CreateSectionPanel("Thông tin cấp quyền", 330);
            sectionColumns = CreateSectionPanel("Cột áp dụng", 235);
            sectionOptions = CreateSectionPanel("Tùy chọn nâng cao", 170);

            InitializeGrantInfoSection();
            InitializeColumnsSection();
            InitializeOptionsSection();

            var footerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 74,
                Padding = new Padding(0, 14, 0, 0)
            };

            btnExecute = CreatePrimaryButton("Thực hiện GRANT");
            btnExecute.Size = new Size(210, 44);
            btnExecute.Click += BtnExecute_Click;
            footerPanel.Controls.Add(btnExecute);
            footerPanel.Resize += (s, e) =>
            {
                btnExecute.Left = footerPanel.ClientSize.Width - btnExecute.Width;
                btnExecute.Top = 10;
            };

            sectionGrantInfo.Dock = DockStyle.Top;
            sectionColumns.Dock = DockStyle.Top;
            sectionOptions.Dock = DockStyle.Top;
            footerPanel.Dock = DockStyle.Top;

            contentPanel.Controls.Add(footerPanel);
            contentPanel.Controls.Add(sectionOptions);
            contentPanel.Controls.Add(sectionColumns);
            contentPanel.Controls.Add(sectionGrantInfo);

            outerCard.Controls.Add(contentPanel);
            outerCard.Controls.Add(headerPanel);

            Controls.Clear();
            Controls.Add(outerCard);

            HookEvents();

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

        private void InitializeGrantInfoSection()
        {
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 6,
                BackColor = Color.Transparent
            };

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));

            for (int i = 0; i < 6; i++)
                mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            cboTargetType = CreateComboBox();
            cboTargetType.Items.AddRange(new object[] { "User", "Role" });
            cboTargetType.SelectedIndex = 0;

            cboTargetName = CreateComboBox();

            cboGrantType = CreateComboBox();
            cboGrantType.Items.AddRange(new object[] { "System Privilege", "Object Privilege", "Role" });
            cboGrantType.SelectedIndex = 0;

            cboObjectType = CreateComboBox();
            cboObjectType.Items.AddRange(new object[] { "TABLE", "VIEW", "PROCEDURE", "FUNCTION" });

            cboSchema = CreateComboBox();
            cboObjectName = CreateComboBox();
            cboPrivilege = CreateComboBox();
            cboRole = CreateComboBox();

            AddField(mainLayout, "Đối tượng được cấp", cboTargetType, 0, 0);
            AddField(mainLayout, "Tên User/Role", cboTargetName, 1, 0);
            AddField(mainLayout, "Loại quyền", cboGrantType, 2, 0);

            AddField(mainLayout, "Loại đối tượng", cboObjectType, 0, 2);
            AddField(mainLayout, "Schema", cboSchema, 1, 2);
            AddField(mainLayout, "Tên đối tượng", cboObjectName, 2, 2);

            AddField(mainLayout, "Privilege", cboPrivilege, 0, 4);
            AddField(mainLayout, "Role", cboRole, 1, 4);

            sectionGrantInfo.Controls.Add(mainLayout);
            mainLayout.BringToFront();
        }

        private void InitializeColumnsSection()
        {
            var lblColumnsHint = new Label
            {
                Text = "Áp dụng cho SELECT hoặc UPDATE. Không chọn cột thì hệ thống sẽ cấp theo toàn bộ đối tượng.",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = MutedTextColor,
                Location = new Point(22, 56)
            };

            clbColumns = new CheckedListBox
            {
                Location = new Point(22, 92),
                Size = new Size(sectionColumns.ClientSize.Width - 44, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                CheckOnClick = true,
                BackColor = Color.White,
                IntegralHeight = false
            };

            sectionColumns.Resize += (s, e) =>
            {
                clbColumns.Width = sectionColumns.ClientSize.Width - 44;
            };

            sectionColumns.Controls.Add(lblColumnsHint);
            sectionColumns.Controls.Add(clbColumns);
        }

        private void InitializeOptionsSection()
        {
            chkAdminOption = new CheckBox
            {
                Text = "WITH ADMIN OPTION",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(22, 58)
            };

            chkGrantOption = new CheckBox
            {
                Text = "WITH GRANT OPTION",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(245, 58)
            };

            var infoPanel = new Panel
            {
                Location = new Point(22, 92),
                Size = new Size(sectionOptions.ClientSize.Width - 44, 48),
                BackColor = HintBgColor,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            sectionOptions.Resize += (s, e) =>
            {
                infoPanel.Width = sectionOptions.ClientSize.Width - 44;
            };

            var lblInfo = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Gợi ý: System Privilege và Role thường dùng WITH ADMIN OPTION. Object Privilege dùng WITH GRANT OPTION.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = PrimaryColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 12, 0)
            };

            infoPanel.Controls.Add(lblInfo);

            sectionOptions.Controls.Add(chkAdminOption);
            sectionOptions.Controls.Add(chkGrantOption);
            sectionOptions.Controls.Add(infoPanel);
        }

        private void HookEvents()
        {
            cboTargetType.SelectedIndexChanged += (s, e) => LoadTargetNames();
            cboGrantType.SelectedIndexChanged += (s, e) => RefreshVisibleState();

            cboObjectType.SelectedIndexChanged += (s, e) =>
            {
                LoadObjectNames();
                FillObjectPrivileges();
                RefreshVisibleState();
            };

            cboSchema.SelectedIndexChanged += (s, e) => LoadObjectNames();
            cboObjectName.SelectedIndexChanged += (s, e) => LoadColumns();
            cboPrivilege.SelectedIndexChanged += (s, e) => RefreshVisibleState();
        }

        private ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 38,
                Font = new Font("Segoe UI", 10.5F),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(8, 0, 8, 18)
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

        private void LoadBaseData()
        {
            LoadTargetNames();

            var schemas = _service.GetSchemas();
            cboSchema.DataSource = schemas;
            cboSchema.DisplayMember = "USERNAME";
            cboSchema.ValueMember = "USERNAME";

            var roles = _roleService.GetAllRolesForCombo();
            cboRole.DataSource = roles;
            cboRole.DisplayMember = "ROLE";
            cboRole.ValueMember = "ROLE";

            cboObjectType.SelectedIndex = 0;
            LoadObjectNames();
            FillSystemPrivileges();
        }

        private void LoadTargetNames()
        {
            if (cboTargetType.SelectedItem?.ToString() == "User")
            {
                var dt = _userService.GetAllUsersForCombo();
                cboTargetName.DataSource = dt;
                cboTargetName.DisplayMember = "USERNAME";
                cboTargetName.ValueMember = "USERNAME";
            }
            else
            {
                var dt = _roleService.GetAllRolesForCombo();
                cboTargetName.DataSource = dt;
                cboTargetName.DisplayMember = "ROLE";
                cboTargetName.ValueMember = "ROLE";
            }
        }

        private void FillSystemPrivileges()
        {
            cboPrivilege.DataSource = _service.GetCommonSystemPrivileges()
                .Select(x => new { NAME = x })
                .ToList();
            cboPrivilege.DisplayMember = "NAME";
            cboPrivilege.ValueMember = "NAME";
        }

        private void FillObjectPrivileges()
        {
            string type = cboObjectType.SelectedItem?.ToString() ?? "TABLE";
            string[] items;

            if (type == "TABLE" || type == "VIEW")
                items = new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "REFERENCES", "ALTER", "INDEX" };
            else
                items = new[] { "EXECUTE" };

            cboPrivilege.DataSource = items.Select(x => new { NAME = x }).ToList();
            cboPrivilege.DisplayMember = "NAME";
            cboPrivilege.ValueMember = "NAME";
        }

        private void LoadObjectNames()
        {
            if (cboSchema.SelectedValue == null || cboObjectType.SelectedItem == null)
                return;

            var dt = _service.GetObjectsByType(
                cboSchema.SelectedValue.ToString(),
                cboObjectType.SelectedItem.ToString());

            cboObjectName.DataSource = dt;
            cboObjectName.DisplayMember = "OBJECT_NAME";
            cboObjectName.ValueMember = "OBJECT_NAME";
        }

        private void LoadColumns()
        {
            clbColumns.Items.Clear();

            if (cboSchema.SelectedValue == null || cboObjectName.SelectedValue == null)
                return;

            string type = cboObjectType.SelectedItem?.ToString();
            if (type != "TABLE" && type != "VIEW")
                return;

            var dt = _service.GetColumns(
                cboSchema.SelectedValue.ToString(),
                cboObjectName.SelectedValue.ToString());

            foreach (DataRow row in dt.Rows)
                clbColumns.Items.Add(row["COLUMN_NAME"].ToString());
        }

        private void RefreshVisibleState()
        {
            string grantType = cboGrantType.SelectedItem?.ToString() ?? "";
            string privilege = cboPrivilege.Text;

            bool isObject = grantType == "Object Privilege";
            bool isRole = grantType == "Role";
            bool isSystem = grantType == "System Privilege";

            cboObjectType.Enabled = isObject;
            cboSchema.Enabled = isObject;
            cboObjectName.Enabled = isObject;

            cboRole.Enabled = isRole;
            cboPrivilege.Enabled = !isRole;

            chkAdminOption.Visible = isRole || isSystem;
            chkGrantOption.Visible = isObject;

            clbColumns.Enabled = isObject && (privilege == "SELECT" || privilege == "UPDATE");
            sectionColumns.Visible = isObject;
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                string target = cboTargetName.SelectedValue?.ToString();
                string grantType = cboGrantType.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(target))
                {
                    MessageBox.Show("Chưa chọn user/role nhận quyền.");
                    return;
                }

                if (grantType == "System Privilege")
                {
                    string privilege = cboPrivilege.SelectedValue?.ToString();
                    _service.GrantSystemPrivilege(privilege, target, chkAdminOption.Checked);
                }
                else if (grantType == "Role")
                {
                    string role = cboRole.SelectedValue?.ToString();
                    _service.GrantRole(role, target, chkAdminOption.Checked);
                }
                else
                {
                    string privilege = cboPrivilege.SelectedValue?.ToString();
                    string owner = cboSchema.SelectedValue?.ToString();
                    string objectName = cboObjectName.SelectedValue?.ToString();
                    string[] cols = clbColumns.CheckedItems.Cast<string>().ToArray();

                    _service.GrantObjectPrivilege(
                        privilege,
                        owner,
                        objectName,
                        target,
                        cols,
                        chkGrantOption.Checked);
                }

                MessageBox.Show("Cấp quyền thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi GRANT: " + ex.Message);
            }
        }
    }
}