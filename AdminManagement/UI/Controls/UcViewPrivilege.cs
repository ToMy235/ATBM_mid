using OracleDBAdmin.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcViewPrivilege : UserControl
    {
        private readonly UserService _userService = new UserService();
        private readonly RoleService _roleService = new RoleService();
        private readonly PrivilegeService _service = new PrivilegeService();

        private ComboBox cboType;
        private ComboBox cboName;
        private Button btnView;

        private DataGridView dgvObject;
        private DataGridView dgvSystem;
        private DataGridView dgvRole;

        private Panel outerCard;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel sectionFilter;
        private Panel sectionObject;
        private Panel sectionSystem;
        private Panel sectionRole;

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

        public UcViewPrivilege()
        {
            InitializeUi();
            LoadNames();
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
                Text = "Xem quyền hệ thống",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(0, 0)
            };

            lblHint = new Label
            {
                Text = "Chọn User hoặc Role để xem quyền đối tượng, quyền hệ thống và danh sách role đã được cấp.",
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

            sectionFilter = CreateSectionPanel("Bộ lọc xem quyền", 160);
            sectionObject = CreateSectionPanel("Quyền đối tượng (Object Privileges)", 260);
            sectionSystem = CreateSectionPanel("Quyền hệ thống (System Privileges)", 220);
            sectionRole = CreateSectionPanel("Roles đã được cấp", 220);

            InitializeFilterSection();
            InitializeObjectSection();
            InitializeSystemSection();
            InitializeRoleSection();

            sectionRole.Dock = DockStyle.Top;
            sectionSystem.Dock = DockStyle.Top;
            sectionObject.Dock = DockStyle.Top;
            sectionFilter.Dock = DockStyle.Top;

            contentPanel.Controls.Add(sectionRole);
            contentPanel.Controls.Add(sectionSystem);
            contentPanel.Controls.Add(sectionObject);
            contentPanel.Controls.Add(sectionFilter);

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

        private void InitializeFilterSection()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 3,
                RowCount = 2,
                Height = 90,
                BackColor = Color.Transparent
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            cboType = CreateComboBox();
            cboType.Items.AddRange(new object[] { "User", "Role" });
            cboType.SelectedIndex = 0;
            cboType.SelectedIndexChanged += (s, e) => LoadNames();

            cboName = CreateComboBox();

            btnView = CreatePrimaryButton("Xem quyền");
            btnView.Size = new Size(130, 40);
            btnView.Click += BtnView_Click;

            AddField(layout, "Loại đối tượng", cboType, 0, 0);
            AddField(layout, "Tên User/Role", cboName, 1, 0);

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Margin = new Padding(8, 28, 8, 0)
            };
            buttonPanel.Controls.Add(btnView);

            layout.Controls.Add(buttonPanel, 2, 1);

            var infoPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = HintBgColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblInfo = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Gợi ý: đổi giữa User và Role để xem quyền được cấp tương ứng.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = PrimaryColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 12, 0)
            };

            infoPanel.Controls.Add(lblInfo);

            sectionFilter.Controls.Add(infoPanel);
            sectionFilter.Controls.Add(layout);
            layout.BringToFront();
        }

        private void InitializeObjectSection()
        {
            dgvObject = CreateStyledGrid();
            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 4, 0, 0)
            };
            wrapper.Controls.Add(dgvObject);
            sectionObject.Controls.Add(wrapper);
            wrapper.BringToFront();
        }

        private void InitializeSystemSection()
        {
            dgvSystem = CreateStyledGrid();
            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 4, 0, 0)
            };
            wrapper.Controls.Add(dgvSystem);
            sectionSystem.Controls.Add(wrapper);
            wrapper.BringToFront();
        }

        private void InitializeRoleSection()
        {
            dgvRole = CreateStyledGrid();
            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 4, 0, 0)
            };
            wrapper.Controls.Add(dgvRole);
            sectionRole.Controls.Add(wrapper);
            wrapper.BringToFront();
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

        private DataGridView CreateStyledGrid()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = BorderColor,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EnableHeadersVisualStyles = false
            };

            dgv.ColumnHeadersDefaultCellStyle.BackColor = HeaderBgColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 40;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = TextColor;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            return dgv;
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

        private void LoadNames()
        {
            if (cboType.SelectedItem?.ToString() == "User")
            {
                var dt = _userService.GetAllUsersForCombo();
                cboName.DataSource = dt;
                cboName.DisplayMember = "USERNAME";
                cboName.ValueMember = "USERNAME";
            }
            else
            {
                var dt = _roleService.GetAllRolesForCombo();
                cboName.DataSource = dt;
                cboName.DisplayMember = "ROLE";
                cboName.ValueMember = "ROLE";
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            try
            {
                string grantee = cboName.SelectedValue?.ToString();
                dgvObject.DataSource = _service.GetObjectPrivileges(grantee);
                dgvSystem.DataSource = _service.GetSystemPrivileges(grantee);
                dgvRole.DataSource = _service.GetRolePrivileges(grantee);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xem quyền: " + ex.Message);
            }
        }
    }
}