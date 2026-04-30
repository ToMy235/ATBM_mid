using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcDichVuChiDinh : UserControl
    {
        private DataGridView dgv;
        private TextBox txtMaHsba;
        private TextBox txtLoaiDv;
        private Button btnLamMoi;
        private Button btnThem;
        private Button btnXoa;

        public UcDichVuChiDinh()
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
                Text = "DỊCH VỤ CHỈ ĐỊNH",
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

            Label lblMaHsba = new Label { Text = "Mã HSBA:", Left = 20, Top = 410, Width = 100 };
            txtMaHsba = new TextBox { Left = 130, Top = 410, Width = 250 };

            Label lblLoaiDv = new Label { Text = "Loại dịch vụ:", Left = 20, Top = 450, Width = 100 };
            txtLoaiDv = new TextBox { Left = 130, Top = 450, Width = 250 };

            btnLamMoi = new Button { Text = "Làm mới", Left = 130, Top = 500, Width = 120, Height = 38 };
            btnThem = new Button { Text = "Thêm dịch vụ", Left = 270, Top = 500, Width = 140, Height = 38 };
            btnXoa = new Button { Text = "Xóa dịch vụ", Left = 430, Top = 500, Width = 130, Height = 38 };

            Controls.Add(btnXoa);
            Controls.Add(btnThem);
            Controls.Add(btnLamMoi);
            Controls.Add(txtLoaiDv);
            Controls.Add(lblLoaiDv);
            Controls.Add(txtMaHsba);
            Controls.Add(lblMaHsba);
            Controls.Add(dgv);
            Controls.Add(title);

            btnLamMoi.Click += (s, e) => LoadData();
            btnThem.Click += (s, e) => ThemDichVu();
            btnXoa.Click += (s, e) => XoaDichVu();

            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow == null) return;

                txtMaHsba.Text = Convert.ToString(dgv.CurrentRow.Cells["MAHSBA"].Value);
                txtLoaiDv.Text = Convert.ToString(dgv.CurrentRow.Cells["LOAIDV"].Value);
            };
        }

        private void LoadData()
        {
            try
            {
                string sql = @"
                    SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA
                    FROM HSBA_DV
                    ORDER BY NGAYDV DESC";

                dgv.DataSource = OracleDb.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dịch vụ chỉ định:\n" + ex.Message);
            }
        }

        private void ThemDichVu()
        {
            if (string.IsNullOrWhiteSpace(txtMaHsba.Text) || string.IsNullOrWhiteSpace(txtLoaiDv.Text))
            {
                MessageBox.Show("Vui lòng nhập mã HSBA và loại dịch vụ.");
                return;
            }

            try
            {
                string sql = @"
                    INSERT INTO HSBA_DV(MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA)
                    VALUES(:MAHSBA, :LOAIDV, SYSDATE, NULL, NULL)";

                OracleParameter[] p =
                {
                    new OracleParameter("MAHSBA", txtMaHsba.Text.Trim()),
                    new OracleParameter("LOAIDV", txtLoaiDv.Text.Trim())
                };

                OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show("Thêm dịch vụ thành công.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm dịch vụ:\n" + ex.Message);
            }
        }

        private void XoaDichVu()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa dịch vụ này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                string sql = @"
                    DELETE FROM HSBA_DV
                    WHERE MAHSBA = :MAHSBA
                      AND LOAIDV = :LOAIDV
                      AND NGAYDV = :NGAYDV";

                OracleParameter[] p =
                {
                    new OracleParameter("MAHSBA", dgv.CurrentRow.Cells["MAHSBA"].Value),
                    new OracleParameter("LOAIDV", dgv.CurrentRow.Cells["LOAIDV"].Value),
                    new OracleParameter("NGAYDV", dgv.CurrentRow.Cells["NGAYDV"].Value)
                };

                int rows = OracleDb.ExecuteNonQuery(sql, p);
                MessageBox.Show(rows > 0 ? "Xóa dịch vụ thành công." : "Không có dòng nào được xóa.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa dịch vụ:\n" + ex.Message);
            }
        }
    }
}