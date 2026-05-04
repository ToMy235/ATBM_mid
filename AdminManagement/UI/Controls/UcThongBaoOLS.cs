using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcThongBaoOLS : UserControl
    {
        private Label lblCurrentUser;
        private Label lblRoleInfo;
        private Label lblLabelInfo;
        private Label lblCount;
        private Label lblStatus;

        private Button btnRefresh;
        private Button btnGuide;

        private DataGridView dgvThongBao;
        private Panel pnlEmpty;

        private string currentUser = "";

        public UcThongBaoOLS()
        {
            InitializeUi();
            LoadThongBao();
        }

        private void InitializeUi()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(22),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));   // Header
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));  // User cards
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));   // Toolbar
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Table

            root.Controls.Add(CreateHeader(), 0, 0);
            root.Controls.Add(CreateInfoCards(), 0, 1);
            root.Controls.Add(CreateToolbar(), 0, 2);
            root.Controls.Add(CreateContentArea(), 0, 3);

            Controls.Add(root);
        }

        private Control CreateHeader()
        {
            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            var title = new Label
            {
                Text = "Thông báo nội bộ",
                Dock = DockStyle.Top,
                Height = 34,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 45, 64)
            };

            var subtitle = new Label
            {
                Text = "Danh sách thông báo được phân phối theo Oracle Label Security (OLS)",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(95, 105, 120)
            };

            var line = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Color.FromArgb(220, 225, 232)
            };

            header.Controls.Add(line);
            header.Controls.Add(subtitle);
            header.Controls.Add(title);

            return header;
        }

        private Control CreateInfoCards()
        {
            var cards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 8, 0, 8)
            };

            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            lblCurrentUser = new Label();
            lblRoleInfo = new Label();
            lblLabelInfo = new Label();

            cards.Controls.Add(CreateCard("Tài khoản đăng nhập", lblCurrentUser, "Người dùng hiện tại của hệ thống"), 0, 0);
            cards.Controls.Add(CreateCard("Vai trò nghiệp vụ", lblRoleInfo, "Vai trò dùng để nhận thông báo"), 1, 0);
            cards.Controls.Add(CreateCard("Nhãn OLS", lblLabelInfo, "Nhãn bảo mật được gán cho user"), 2, 0);

            return cards;
        }

        private Panel CreateCard(string title, Label valueLabel, string description)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(14),
                Margin = new Padding(0, 0, 12, 0)
            };

            var lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(105, 115, 130)
            };

            valueLabel.Text = "...";
            valueLabel.Dock = DockStyle.Top;
            valueLabel.Height = 30;
            valueLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            valueLabel.ForeColor = Color.FromArgb(35, 75, 135);

            var lblDesc = new Label
            {
                Text = description,
                Dock = DockStyle.Top,
                Height = 24,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(120, 130, 145)
            };

            card.Controls.Add(lblDesc);
            card.Controls.Add(valueLabel);
            card.Controls.Add(lblTitle);

            return card;
        }

        private Control CreateToolbar()
        {
            var toolbar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 8, 0, 8)
            };

            btnRefresh = CreatePrimaryButton("Làm mới");
            btnRefresh.Location = new Point(0, 8);
            btnRefresh.Click += (s, e) => LoadThongBao();

            lblCount = new Label
            {
                Text = "0 thông báo",
                AutoSize = false,
                Width = 160,
                Height = 34,
                Location = new Point(260, 8),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 75, 135)
            };

            lblStatus = new Label
            {
                Text = "Dữ liệu được lọc tự động bởi Oracle OLS.",
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Width = 420,
                Height = 34,
                Location = new Point(toolbar.Width - 420, 8),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 110, 125)
            };

            toolbar.Resize += (s, e) =>
            {
                lblStatus.Location = new Point(toolbar.Width - lblStatus.Width, 8);
            };

            toolbar.Controls.Add(btnRefresh);
            toolbar.Controls.Add(btnGuide);
            toolbar.Controls.Add(lblCount);
            toolbar.Controls.Add(lblStatus);

            return toolbar;
        }

        private Control CreateContentArea()
        {
            var content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(1)
            };

            dgvThongBao = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(230, 234, 240),
                EnableHeadersVisualStyles = false
            };

            dgvThongBao.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(236, 240, 246);
            dgvThongBao.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(35, 45, 60);
            dgvThongBao.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvThongBao.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvThongBao.ColumnHeadersHeight = 38;

            dgvThongBao.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dgvThongBao.DefaultCellStyle.ForeColor = Color.FromArgb(45, 55, 70);
            dgvThongBao.DefaultCellStyle.SelectionBackColor = Color.FromArgb(218, 232, 252);
            dgvThongBao.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 45, 65);
            dgvThongBao.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvThongBao.RowTemplate.Height = 34;
            dgvThongBao.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            pnlEmpty = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };

            var emptyTitle = new Label
            {
                Text = "Không có thông báo phù hợp",
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 105),
                Padding = new Padding(0, 80, 0, 0)
            };

            var emptyDesc = new Label
            {
                Text = "User hiện tại không có nhãn OLS phù hợp hoặc bảng THONGBAO chưa có dữ liệu.",
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 34,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(120, 130, 145)
            };

            pnlEmpty.Controls.Add(emptyDesc);
            pnlEmpty.Controls.Add(emptyTitle);

            content.Controls.Add(dgvThongBao);
            content.Controls.Add(pnlEmpty);

            return content;
        }

        private Button CreatePrimaryButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Width = 100,
                Height = 34,
                BackColor = Color.FromArgb(35, 100, 190),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Button CreateSecondaryButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Width = 130,
                Height = 34,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(35, 75, 135),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderColor = Color.FromArgb(190, 205, 225);
            return btn;
        }

        private void LoadThongBao()
        {
            try
            {
                currentUser = Convert.ToString(OracleDb.ExecuteScalar("SELECT USER FROM dual"));
                UpdateUserInfo(currentUser);

                DataTable dt;

                try
                {
                    dt = OracleDb.ExecuteQuery(@"
                        SELECT ID,
                               NOIDUNG AS ""Nội dung thông báo"",
                               TO_CHAR(NGAYGIO, 'DD/MM/YYYY HH24:MI:SS') AS ""Ngày giờ"",
                               DIADIEM AS ""Địa điểm"",
                               LABEL_TO_CHAR(OLS_LABEL) AS ""Nhãn OLS""
                        FROM C##ADMINBV.THONGBAO
                        ORDER BY ID");
                }
                catch
                {
                    dt = OracleDb.ExecuteQuery(@"
                        SELECT ID,
                               NOIDUNG AS ""Nội dung thông báo"",
                               TO_CHAR(NGAYGIO, 'DD/MM/YYYY HH24:MI:SS') AS ""Ngày giờ"",
                               DIADIEM AS ""Địa điểm""
                        FROM C##ADMINBV.THONGBAO
                        ORDER BY ID");
                }

                dgvThongBao.DataSource = dt;
                FormatGridColumns();

                lblCount.Text = dt.Rows.Count + " thông báo";
                pnlEmpty.Visible = dt.Rows.Count == 0;
                dgvThongBao.Visible = dt.Rows.Count > 0;

                if (dt.Rows.Count == 0)
                {
                    lblStatus.Text = "Không có dữ liệu hiển thị cho user hiện tại.";
                }
                else
                {
                    lblStatus.Text = "OLS đã lọc và trả về các thông báo được phép đọc.";
                }
            }
            catch (Exception ex)
            {
                dgvThongBao.DataSource = null;
                dgvThongBao.Visible = false;
                pnlEmpty.Visible = true;

                lblCount.Text = "0 thông báo";
                lblStatus.Text = "Lỗi khi tải dữ liệu.";

                MessageBox.Show(
                    "Không thể tải danh sách thông báo.\n\nChi tiết lỗi:\n" + ex.Message,
                    "Lỗi tải thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void FormatGridColumns()
        {
            if (dgvThongBao.Columns.Count == 0)
                return;

            if (dgvThongBao.Columns.Contains("ID"))
            {
                dgvThongBao.Columns["ID"].Width = 60;
                dgvThongBao.Columns["ID"].FillWeight = 45;
                dgvThongBao.Columns["ID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvThongBao.Columns.Contains("Nội dung thông báo"))
            {
                dgvThongBao.Columns["Nội dung thông báo"].FillWeight = 230;
            }

            if (dgvThongBao.Columns.Contains("Ngày giờ"))
            {
                dgvThongBao.Columns["Ngày giờ"].FillWeight = 110;
            }

            if (dgvThongBao.Columns.Contains("Địa điểm"))
            {
                dgvThongBao.Columns["Địa điểm"].FillWeight = 80;
            }

            if (dgvThongBao.Columns.Contains("Nhãn OLS"))
            {
                dgvThongBao.Columns["Nhãn OLS"].FillWeight = 150;
            }
        }

        private void UpdateUserInfo(string user)
        {
            var info = GetUserInfo(user);

            lblCurrentUser.Text = info.UserName;
            lblRoleInfo.Text = info.RoleName;
            lblLabelInfo.Text = info.OlsLabel;
        }

        private OlsUserInfo GetUserInfo(string user)
        {
            switch ((user ?? "").ToUpper())
            {
                case "U1":
                    return new OlsUserInfo("U1", "Ban Giám đốc", "BGD:TH,TK,TM:HCM,HN,HP");

                case "U2":
                    return new OlsUserInfo("U2", "Lãnh đạo Khoa Tim mạch - HCM", "LDK:TM:HCM");

                case "U3":
                    return new OlsUserInfo("U3", "Lãnh đạo Khoa Thần kinh - Hà Nội", "LDK:TK:HN");

                case "U4":
                    return new OlsUserInfo("U4", "Nhân viên Khoa Thần kinh - HCM", "NV:TK:HCM");

                case "U5":
                    return new OlsUserInfo("U5", "Nhân viên Khoa Tim mạch - HCM", "NV:TM:HCM");

                case "U6":
                    return new OlsUserInfo("U6", "Lãnh đạo phòng/Khoa Tim mạch - HCM", "LDK:TM:HCM");

                case "U7":
                    return new OlsUserInfo("U7", "Lãnh đạo phòng toàn hệ thống", "LDK:TH,TK,TM:HCM,HN,HP");

                case "U8":
                    return new OlsUserInfo("U8", "Nhân viên Khoa Tiêu hóa - Hà Nội", "NV:TH:HN");

                case "C##ADMINBV":
                    return new OlsUserInfo("C##ADMINBV", "Quản trị hệ thống", "Nhãn quản trị/kiểm tra dữ liệu");

                default:
                    return new OlsUserInfo(user, "Không xác định", "Không thuộc nhóm U1-U8");
            }
        }

        private class OlsUserInfo
        {
            public string UserName { get; private set; }
            public string RoleName { get; private set; }
            public string OlsLabel { get; private set; }

            public OlsUserInfo(string userName, string roleName, string olsLabel)
            {
                UserName = userName;
                RoleName = roleName;
                OlsLabel = olsLabel;
            }
        }
    }
}