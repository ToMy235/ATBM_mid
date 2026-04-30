using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcDonThuoc : UserControl
    {
        private DataGridView dgv;
        private TextBox txtMaHsba;
        private TextBox txtTenThuoc;
        private TextBox txtLieuDung;
        private Button btnLamMoi;
        private Button btnThem;
        private Button btnCapNhat;
        private Button btnXoa;

        private string tenThuocCu = "";

        public UcDonThuoc()
        {
            InitializeUi();
            LoadData();
        }

        private void InitializeUi()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            Padding = new Padding(20);

            Label title = new Label
            {
                Text = "QUẢN LÝ ĐƠN THUỐC",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 16, FontStyle.Bold)
            };

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 330,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            Label lblMaHsba = new Label { Text = "Mã HSBA:", Left = 20, Top = 420, Width = 100 };
            txtMaHsba = new TextBox { Left = 130, Top = 420, Width = 250 };

            Label lblTenThuoc = new Label { Text = "Tên thuốc:", Left = 20, Top = 470, Width = 100 };
            txtTenThuoc = new TextBox { Left = 130, Top = 470, Width = 250 };

            Label lblLieuDung = new Label { Text = "Liều dùng:", Left = 20, Top = 510, Width = 100 };
            txtLieuDung = new TextBox { Left = 130, Top = 510, Width = 250 };

            btnLamMoi = new Button { Text = "Làm mới", Left = 130, Top = 550, Width = 120, Height = 38 };
            btnThem = new Button { Text = "Thêm thuốc", Left = 270, Top = 550, Width = 130, Height = 38 };
            btnCapNhat = new Button { Text = "Cập nhật", Left = 420, Top = 550, Width = 130, Height = 38 };
            btnXoa = new Button { Text = "Xóa thuốc", Left = 570, Top = 550, Width = 130, Height = 38 };

            Controls.Add(btnXoa);
            Controls.Add(btnCapNhat);
            Controls.Add(btnThem);
            Controls.Add(btnLamMoi);
            Controls.Add(txtLieuDung);
            Controls.Add(lblLieuDung);
            Controls.Add(txtTenThuoc);
            Controls.Add(lblTenThuoc);
            Controls.Add(txtMaHsba);
            Controls.Add(lblMaHsba);
            Controls.Add(dgv);
            Controls.Add(title);

            btnLamMoi.Click += (s, e) => LoadData();
            btnThem.Click += (s, e) => ThemThuoc();
            btnCapNhat.Click += (s, e) => CapNhatThuoc();
            btnXoa.Click += (s, e) => XoaThuoc();

            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow == null) return;

                txtMaHsba.Text = Convert.ToString(dgv.CurrentRow.Cells["MAHSBA"].Value);
                txtTenThuoc.Text = Convert.ToString(dgv.CurrentRow.Cells["TENTHUOC"].Value);
                txtLieuDung.Text = Convert.ToString(dgv.CurrentRow.Cells["LIEUDUNG"].Value);
                tenThuocCu = txtTenThuoc.Text;
            };
        }

        private void LoadData()
        {
            try
            {
                string sql = @"
                    SELECT MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG
                    FROM DONTHUOC
                    ORDER BY NGAYDT DESC";

                dgv.DataSource = OracleDb.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải đơn thuốc:\n" + ex.Message);
            }
        }

        private void ThemThuoc()
        {
            if (string.IsNullOrWhiteSpace(txtMaHsba.Text) ||
                string.IsNullOrWhiteSpace(txtTenThuoc.Text) ||
                string.IsNullOrWhiteSpace(txtLieuDung.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ mã HSBA, tên thuốc và liều dùng.");
                return;
            }

            try
            {
                string sql = @"
                    INSERT INTO DONTHUOC(MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG)
                    VALUES(:MAHSBA, SYSDATE, :TENTHUOC, :LIEUDUNG)";

                OracleParameter[] p =
                {
                    new OracleParameter("MAHSBA", txtMaHsba.Text.Trim()),
                    new OracleParameter("TENTHUOC", txtTenThuoc.Text.Trim()),
                    new OracleParameter("LIEUDUNG", txtLieuDung.Text.Trim())
                };

                OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show("Thêm thuốc thành công.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm thuốc:\n" + ex.Message);
            }
        }

        private void CapNhatThuoc()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn thuốc cần cập nhật.");
                return;
            }

            try
            {
                string sql = @"
                    UPDATE DONTHUOC
                    SET TENTHUOC = :TENTHUOC_MOI,
                        LIEUDUNG = :LIEUDUNG
                    WHERE MAHSBA = :MAHSBA
                      AND NGAYDT = :NGAYDT
                      AND TENTHUOC = :TENTHUOC_CU";

                OracleParameter[] p =
                {
                    new OracleParameter("TENTHUOC_MOI", txtTenThuoc.Text.Trim()),
                    new OracleParameter("LIEUDUNG", txtLieuDung.Text.Trim()),
                    new OracleParameter("MAHSBA", dgv.CurrentRow.Cells["MAHSBA"].Value),
                    new OracleParameter("NGAYDT", dgv.CurrentRow.Cells["NGAYDT"].Value),
                    new OracleParameter("TENTHUOC_CU", tenThuocCu)
                };

                int rows = OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show(rows > 0 ? "Cập nhật thuốc thành công." : "Không có dòng nào được cập nhật.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật thuốc:\n" + ex.Message);
            }
        }

        private void XoaThuoc()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn thuốc cần xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa thuốc này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                string sql = @"
                    DELETE FROM DONTHUOC
                    WHERE MAHSBA = :MAHSBA
                      AND NGAYDT = :NGAYDT
                      AND TENTHUOC = :TENTHUOC";

                OracleParameter[] p =
                {
                    new OracleParameter("MAHSBA", dgv.CurrentRow.Cells["MAHSBA"].Value),
                    new OracleParameter("NGAYDT", dgv.CurrentRow.Cells["NGAYDT"].Value),
                    new OracleParameter("TENTHUOC", dgv.CurrentRow.Cells["TENTHUOC"].Value)
                };

                int rows = OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show(rows > 0 ? "Xóa thuốc thành công." : "Không có dòng nào được xóa.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa thuốc:\n" + ex.Message);
            }
        }
    }
}