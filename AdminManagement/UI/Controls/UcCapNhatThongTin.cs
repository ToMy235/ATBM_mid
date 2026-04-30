using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcCapNhatThongTin : UserControl
    {
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblCurrentUser;
        private Label lblRole;
        private Label lblStatus;

        private Panel pnlHeader;
        private Panel pnlForm;
        private Panel pnlButtons;

        private TextBox txtSonha;
        private TextBox txtTenduong;
        private TextBox txtQuanHuyen;
        private TextBox txtTinhTp;
        private TextBox txtTienSuBenh;
        private TextBox txtTienSuBenhGd;
        private TextBox txtDiUngThuoc;

        private TextBox txtQueQuan;
        private TextBox txtSoDt;

        private Button btnSave;
        private Button btnReload;

        private string currentUser = "";
        private bool isBenhNhan = false;

        public UcCapNhatThongTin()
        {
            InitializeUi();
            LoadCurrentInfo();
        }

        private void InitializeUi()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(248, 250, 252);

            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 135,
                Padding = new Padding(28, 18, 28, 12),
                BackColor = Color.White
            };

            lblTitle = new Label
            {
                Text = "CẬP NHẬT THÔNG TIN CÁ NHÂN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 64, 175),
                AutoSize = true,
                Location = new Point(28, 18)
            };

            lblSubtitle = new Label
            {
                Text = "Chỉ các trường được cấp quyền mới có thể cập nhật",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(30, 58)
            };

            lblCurrentUser = new Label
            {
                Text = "Tài khoản đăng nhập:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(30, 92)
            };

            lblRole = new Label
            {
                Text = "Vai trò:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(360, 92)
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblCurrentUser);
            pnlHeader.Controls.Add(lblRole);

            pnlForm = new Panel
            {
                Dock = DockStyle.Top,
                Height = 380,
                Padding = new Padding(28, 22, 28, 12),
                BackColor = Color.White
            };

            pnlButtons = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                Padding = new Padding(28, 10, 28, 10),
                BackColor = Color.White
            };

            btnSave = new Button
            {
                Text = "Lưu thay đổi",
                Location = new Point(28, 14),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => SaveInfo();

            btnReload = new Button
            {
                Text = "Tải lại",
                Location = new Point(192, 14),
                Size = new Size(120, 40),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(37, 99, 235),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnReload.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnReload.Click += (s, e) => LoadCurrentInfo();

            lblStatus = new Label
            {
                Text = "",
                Location = new Point(330, 24),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(22, 163, 74)
            };

            pnlButtons.Controls.Add(btnSave);
            pnlButtons.Controls.Add(btnReload);
            pnlButtons.Controls.Add(lblStatus);

            Controls.Add(pnlButtons);
            Controls.Add(pnlForm);
            Controls.Add(pnlHeader);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(170, 28),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            };
        }

        private TextBox CreateTextBox(int x, int y, int width = 280, bool multiline = false)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, multiline ? 70 : 28),
                Multiline = multiline,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void BuildBenhNhanForm()
        {
            pnlForm.Controls.Clear();

            int labelX1 = 20;
            int inputX1 = 190;
            int labelX2 = 520;
            int inputX2 = 700;

            pnlForm.Controls.Add(CreateLabel("Số nhà", labelX1, 25));
            txtSonha = CreateTextBox(inputX1, 22);
            pnlForm.Controls.Add(txtSonha);

            pnlForm.Controls.Add(CreateLabel("Tên đường", labelX2, 25));
            txtTenduong = CreateTextBox(inputX2, 22);
            pnlForm.Controls.Add(txtTenduong);

            pnlForm.Controls.Add(CreateLabel("Quận/Huyện", labelX1, 75));
            txtQuanHuyen = CreateTextBox(inputX1, 72);
            pnlForm.Controls.Add(txtQuanHuyen);

            pnlForm.Controls.Add(CreateLabel("Tỉnh/TP", labelX2, 75));
            txtTinhTp = CreateTextBox(inputX2, 72);
            pnlForm.Controls.Add(txtTinhTp);

            pnlForm.Controls.Add(CreateLabel("Tiền sử bệnh", labelX1, 130));
            txtTienSuBenh = CreateTextBox(inputX1, 127, 790, true);
            pnlForm.Controls.Add(txtTienSuBenh);

            pnlForm.Controls.Add(CreateLabel("Tiền sử bệnh GĐ", labelX1, 220));
            txtTienSuBenhGd = CreateTextBox(inputX1, 217, 790, true);
            pnlForm.Controls.Add(txtTienSuBenhGd);

            pnlForm.Controls.Add(CreateLabel("Dị ứng thuốc", labelX1, 310));
            txtDiUngThuoc = CreateTextBox(inputX1, 307, 790, true);
            pnlForm.Controls.Add(txtDiUngThuoc);
        }

        private void BuildNhanVienForm()
        {
            pnlForm.Controls.Clear();

            int labelX = 20;
            int inputX = 190;

            pnlForm.Controls.Add(CreateLabel("Quê quán", labelX, 30));
            txtQueQuan = CreateTextBox(inputX, 27, 420);
            pnlForm.Controls.Add(txtQueQuan);

            pnlForm.Controls.Add(CreateLabel("Số điện thoại", labelX, 85));
            txtSoDt = CreateTextBox(inputX, 82, 420);
            pnlForm.Controls.Add(txtSoDt);

            Label note = new Label
            {
                Text = "Các thông tin như mã nhân viên, họ tên, phái, ngày sinh, CMND, vai trò và chuyên khoa không được phép tự chỉnh sửa.",
                Location = new Point(28, 145),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            pnlForm.Controls.Add(note);
        }

        private void LoadCurrentInfo()
        {
            lblStatus.Text = "";

            try
            {
                currentUser = Convert.ToString(OracleDb.ExecuteScalar("SELECT USER FROM dual"));
                isBenhNhan = currentUser.StartsWith("BN");

                lblCurrentUser.Text = "Tài khoản đăng nhập: " + currentUser;

                if (isBenhNhan)
                {
                    lblRole.Text = "Vai trò: Bệnh nhân";
                    BuildBenhNhanForm();
                    LoadBenhNhanInfo();
                }
                else
                {
                    BuildNhanVienForm();
                    LoadNhanVienInfo();
                }
            }
            catch (Exception ex)
            {
                ShowError("Không thể tải thông tin.", ex);
            }
        }

        private void LoadBenhNhanInfo()
        {
            string sql = @"
        SELECT 
            TRIM(SONHA) AS SONHA,
            TRIM(TENDUONG) AS TENDUONG,
            TRIM(QUANHUYEN) AS QUANHUYEN,
            TRIM(TINHTP) AS TINHTP,
            TRIM(TIENSUBENH) AS TIENSUBENH,
            TRIM(TIENSUBENHGD) AS TIENSUBENHGD,
            TRIM(DIUNGTHUOC) AS DIUNGTHUOC
        FROM VIEW_THONGTIN_BENHNHAN";

            DataTable dt = OracleDb.ExecuteQuery(sql);

            if (dt.Rows.Count == 0)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Không tìm thấy thông tin bệnh nhân.";
                return;
            }

            DataRow row = dt.Rows[0];

            txtSonha.Text = Convert.ToString(row["SONHA"]).Trim();
            txtTenduong.Text = Convert.ToString(row["TENDUONG"]).Trim();
            txtQuanHuyen.Text = Convert.ToString(row["QUANHUYEN"]).Trim();
            txtTinhTp.Text = Convert.ToString(row["TINHTP"]).Trim();
            txtTienSuBenh.Text = Convert.ToString(row["TIENSUBENH"]).Trim();
            txtTienSuBenhGd.Text = Convert.ToString(row["TIENSUBENHGD"]).Trim();
            txtDiUngThuoc.Text = Convert.ToString(row["DIUNGTHUOC"]).Trim();

            lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblStatus.Text = "Đã tải thông tin hiện tại.";
        }

        private void LoadNhanVienInfo()
        {
            string sql = @"
                SELECT 
                    QUEQUAN,
                    SODT,
                    VAITRO
                FROM VIEW_THONGTIN_NHANVIEN";

            DataTable dt = OracleDb.ExecuteQuery(sql);

            if (dt.Rows.Count == 0)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Không tìm thấy thông tin nhân viên.";
                lblRole.Text = "Vai trò: Nhân viên";
                return;
            }

            DataRow row = dt.Rows[0];

            txtQueQuan.Text = Convert.ToString(row["QUEQUAN"]).Trim();
            txtSoDt.Text = Convert.ToString(row["SODT"]).Trim();
            lblRole.Text = "Vai trò: " + Convert.ToString(row["VAITRO"]).Trim();

            lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblStatus.Text = "Đã tải thông tin hiện tại.";
        }

        private void SaveInfo()
        {
            try
            {
                if (isBenhNhan)
                {
                    SaveBenhNhanInfo();
                }
                else
                {
                    SaveNhanVienInfo();
                }
            }
            catch (Exception ex)
            {
                ShowError("Cập nhật thất bại.", ex);
            }
        }

        private void SaveBenhNhanInfo()
        {
            string sql = @"
                UPDATE VIEW_THONGTIN_BENHNHAN
                SET 
                    SONHA = :sonha,
                    TENDUONG = :tenduong,
                    QUANHUYEN = :quanhuyen,
                    TINHTP = :tinhtp,
                    TIENSUBENH = :tiensubenh,
                    TIENSUBENHGD = :tiensubenhgd,
                    DIUNGTHUOC = :diungthuoc
                WHERE MABN = USER";

            OracleDb.ExecuteNonQuery(
                sql,
                new Oracle.ManagedDataAccess.Client.OracleParameter("sonha", txtSonha.Text.Trim()),
                new Oracle.ManagedDataAccess.Client.OracleParameter("tenduong", txtTenduong.Text.Trim()),
                new Oracle.ManagedDataAccess.Client.OracleParameter("quanhuyen", txtQuanHuyen.Text.Trim()),
                new Oracle.ManagedDataAccess.Client.OracleParameter("tinhtp", txtTinhTp.Text.Trim()),
                new Oracle.ManagedDataAccess.Client.OracleParameter("tiensubenh", txtTienSuBenh.Text.Trim()),
                new Oracle.ManagedDataAccess.Client.OracleParameter("tiensubenhgd", txtTienSuBenhGd.Text.Trim()),
                new Oracle.ManagedDataAccess.Client.OracleParameter("diungthuoc", txtDiUngThuoc.Text.Trim())
            );

            lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblStatus.Text = "Cập nhật thông tin bệnh nhân thành công.";
        }

        private void SaveNhanVienInfo()
        {
            string sql = @"
                UPDATE VIEW_THONGTIN_NHANVIEN
                SET 
                    QUEQUAN = :quequan,
                    SODT = :sodt
                WHERE MANV = USER";

            OracleDb.ExecuteNonQuery(
                sql,
                new Oracle.ManagedDataAccess.Client.OracleParameter("quequan", txtQueQuan.Text.Trim()),
                new Oracle.ManagedDataAccess.Client.OracleParameter("sodt", txtSoDt.Text.Trim())
            );

            lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
            lblStatus.Text = "Cập nhật thông tin nhân viên thành công.";
        }

        private void ShowError(string title, Exception ex)
        {
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            lblStatus.Text = title;
            MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}