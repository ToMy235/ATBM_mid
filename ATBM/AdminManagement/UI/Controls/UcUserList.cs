using OracleDBAdmin.Services;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcUserList : UserControl
    {
        private readonly UserService _service = new UserService();

        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnRefresh;
        private Button btnCreate;
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
        private readonly Color SuccessColor = Color.FromArgb(22, 163, 74);
        private readonly Color SuccessHoverColor = Color.FromArgb(21, 128, 61);
        private readonly Color BorderColor = Color.FromArgb(191, 219, 254);
        private readonly Color TextColor = Color.FromArgb(15, 23, 42);
        private readonly Color MutedTextColor = Color.FromArgb(100, 116, 139);
        private readonly Color HintBgColor = Color.FromArgb(239, 246, 255);

        public UcUserList()
        {
            InitializeUi();
            this.Load += (s, e) => LoadData();
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
                Text = "Danh sách User",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = TextColor,
                Location = new Point(0, 0)
            };

            lblHint = new Label
            {
                Text = "Theo dõi, tìm kiếm, tạo mới, chỉnh sửa và xóa các tài khoản user trong hệ thống Oracle.",
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

            sectionFilter = CreateSectionPanel("Tìm kiếm và thao tác", 160);
            sectionGrid = CreateSectionPanel("Danh sách user hiện có", 540);

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

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.5f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.5f));

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            txtSearch = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10.5F),
                Height = 38,
                Margin = new Padding(8, 0, 8, 18),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnSearch = CreatePrimaryButton("Tìm kiếm");
            btnSearch.Size = new Size(110, 40);
            btnSearch.Click += (s, e) => SearchData();

            btnCreate = CreateSuccessButton("+ Tạo mới");
            btnCreate.Size = new Size(120, 40);
            btnCreate.Click += (s, e) =>
            {
                var f = new Form
                {
                    Width = 900,
                    Height = 520,
                    Text = "Tạo User",
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = SurfaceColor
                };

                var uc = new UcUserEditor();
                uc.Dock = DockStyle.Fill;
                f.Controls.Add(uc);
                f.ShowDialog();
                LoadData();
            };

            btnRefresh = CreateSecondaryButton("Làm mới");
            btnRefresh.Size = new Size(110, 40);
            btnRefresh.Click += (s, e) => LoadData();

            AddField(layout, "Từ khóa tìm kiếm", txtSearch, 0, 0);

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Margin = new Padding(8, 28, 8, 0)
            };

            buttonPanel.Controls.Add(btnSearch);
            buttonPanel.Controls.Add(btnCreate);
            buttonPanel.Controls.Add(btnRefresh);

            layout.Controls.Add(buttonPanel, 1, 1);
            layout.SetColumnSpan(buttonPanel, 3);

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
                Text = "Gợi ý: nhập username rồi bấm Tìm kiếm để lọc nhanh danh sách user.",
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
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                ReadOnly = true,
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

            dgv.CellContentClick += Dgv_CellContentClick;

            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 4, 0, 0)
            };

            wrapper.Controls.Add(dgv);
            sectionGrid.Controls.Add(wrapper);
            wrapper.BringToFront();
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

        private Button CreateSuccessButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = SuccessColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 12, 0)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = SuccessHoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = SuccessColor;

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
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
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

        private void LoadData()
        {
            try
            {
                var dt = _service.GetUsers();
                dgv.DataSource = dt;
                AddActionColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách user: " + ex.Message, "Lỗi Kết Nối",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (this.FindForm() is MainForm main)
                {
                    main.IsReturningToLogin = true;
                    main.Close();
                }
            }
        }

        private void SearchData()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadData();
                    return;
                }

                var dt = _service.SearchUsers(txtSearch.Text);
                dgv.DataSource = dt;
                AddActionColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm user: " + ex.Message);
            }
        }

        private void AddActionColumns()
        {
            if (!dgv.Columns.Contains("EDIT_BTN"))
            {
                dgv.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "EDIT_BTN",
                    HeaderText = "Sửa",
                    Text = "Sửa",
                    UseColumnTextForButtonValue = true,
                    FillWeight = 70
                });
            }

            if (!dgv.Columns.Contains("DELETE_BTN"))
            {
                dgv.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "DELETE_BTN",
                    HeaderText = "Xóa",
                    Text = "Xóa",
                    UseColumnTextForButtonValue = true,
                    FillWeight = 70
                });
            }
        }

        private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string username = dgv.Rows[e.RowIndex].Cells["USERNAME"].Value.ToString();

            if (dgv.Columns[e.ColumnIndex].Name == "EDIT_BTN")
            {
                var f = new Form
                {
                    Width = 900,
                    Height = 520,
                    Text = "Sửa User",
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = SurfaceColor
                };

                var uc = new UcUserEditor(username);
                uc.Dock = DockStyle.Fill;
                f.Controls.Add(uc);
                f.ShowDialog();
                LoadData();
            }
            else if (dgv.Columns[e.ColumnIndex].Name == "DELETE_BTN")
            {
                if (MessageBox.Show($"Xóa user {username}?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        _service.DropUser(username);
                        MessageBox.Show("Xóa user thành công.");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xóa user: " + ex.Message);
                    }
                }
            }
        }
    }
}