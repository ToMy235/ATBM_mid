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
        private Button btnCongViecKTV;
        private Button btnCapNhatKetQua;
        private Button btnQuanLyBenhNhan;
        private Button btnTaoHSBA;
        private Button btnDieuPhoi;

        public MainForm()
        {
            currentUser = GetCurrentUser();
            currentRole = GetUserRole(currentUser);

            InitializeUi();
            CreateMenuByRole();

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
            else
            {
                nav.Controls.Add(btnThongTinTaiKhoan);
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
        }

        private void CreateBacSiMenu()
        {
            btnHoSoBenhAn = CreateTabButton(
                "Hồ sơ phụ trách",
                (s, e) => ShowComingSoon("Chức năng này sẽ dùng VPD để bác sĩ chỉ xem hồ sơ bệnh án mình phụ trách.")
            );

            btnDichVuChiDinh = CreateTabButton(
                "Dịch vụ chỉ định",
                (s, e) => ShowComingSoon("Chức năng này sẽ cho bác sĩ thêm/xóa dịch vụ HSBA_DV thuộc hồ sơ mình phụ trách.")
            );

            btnDonThuoc = CreateTabButton(
                "Đơn thuốc",
                (s, e) => ShowComingSoon("Chức năng này sẽ cho bác sĩ quản lý đơn thuốc thuộc hồ sơ mình phụ trách.")
            );
            btnCapNhatThongTin = CreateTabButton(
                "Cập nhật thông tin",
                (s, e) => LoadControl(new UcCapNhatThongTin())
            );

            nav.Controls.Add(btnThongTinTaiKhoan);
            nav.Controls.Add(btnCapNhatThongTin);
            nav.Controls.Add(btnHoSoBenhAn);
            nav.Controls.Add(btnDichVuChiDinh);
            nav.Controls.Add(btnDonThuoc);
        }

        private void CreateKtvMenu()
        {
            btnCongViecKTV = CreateTabButton(
                "Công việc được phân công",
                (s, e) => ShowComingSoon("Chức năng này sẽ đọc VIEW_CONGVIEC_KTV để KTV chỉ thấy dịch vụ được phân công.")
            );

            btnCapNhatKetQua = CreateTabButton(
                "Cập nhật kết quả",
                (s, e) => ShowComingSoon("Chức năng này sẽ cập nhật trường KETQUA trên VIEW_CONGVIEC_KTV.")
            );
            btnCapNhatThongTin = CreateTabButton(
                "Cập nhật thông tin",
                (s, e) => LoadControl(new UcCapNhatThongTin())
            );

            nav.Controls.Add(btnThongTinTaiKhoan);
            nav.Controls.Add(btnCapNhatThongTin);
            nav.Controls.Add(btnCongViecKTV);
            nav.Controls.Add(btnCapNhatKetQua);
        }

        private void CreateDpvMenu()
        {
            btnQuanLyBenhNhan = CreateTabButton(
                "Quản lý bệnh nhân",
                (s, e) => ShowComingSoon("Điều phối viên có thể xem, thêm và sửa dữ liệu bệnh nhân.")
            );

            btnTaoHSBA = CreateTabButton(
                "Tạo hồ sơ bệnh án",
                (s, e) => ShowComingSoon("Điều phối viên có thể tạo mới hồ sơ bệnh án.")
            );

            btnDieuPhoi = CreateTabButton(
                "Điều phối",
                (s, e) => ShowComingSoon("Điều phối viên cập nhật bác sĩ, khoa và kỹ thuật viên phụ trách.")
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