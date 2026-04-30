using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcDieuPhoiVien : UserControl
    {
        private TabControl tabs;

        private DataGridView dgvBenhNhan;
        private TextBox txtMaBn, txtTenBn, txtPhai, txtCccd, txtSoNha, txtTenDuong, txtQuanHuyen, txtTinhTp, txtTienSu, txtTienSuGd, txtDiUng;
        private DateTimePicker dtNgaySinh;

        private DataGridView dgvHsba;
        private TextBox txtMaHsba, txtHsbaMaBn, txtMaBs, txtMaKhoa;

        private DataGridView dgvDv;
        private TextBox txtDpMaHsba, txtDpMaBs, txtDpMaKhoa, txtDvMaHsba, txtDvLoaiDv, txtDvMaKtv;
        private DateTimePicker dtDvNgay;

        public UcDieuPhoiVien(int selectedTab = 0)
        {
            InitializeUi();

            if (selectedTab >= 0 && selectedTab < tabs.TabPages.Count)
                tabs.SelectedIndex = selectedTab;

            LoadBenhNhan();
            LoadHsba();
            LoadDichVu();
        }

        private void InitializeUi()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            Padding = new Padding(15);

            tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            tabs.TabPages.Add(CreateTabBenhNhan());
            tabs.TabPages.Add(CreateTabTaoHsba());
            tabs.TabPages.Add(CreateTabDieuPhoi());

            Controls.Add(tabs);
        }

        private TabPage CreateTabBenhNhan()
        {
            TabPage tab = new TabPage("Quản lý bệnh nhân");

            dgvBenhNhan = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 280,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            Panel form = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            int y = 15;
            txtMaBn = AddText(form, "Mã BN", 20, y); y += 35;
            txtTenBn = AddText(form, "Tên BN", 20, y); y += 35;
            txtPhai = AddText(form, "Phái", 20, y); y += 35;

            Label lblNgaySinh = new Label { Text = "Ngày sinh", Left = 20, Top = y + 5, Width = 100 };
            dtNgaySinh = new DateTimePicker { Left = 130, Top = y, Width = 220, Format = DateTimePickerFormat.Short };
            form.Controls.Add(lblNgaySinh);
            form.Controls.Add(dtNgaySinh);
            y += 35;

            txtCccd = AddText(form, "CCCD", 20, y); y += 35;
            txtSoNha = AddText(form, "Số nhà", 20, y); y += 35;
            txtTenDuong = AddText(form, "Tên đường", 20, y); y += 35;
            txtQuanHuyen = AddText(form, "Quận huyện", 20, y); y += 35;
            txtTinhTp = AddText(form, "Tỉnh TP", 20, y); y += 35;
            txtTienSu = AddText(form, "Tiền sử bệnh", 20, y); y += 35;
            txtTienSuGd = AddText(form, "Tiền sử bệnh GĐ", 20, y); y += 35;
            txtDiUng = AddText(form, "Dị ứng thuốc", 20, y); y += 45;

            Button btnLamMoi = new Button { Text = "Làm mới", Left = 130, Top = y, Width = 120, Height = 36 };
            Button btnThem = new Button { Text = "Thêm bệnh nhân", Left = 270, Top = y, Width = 150, Height = 36 };
            Button btnCapNhat = new Button { Text = "Cập nhật", Left = 440, Top = y, Width = 130, Height = 36 };

            form.Controls.Add(btnLamMoi);
            form.Controls.Add(btnThem);
            form.Controls.Add(btnCapNhat);

            tab.Controls.Add(form);
            tab.Controls.Add(dgvBenhNhan);

            btnLamMoi.Click += (s, e) => LoadBenhNhan();
            btnThem.Click += (s, e) => ThemBenhNhan();
            btnCapNhat.Click += (s, e) => CapNhatBenhNhan();

            dgvBenhNhan.SelectionChanged += (s, e) => FillBenhNhanForm();

            return tab;
        }

        private TabPage CreateTabTaoHsba()
        {
            TabPage tab = new TabPage("Tạo hồ sơ bệnh án");

            dgvHsba = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 320,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            Panel form = new Panel { Dock = DockStyle.Fill };

            txtMaHsba = AddText(form, "Mã HSBA", 20, 25);
            txtHsbaMaBn = AddText(form, "Mã BN", 20, 65);
            txtMaBs = AddText(form, "Mã bác sĩ", 20, 105);
            txtMaKhoa = AddText(form, "Mã khoa", 20, 145);

            Button btnLamMoi = new Button { Text = "Làm mới", Left = 130, Top = 200, Width = 120, Height = 36 };
            Button btnTao = new Button { Text = "Tạo HSBA", Left = 270, Top = 200, Width = 140, Height = 36 };

            form.Controls.Add(btnLamMoi);
            form.Controls.Add(btnTao);

            tab.Controls.Add(form);
            tab.Controls.Add(dgvHsba);

            btnLamMoi.Click += (s, e) => LoadHsba();
            btnTao.Click += (s, e) => TaoHsba();

            dgvHsba.SelectionChanged += (s, e) =>
            {
                if (dgvHsba.CurrentRow == null) return;

                txtMaHsba.Text = Convert.ToString(dgvHsba.CurrentRow.Cells["MAHSBA"].Value);
                txtHsbaMaBn.Text = Convert.ToString(dgvHsba.CurrentRow.Cells["MABN"].Value);
                txtMaBs.Text = Convert.ToString(dgvHsba.CurrentRow.Cells["MABS"].Value);
                txtMaKhoa.Text = Convert.ToString(dgvHsba.CurrentRow.Cells["MAKHOA"].Value);
            };

            return tab;
        }

        private TabPage CreateTabDieuPhoi()
        {
            TabPage tab = new TabPage("Điều phối");

            Panel panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            Label lbl1 = new Label
            {
                Text = "Phân công bác sĩ / khoa cho hồ sơ bệnh án",
                Left = 20,
                Top = 20,
                Width = 500,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            txtDpMaHsba = AddText(panel, "Mã HSBA", 20, 55);
            txtDpMaBs = AddText(panel, "Mã bác sĩ", 20, 95);
            txtDpMaKhoa = AddText(panel, "Mã khoa", 20, 135);

            Button btnPhanCongBs = new Button
            {
                Text = "Phân công bác sĩ/khoa",
                Left = 130,
                Top = 180,
                Width = 200,
                Height = 36
            };

            Label lbl2 = new Label
            {
                Text = "Phân công kỹ thuật viên cho dịch vụ",
                Left = 20,
                Top = 240,
                Width = 500,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            txtDvMaHsba = AddText(panel, "Mã HSBA", 20, 275);
            txtDvLoaiDv = AddText(panel, "Loại DV", 20, 315);

            Label lblNgay = new Label { Text = "Ngày DV", Left = 20, Top = 360, Width = 100 };
            dtDvNgay = new DateTimePicker { Left = 130, Top = 355, Width = 220, Format = DateTimePickerFormat.Short };
            panel.Controls.Add(lblNgay);
            panel.Controls.Add(dtDvNgay);

            txtDvMaKtv = AddText(panel, "Mã KTV", 20, 395);

            Button btnPhanCongKtv = new Button
            {
                Text = "Phân công KTV",
                Left = 130,
                Top = 440,
                Width = 160,
                Height = 36
            };

            dgvDv = new DataGridView
            {
                Left = 430,
                Top = 55,
                Width = 700,
                Height = 420,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            Button btnLamMoiDv = new Button
            {
                Text = "Làm mới danh sách dịch vụ",
                Left = 430,
                Top = 490,
                Width = 220,
                Height = 36
            };

            panel.Controls.Add(lbl1);
            panel.Controls.Add(btnPhanCongBs);
            panel.Controls.Add(lbl2);
            panel.Controls.Add(btnPhanCongKtv);
            panel.Controls.Add(dgvDv);
            panel.Controls.Add(btnLamMoiDv);

            tab.Controls.Add(panel);

            btnPhanCongBs.Click += (s, e) => PhanCongBacSi();
            btnPhanCongKtv.Click += (s, e) => PhanCongKtv();
            btnLamMoiDv.Click += (s, e) => LoadDichVu();

            dgvDv.SelectionChanged += (s, e) =>
            {
                if (dgvDv.CurrentRow == null) return;

                txtDvMaHsba.Text = Convert.ToString(dgvDv.CurrentRow.Cells["MAHSBA"].Value);
                txtDvLoaiDv.Text = Convert.ToString(dgvDv.CurrentRow.Cells["LOAIDV"].Value);
                txtDvMaKtv.Text = Convert.ToString(dgvDv.CurrentRow.Cells["MAKTV"].Value);

                object ngay = dgvDv.CurrentRow.Cells["NGAYDV"].Value;
                if (ngay != DBNull.Value && ngay != null)
                    dtDvNgay.Value = Convert.ToDateTime(ngay);
            };

            return tab;
        }

        private TextBox AddText(Control parent, string label, int x, int y)
        {
            Label lbl = new Label { Text = label, Left = x, Top = y + 5, Width = 100 };
            TextBox txt = new TextBox { Left = x + 110, Top = y, Width = 220 };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            return txt;
        }

        private void LoadBenhNhan()
        {
            try
            {
                dgvBenhNhan.DataSource = OracleDb.ExecuteQuery("SELECT * FROM BENHNHAN ORDER BY MABN");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bệnh nhân:\n" + ex.Message);
            }
        }

        private void FillBenhNhanForm()
        {
            if (dgvBenhNhan.CurrentRow == null) return;

            txtMaBn.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["MABN"].Value);
            txtTenBn.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["TENBN"].Value);
            txtPhai.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["PHAI"].Value);

            object ns = dgvBenhNhan.CurrentRow.Cells["NGAYSINH"].Value;
            if (ns != DBNull.Value && ns != null)
                dtNgaySinh.Value = Convert.ToDateTime(ns);

            txtCccd.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["CCCD"].Value);
            txtSoNha.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["SONHA"].Value);
            txtTenDuong.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["TENDUONG"].Value);
            txtQuanHuyen.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["QUANHUYEN"].Value);
            txtTinhTp.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["TINHTP"].Value);
            txtTienSu.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["TIENSUBENH"].Value);
            txtTienSuGd.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["TIENSUBENHGD"].Value);
            txtDiUng.Text = Convert.ToString(dgvBenhNhan.CurrentRow.Cells["DIUNGTHUOC"].Value);
        }

        private void ThemBenhNhan()
        {
            try
            {
                string sql = @"
                    INSERT INTO BENHNHAN(
                        MABN, TENBN, PHAI, NGAYSINH, CCCD,
                        SONHA, TENDUONG, QUANHUYEN, TINHTP,
                        TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC
                    )
                    VALUES(
                        :MABN, :TENBN, :PHAI, :NGAYSINH, :CCCD,
                        :SONHA, :TENDUONG, :QUANHUYEN, :TINHTP,
                        :TIENSUBENH, :TIENSUBENHGD, :DIUNGTHUOC
                    )";

                OracleParameter[] p =
                {
                    new OracleParameter("MABN", txtMaBn.Text.Trim()),
                    new OracleParameter("TENBN", txtTenBn.Text.Trim()),
                    new OracleParameter("PHAI", txtPhai.Text.Trim()),
                    new OracleParameter("NGAYSINH", dtNgaySinh.Value.Date),
                    new OracleParameter("CCCD", txtCccd.Text.Trim()),
                    new OracleParameter("SONHA", txtSoNha.Text.Trim()),
                    new OracleParameter("TENDUONG", txtTenDuong.Text.Trim()),
                    new OracleParameter("QUANHUYEN", txtQuanHuyen.Text.Trim()),
                    new OracleParameter("TINHTP", txtTinhTp.Text.Trim()),
                    new OracleParameter("TIENSUBENH", txtTienSu.Text.Trim()),
                    new OracleParameter("TIENSUBENHGD", txtTienSuGd.Text.Trim()),
                    new OracleParameter("DIUNGTHUOC", txtDiUng.Text.Trim())
                };

                OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show("Thêm bệnh nhân thành công.");
                LoadBenhNhan();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm bệnh nhân:\n" + ex.Message);
            }
        }

        private void CapNhatBenhNhan()
        {
            if (string.IsNullOrWhiteSpace(txtMaBn.Text))
            {
                MessageBox.Show("Vui lòng chọn bệnh nhân.");
                return;
            }

            try
            {
                string sql = @"
                    UPDATE BENHNHAN
                    SET SONHA = :SONHA,
                        TENDUONG = :TENDUONG,
                        QUANHUYEN = :QUANHUYEN,
                        TINHTP = :TINHTP,
                        TIENSUBENH = :TIENSUBENH,
                        TIENSUBENHGD = :TIENSUBENHGD,
                        DIUNGTHUOC = :DIUNGTHUOC
                    WHERE MABN = :MABN";

                OracleParameter[] p =
                {
                    new OracleParameter("SONHA", txtSoNha.Text.Trim()),
                    new OracleParameter("TENDUONG", txtTenDuong.Text.Trim()),
                    new OracleParameter("QUANHUYEN", txtQuanHuyen.Text.Trim()),
                    new OracleParameter("TINHTP", txtTinhTp.Text.Trim()),
                    new OracleParameter("TIENSUBENH", txtTienSu.Text.Trim()),
                    new OracleParameter("TIENSUBENHGD", txtTienSuGd.Text.Trim()),
                    new OracleParameter("DIUNGTHUOC", txtDiUng.Text.Trim()),
                    new OracleParameter("MABN", txtMaBn.Text.Trim())
                };

                int rows = OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show(rows > 0 ? "Cập nhật bệnh nhân thành công." : "Không có dòng nào được cập nhật.");
                LoadBenhNhan();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật bệnh nhân:\n" + ex.Message);
            }
        }

        private void LoadHsba()
        {
            try
            {
                string sql = @"
                    SELECT MAHSBA, MABN, NGAY, CHANDUAN, DIEUTRI, MABS, MAKHOA, KETLUAN
                    FROM HSBA
                    ORDER BY NGAY DESC";

                dgvHsba.DataSource = OracleDb.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải HSBA:\n" + ex.Message);
            }
        }

        private void TaoHsba()
        {
            try
            {
                string sql = @"
                    INSERT INTO HSBA(MAHSBA, MABN, NGAY, CHANDUAN, DIEUTRI, MABS, MAKHOA, KETLUAN)
                    VALUES(:MAHSBA, :MABN, SYSDATE, NULL, NULL, :MABS, :MAKHOA, NULL)";

                OracleParameter[] p =
                {
                    new OracleParameter("MAHSBA", txtMaHsba.Text.Trim()),
                    new OracleParameter("MABN", txtHsbaMaBn.Text.Trim()),
                    new OracleParameter("MABS", txtMaBs.Text.Trim()),
                    new OracleParameter("MAKHOA", txtMaKhoa.Text.Trim())
                };

                OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show("Tạo hồ sơ bệnh án thành công.");
                LoadHsba();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo HSBA:\n" + ex.Message);
            }
        }

        private void LoadDichVu()
        {
            try
            {
                string sql = @"
                    SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA
                    FROM HSBA_DV
                    ORDER BY NGAYDV DESC";

                dgvDv.DataSource = OracleDb.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dịch vụ:\n" + ex.Message);
            }
        }

        private void PhanCongBacSi()
        {
            try
            {
                string sql = @"
                    UPDATE HSBA
                    SET MABS = :MABS,
                        MAKHOA = :MAKHOA
                    WHERE MAHSBA = :MAHSBA";

                OracleParameter[] p =
                {
                    new OracleParameter("MABS", txtDpMaBs.Text.Trim()),
                    new OracleParameter("MAKHOA", txtDpMaKhoa.Text.Trim()),
                    new OracleParameter("MAHSBA", txtDpMaHsba.Text.Trim())
                };

                int rows = OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show(rows > 0 ? "Phân công bác sĩ/khoa thành công." : "Không có dòng nào được cập nhật.");
                LoadHsba();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi phân công bác sĩ/khoa:\n" + ex.Message);
            }
        }

        private void PhanCongKtv()
        {
            try
            {
                string sql = @"
                    UPDATE HSBA_DV
                    SET MAKTV = :MAKTV
                    WHERE MAHSBA = :MAHSBA
                      AND LOAIDV = :LOAIDV
                      AND TRUNC(NGAYDV) = TRUNC(:NGAYDV)";

                OracleParameter[] p =
                {
                    new OracleParameter("MAKTV", txtDvMaKtv.Text.Trim()),
                    new OracleParameter("MAHSBA", txtDvMaHsba.Text.Trim()),
                    new OracleParameter("LOAIDV", txtDvLoaiDv.Text.Trim()),
                    new OracleParameter("NGAYDV", dtDvNgay.Value.Date)
                };

                int rows = OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show(rows > 0 ? "Phân công KTV thành công." : "Không có dòng nào được cập nhật.");
                LoadDichVu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi phân công KTV:\n" + ex.Message);
            }
        }
    }
}