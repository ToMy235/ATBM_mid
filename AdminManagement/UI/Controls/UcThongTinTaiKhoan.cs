using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcThongTinTaiKhoan : UserControl
    {
        private Panel pnlHeader;
        private Panel pnlInfo;
        private Panel pnlActions;
        private Panel pnlGrid;

        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblCurrentUser;
        private Label lblUserType;
        private Label lblStatus;

        private Button btnLoad;
        private Button btnRefresh;
        private DataGridView dgvData;

        public UcThongTinTaiKhoan()
        {
            InitializeUi();
            LoadUserInfo();
        }

        private void InitializeUi()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(248, 250, 252);

            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                Padding = new Padding(24, 18, 24, 12),
                BackColor = Color.White
            };

            lblTitle = new Label
            {
                Text = "HỒ SƠ CÁ NHÂN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175),
                AutoSize = true,
                Location = new Point(24, 18)
            };

            lblSubtitle = new Label
            {
                Text = "Thông tin được truy xuất theo tài khoản Oracle đang đăng nhập",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(26, 58)
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            pnlInfo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 86,
                Padding = new Padding(24, 12, 24, 8),
                BackColor = Color.White
            };

            lblCurrentUser = new Label
            {
                Text = "Tài khoản đăng nhập: ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 15)
            };

            lblUserType = new Label
            {
                Text = "Vai trò: ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(24, 45)
            };

            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(22, 163, 74),
                AutoSize = true,
                Location = new Point(420, 16)
            };

            pnlInfo.Controls.Add(lblCurrentUser);
            pnlInfo.Controls.Add(lblUserType);
            pnlInfo.Controls.Add(lblStatus);

            pnlActions = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                Padding = new Padding(24, 10, 24, 10),
                BackColor = Color.White
            };

            btnLoad = new Button
            {
                Text = "Xem hồ sơ",
                Location = new Point(24, 12),
                Size = new Size(140, 38),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Click += (s, e) => LoadUserInfo();

            btnRefresh = new Button
            {
                Text = "Làm mới",
                Location = new Point(178, 12),
                Size = new Size(120, 38),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(37, 99, 235),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnRefresh.Click += (s, e) => LoadUserInfo();

            pnlActions.Controls.Add(btnLoad);
            pnlActions.Controls.Add(btnRefresh);

            pnlGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 16, 24, 24),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            dgvData = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EnableHeadersVisualStyles = false
            };

            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(226, 232, 240);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvData.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvData.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvData.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);

            pnlGrid.Controls.Add(dgvData);

            Controls.Add(pnlGrid);
            Controls.Add(pnlActions);
            Controls.Add(pnlInfo);
            Controls.Add(pnlHeader);
        }

        private void LoadUserInfo()
        {
            dgvData.DataSource = null;
            lblStatus.Text = "";

            try
            {
                string currentUser = Convert.ToString(OracleDb.ExecuteScalar("SELECT USER FROM dual"));
                lblCurrentUser.Text = "Tài khoản đăng nhập: " + currentUser;

                if (currentUser.StartsWith("BN"))
                {
                    LoadBenhNhan();
                }
                else
                {
                    LoadNhanVien();
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Không thể tải hồ sơ.";
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBenhNhan()
        {
            try
            {
                string sql = @"
                    SELECT 
                        MABN AS ""Mã bệnh nhân"",
                        TENBN AS ""Họ tên"",
                        PHAI AS ""Phái"",
                        NGAYSINH AS ""Ngày sinh"",
                        CCCD AS ""CCCD"",
                        SONHA AS ""Số nhà"",
                        TENDUONG AS ""Tên đường"",
                        QUANHUYEN AS ""Quận/Huyện"",
                        TINHTP AS ""Tỉnh/TP"",
                        TIENSUBENH AS ""Tiền sử bệnh"",
                        TIENSUBENHGD AS ""Tiền sử bệnh gia đình"",
                        DIUNGTHUOC AS ""Dị ứng thuốc""
                    FROM VIEW_THONGTIN_BENHNHAN";

                DataTable dt = OracleDb.ExecuteQuery(sql);

                dgvData.DataSource = dt;
                lblUserType.Text = "Vai trò: Bệnh nhân";
                lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                lblStatus.Text = dt.Rows.Count > 0
                    ? "Đã tải hồ sơ bệnh nhân thành công."
                    : "Không tìm thấy hồ sơ bệnh nhân.";
            }
            catch (Exception ex)
            {
                lblUserType.Text = "Vai trò: Bệnh nhân";
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Không thể tải hồ sơ bệnh nhân.";
                MessageBox.Show(ex.Message, "Lỗi đọc hồ sơ bệnh nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadNhanVien()
        {
            try
            {
                string sql = @"
                    SELECT 
                        MANV AS ""Mã nhân viên"",
                        HOTEN AS ""Họ tên"",
                        PHAI AS ""Phái"",
                        NGAYSINH AS ""Ngày sinh"",
                        CMND AS ""CMND"",
                        QUEQUAN AS ""Quê quán"",
                        SODT AS ""Số điện thoại"",
                        VAITRO AS ""Vai trò"",
                        CHUYENKHOA AS ""Chuyên khoa""
                    FROM VIEW_THONGTIN_NHANVIEN";

                DataTable dt = OracleDb.ExecuteQuery(sql);

                dgvData.DataSource = dt;

                if (dt.Rows.Count > 0 && dt.Columns.Contains("Vai trò"))
                {
                    lblUserType.Text = "Vai trò: " + Convert.ToString(dt.Rows[0]["Vai trò"]);
                }
                else
                {
                    lblUserType.Text = "Vai trò: Nhân viên";
                }

                lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                lblStatus.Text = dt.Rows.Count > 0
                    ? "Đã tải hồ sơ nhân viên thành công."
                    : "Không tìm thấy hồ sơ nhân viên.";
            }
            catch (Exception ex)
            {
                lblUserType.Text = "Vai trò: Nhân viên";
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Không thể tải hồ sơ nhân viên.";
                MessageBox.Show(ex.Message, "Lỗi đọc hồ sơ nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}