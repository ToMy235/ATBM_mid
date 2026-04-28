using OracleDBAdmin.Services;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcRevokePrivilege : UserControl
    {
        private readonly UserService _userService = new UserService();
        private readonly RoleService _roleService = new RoleService();
        private readonly PrivilegeService _service = new PrivilegeService();

        private ComboBox cboType;
        private ComboBox cboName;
        private Button btnLoad;
        private Button btnRevoke;
        private DataGridView dgv;

        private Panel outerCard;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel sectionFilter;
        private Panel sectionGrid;

        private Label lblTitle;
        private Label lblHint;

        private readonly Color BgColor = Color.FromArgb(236, 246, 255);
        private readonly Color SurfaceColor = Color.White;
        private readonly Color HeaderBgColor = Color.FromArgb(219, 234, 254);
        private readonly Color SectionBgColor = Color.FromArgb(248, 252, 255);
        private readonly Color PrimaryColor = Color.FromArgb(37, 99, 235);
        private readonly Color PrimaryHoverColor = Color.FromArgb(29, 78, 216);
        private readonly Color DangerColor = Color.FromArgb(220, 38, 38);
        private readonly Color DangerHoverColor = Color.FromArgb(185, 28, 28);
        private readonly Color BorderColor = Color.FromArgb(191, 219, 254);
        private readonly Color TextColor = Color.FromArgb(15, 23, 42);
        private readonly Color MutedTextColor = Color.FromArgb(100, 116, 139);
        private readonly Color HintBgColor = Color.FromArgb(239, 246, 255);

        public UcRevokePrivilege()
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
                Text = "Thu hồi quyền hệ thống",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(0, 0)
            };

            lblHint = new Label
            {
                Text = "Chọn User hoặc Role, tải danh sách quyền hiện có, sau đó đánh dấu các quyền cần thu hồi.",
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

            sectionFilter = CreateSectionPanel("Bộ lọc và thao tác", 170);
            sectionGrid = CreateSectionPanel("Danh sách quyền hiện có", 520);

            InitializeFilterSection();
            InitializeGridSection();

            sectionGrid.Dock = DockStyle.Top;
            sectionFilter.Dock = DockStyle.Top;

            contentPanel.Controls.Add(sectionGrid);
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
                ColumnCount = 4,
                RowCount = 2,
                Height = 90,
                BackColor = Color.Transparent
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            cboType = CreateComboBox();
            cboType.Items.AddRange(new object[] { "User", "Role" });
            cboType.SelectedIndex = 0;
            cboType.SelectedIndexChanged += (s, e) => LoadNames();

            cboName = CreateComboBox();

            AddField(layout, "Loại đối tượng", cboType, 0, 0);
            AddField(layout, "Tên User/Role", cboName, 1, 0);

            btnLoad = CreatePrimaryButton("Tải danh sách quyền");
            btnLoad.Size = new Size(180, 40);
            btnLoad.Click += (s, e) => LoadPrivileges();

            btnRevoke = CreateDangerButton("Thu hồi đã chọn");
            btnRevoke.Size = new Size(170, 40);
            btnRevoke.Click += BtnRevoke_Click;

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Margin = new Padding(8, 28, 8, 0)
            };
            buttonPanel.Controls.Add(btnLoad);
            buttonPanel.Controls.Add(btnRevoke);

            layout.Controls.Add(buttonPanel, 2, 1);
            layout.SetColumnSpan(buttonPanel, 2);

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
                Text = "Gợi ý: tích chọn nhiều dòng trong bảng để thu hồi nhiều quyền cùng lúc.",
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

        private void InitializeGridSection()
        {
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = false,
                AutoGenerateColumns = true,
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

            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 4, 0, 0)
            };

            wrapper.Controls.Add(dgv);
            sectionGrid.Controls.Add(wrapper);
            wrapper.BringToFront();
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
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 12, 0)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = PrimaryHoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = PrimaryColor;

            return btn;
        }

        private Button CreateDangerButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = DangerColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = DangerHoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = DangerColor;

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

        private void LoadPrivileges()
        {
            try
            {
                string grantee = cboName.SelectedValue?.ToString();
                var dt = _service.GetPrivilegeListForRevoke(grantee);
                dgv.DataSource = dt;

                if (!dgv.Columns.Contains("CHK"))
                {
                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn
                    {
                        Name = "CHK",
                        HeaderText = "Chọn",
                        FillWeight = 50
                    };
                    dgv.Columns.Insert(0, chk);
                }

                dgv.Columns["CHK"].DisplayIndex = 0;

                if (dgv.Columns.Contains("CATEGORY"))
                    dgv.Columns["CATEGORY"].HeaderText = "Loại";

                if (dgv.Columns.Contains("PRIVILEGE"))
                    dgv.Columns["PRIVILEGE"].HeaderText = "Quyền";

                if (dgv.Columns.Contains("OWNER"))
                    dgv.Columns["OWNER"].HeaderText = "Schema";

                if (dgv.Columns.Contains("OBJECT_NAME"))
                    dgv.Columns["OBJECT_NAME"].HeaderText = "Đối tượng";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách quyền: " + ex.Message);
            }
        }

        private void BtnRevoke_Click(object sender, EventArgs e)
        {
            try
            {
                string grantee = cboName.SelectedValue?.ToString();
                if (string.IsNullOrWhiteSpace(grantee))
                {
                    MessageBox.Show("Chưa chọn user/role.");
                    return;
                }

                int count = 0;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    bool selected = row.Cells["CHK"].Value != null && Convert.ToBoolean(row.Cells["CHK"].Value);
                    if (!selected) continue;

                    string category = row.Cells["CATEGORY"].Value?.ToString();
                    string priv = row.Cells["PRIVILEGE"].Value?.ToString();

                    if (category == "SYSTEM")
                    {
                        _service.RevokeSystemPrivilege(priv, grantee);
                    }
                    else if (category == "ROLE")
                    {
                        _service.RevokeRole(priv, grantee);
                    }
                    else
                    {
                        string owner = row.Cells["OWNER"].Value?.ToString();
                        string obj = row.Cells["OBJECT_NAME"].Value?.ToString();
                        _service.RevokeObjectPrivilege(priv, owner, obj, grantee);
                    }

                    count++;
                }

                MessageBox.Show(count > 0 ? "Thu hồi quyền thành công." : "Chưa chọn dòng nào.");
                LoadPrivileges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi REVOKE: " + ex.Message);
            }
        }
    }
}