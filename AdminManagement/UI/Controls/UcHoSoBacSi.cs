using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcHoSoBacSi : UserControl
    {
        private DataGridView dgv;
        private TextBox txtChanDoan;
        private TextBox txtDieuTri;
        private TextBox txtKetLuan;
        private Button btnLamMoi;
        private Button btnCapNhat;

        public UcHoSoBacSi()
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
                Text = "HỒ SƠ BỆNH ÁN PHỤ TRÁCH",
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

            Label lblChanDoan = new Label { Text = "Chẩn đoán:", Left = 20, Top = 420, Width = 100 };
            txtChanDoan = new TextBox { Left = 130, Top = 420, Width = 600 };

            Label lblDieuTri = new Label { Text = "Điều trị:", Left = 20, Top = 470, Width = 100 };
            txtDieuTri = new TextBox { Left = 130, Top = 470, Width = 600 };

            Label lblKetLuan = new Label { Text = "Kết luận:", Left = 20, Top = 510, Width = 100 };
            txtKetLuan = new TextBox { Left = 130, Top = 510, Width = 600 };

            btnLamMoi = new Button
            {
                Text = "Làm mới",
                Left = 130,
                Top = 550,
                Width = 130,
                Height = 38
            };

            btnCapNhat = new Button
            {
                Text = "Cập nhật hồ sơ",
                Left = 280,
                Top = 550,
                Width = 160,
                Height = 38
            };

            Controls.Add(btnCapNhat);
            Controls.Add(btnLamMoi);
            Controls.Add(txtKetLuan);
            Controls.Add(lblKetLuan);
            Controls.Add(txtDieuTri);
            Controls.Add(lblDieuTri);
            Controls.Add(txtChanDoan);
            Controls.Add(lblChanDoan);
            Controls.Add(dgv);
            Controls.Add(title);

            btnLamMoi.Click += (s, e) => LoadData();
            btnCapNhat.Click += (s, e) => CapNhatHoSo();

            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow == null) return;

                txtChanDoan.Text = Convert.ToString(dgv.CurrentRow.Cells["CHANDUAN"].Value);
                txtDieuTri.Text = Convert.ToString(dgv.CurrentRow.Cells["DIEUTRI"].Value);
                txtKetLuan.Text = Convert.ToString(dgv.CurrentRow.Cells["KETLUAN"].Value);
            };
        }

        private void LoadData()
        {
            try
            {
                string sql = @"
                    SELECT MAHSBA, MABN, NGAY, CHANDUAN, DIEUTRI, MABS, MAKHOA, KETLUAN
                    FROM HSBA
                    ORDER BY NGAY DESC";

                dgv.DataSource = OracleDb.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải hồ sơ bệnh án:\n" + ex.Message);
            }
        }

        private void CapNhatHoSo()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn hồ sơ bệnh án.");
                return;
            }

            try
            {
                string sql = @"
                    UPDATE HSBA
                    SET CHANDUAN = :CHANDUAN,
                        DIEUTRI = :DIEUTRI,
                        KETLUAN = :KETLUAN
                    WHERE MAHSBA = :MAHSBA";

                OracleParameter[] p =
                {
                    new OracleParameter("CHANDUAN", txtChanDoan.Text),
                    new OracleParameter("DIEUTRI", txtDieuTri.Text),
                    new OracleParameter("KETLUAN", txtKetLuan.Text),
                    new OracleParameter("MAHSBA", dgv.CurrentRow.Cells["MAHSBA"].Value)
                };

                int rows = OracleDb.ExecuteNonQuery(sql, p);

                MessageBox.Show(rows > 0 ? "Cập nhật hồ sơ thành công." : "Không có dòng nào được cập nhật.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật hồ sơ:\n" + ex.Message);
            }
        }
    }
}