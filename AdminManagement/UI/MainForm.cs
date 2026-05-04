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
        private FlowLayoutPanel nav;

        private string currentUser = "";
        private string currentRole = "";

        private Button btnUserList;
        private Button btnUserCreate;
        private Button btnRoleList;
        private Button btnRoleCreate;
        private Button btnGrant;
        private Button btnRevoke;
        private Button btnViewPrivilege;

        private Button btnThongTinTaiKhoan;
        private Button btnCapNhatThongTin;
        private Button btnHoSoBenhAn;
        private Button btnDichVuChiDinh;
        private Button btnDonThuoc;
        private Button btnBenhNhanDieuTri;

        private Button btnCongViecKTV;
        private Button btnCapNhatKetQua;
        private Button btnQuanLyBenhNhan;
        private Button btnTaoHSBA;
        private Button btnDieuPhoi;
        private Button btnThongBaoOLS;


        public MainForm()
        {
            currentUser = GetCurrentUser();
            currentRole = GetUserRole(currentUser);

            InitializeUi();
            CreateMenuByRole();

            if (currentRole == "OLS")
                LoadControl(new UcThongBaoOLS());
            else
                LoadControl(new UcThongTinTaiKhoan());
        }

        private string GetCurrentUser()
        {
            try
            {
                return Convert.ToString(
                    OracleDBAdmin.Data.OracleDb.ExecuteScalar("SELECT USER FROM dual")
                );
            }
            catch
            {
                return "";
            }
        }

        private string GetUserRole(string username)
        {
            if (username == "SYS" || username == "SYSTEM" || username == "C##ADMINBV")
                return "ADMIN";

            if (username.StartsWith("BN"))
                return "BENHNHAN";

            if (username.StartsWith("BS"))
                return "BACSI";

            if (username.StartsWith("KTV"))
                return "KTV";

            if (username.StartsWith("DPV"))
                return "DPV";

            if (username.StartsWith("U"))
                return "OLS";

            return "UNKNOWN";
        }

        private void InitializeUi()
        {
            Text = GetWindowTitle();
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(244, 242, 236);

            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 78,
                Padding = new Padding(15, 15, 15, 10),
                BackColor = Color.FromArgb(244, 242, 236)
            };

            nav = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight
            };

            pnlTop.Controls.Add(nav);

            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            Controls.Add(pnlContent);
            Controls.Add(pnlTop);
        }

        private string GetWindowTitle()
        {
            if (currentRole == "ADMIN")
                return "Hệ thống quản lý dữ liệu y tế - Quản trị CSDL";

            return "Hệ thống quản lý dữ liệu y tế";
        }

        private void CreateMenuByRole()
        {
            nav.Controls.Clear();

            btnThongTinTaiKhoan = CreateTabButton(
                "Thông tin cá nhân",
                (s, e) => LoadControl(new UcThongTinTaiKhoan())
            );

            if (currentRole == "ADMIN")
            {
                CreateAdminMenu();
            }
            else if (currentRole == "BENHNHAN")
            {
                CreateBenhNhanMenu();
            }
            else if (currentRole == "BACSI")
            {
                CreateBacSiMenu();
            }
            else if (currentRole == "KTV")
            {
                CreateKtvMenu();
            }
            else if (currentRole == "DPV")
            {
                CreateDpvMenu();
            }
            else if (currentRole == "OLS")
            {
                CreateOlsMenu();
            }
            else
            {
                nav.Controls.Add(btnThongTinTaiKhoan);
                AddThongBaoOlsButton();
            }
        }

        private void CreateAdminMenu()
        {
            btnUserList = CreateTabButton("Danh sách User", (s, e) => LoadControl(new UcUserList()));
            btnUserCreate = CreateTabButton("Tạo mới User", (s, e) => LoadControl(new UcUserEditor()));
            btnRoleList = CreateTabButton("Danh sách Role", (s, e) => LoadControl(new UcRoleList()));
            btnRoleCreate = CreateTabButton("Tạo mới Role", (s, e) => LoadControl(new UcRoleEditor()));
            btnGrant = CreateTabButton("Cấp quyền", (s, e) => LoadControl(new UcGrantPrivilege()));
            btnRevoke = CreateTabButton("Thu hồi quyền", (s, e) => LoadControl(new UcRevokePrivilege()));
            btnViewPrivilege = CreateTabButton("Xem quyền", (s, e) => LoadControl(new UcViewPrivilege()));

            nav.Controls.Add(btnUserList);
            nav.Controls.Add(btnUserCreate);
            nav.Controls.Add(btnRoleList);
            nav.Controls.Add(btnRoleCreate);
            nav.Controls.Add(btnGrant);
            nav.Controls.Add(btnRevoke);
            nav.Controls.Add(btnViewPrivilege);
            AddThongBaoOlsButton();

            nav.Controls.Add(btnThongTinTaiKhoan);
        }

        private void CreateBenhNhanMenu()
        {
            btnCapNhatThongTin = CreateTabButton(
                "Cập nhật thông tin",
                (s, e) => LoadControl(new UcCapNhatThongTin())
            );

            nav.Controls.Add(btnThongTinTaiKhoan);
            nav.Controls.Add(btnCapNhatThongTin);
            AddThongBaoOlsButton();
        }

        private void CreateBacSiMenu()
        {
            btnHoSoBenhAn = CreateTabButton(
                "Hồ sơ phụ trách",
                (s, e) => LoadControl(new UcHoSoBacSi())
            );

            btnBenhNhanDieuTri = CreateTabButton(
                "Bệnh nhân điều trị",
                (s, e) => LoadControl(new UcBenhNhanBacSi())
            );

            btnDichVuChiDinh = CreateTabButton(
                "Dịch vụ chỉ định",
                (s, e) => LoadControl(new UcDichVuChiDinh())
            );

            btnDonThuoc = CreateTabButton(
                "Đơn thuốc",
                (s, e) => LoadControl(new UcDonThuoc())
            );

            btnCapNhatThongTin = CreateTabButton(
                "Cập nhật thông tin",
                (s, e) => LoadControl(new UcCapNhatThongTin())
            );

            nav.Controls.Add(btnThongTinTaiKhoan);
            nav.Controls.Add(btnCapNhatThongTin);
            nav.Controls.Add(btnHoSoBenhAn);
            nav.Controls.Add(btnBenhNhanDieuTri);
            nav.Controls.Add(btnDichVuChiDinh);
            nav.Controls.Add(btnDonThuoc);
            AddThongBaoOlsButton();
        }

        private void CreateKtvMenu()
        {
            btnCongViecKTV = CreateTabButton(
                "Công việc KTV",
                (s, e) => LoadControl(new UcCongViecKTV())
            );

            btnCapNhatThongTin = CreateTabButton(
                "Cập nhật thông tin",
                (s, e) => LoadControl(new UcCapNhatThongTin())
            );

            nav.Controls.Add(btnThongTinTaiKhoan);
            nav.Controls.Add(btnCapNhatThongTin);
            nav.Controls.Add(btnCongViecKTV);
            AddThongBaoOlsButton();
        }

        private void CreateDpvMenu()
        {
            btnQuanLyBenhNhan = CreateTabButton(
                "Quản lý bệnh nhân",
                (s, e) => LoadControl(new UcDieuPhoiVien(0))
            );

            btnTaoHSBA = CreateTabButton(
                "Tạo HSBA",
                (s, e) => LoadControl(new UcDieuPhoiVien(1))
            );

            btnDieuPhoi = CreateTabButton(
                "Điều phối",
                (s, e) => LoadControl(new UcDieuPhoiVien(2))
            );

            btnCapNhatThongTin = CreateTabButton(
                "Cập nhật thông tin",
                (s, e) => LoadControl(new UcCapNhatThongTin())
            );

            nav.Controls.Add(btnThongTinTaiKhoan);
            nav.Controls.Add(btnCapNhatThongTin);
            nav.Controls.Add(btnQuanLyBenhNhan);
            nav.Controls.Add(btnTaoHSBA);
            nav.Controls.Add(btnDieuPhoi);
            AddThongBaoOlsButton();
        }

        private void CreateOlsMenu()
        {
            AddThongBaoOlsButton();
        }

        private void AddThongBaoOlsButton()
        {
            btnThongBaoOLS = CreateTabButton(
                "Thông báo OLS",
                (s, e) => LoadControl(new UcThongBaoOLS())
            );
            nav.Controls.Add(btnThongBaoOLS);
        }

        private Button CreateTabButton(string text, EventHandler click)
        {
            Button btn = new Button
            {
                Text = text,
                Width = 165,
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

        private void ShowComingSoon(string message)
        {
            MessageBox.Show(
                message,
                "Chức năng sẽ cài đặt ở câu tiếp theo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
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