using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcCongViecKTV : UserControl
    {
        private DataGridView dgv;
        private TextBox txtKetQua;
        private Button btnLamMoi;
        private Button btnCapNhat;

        public UcCongViecKTV()
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
                Text = "CÔNG VIỆC KỸ THUẬT VIÊN",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 16, FontStyle.Bold)
            };

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 360,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            Label lblKetQua = new Label
            {
                Text = "Kết quả:",
                Top = 420,
                Left = 20,
                Width = 100
            };

            txtKetQua = new TextBox
            {
                Top = 445,
                Left = 20,
                Width = 600,
                Height = 80,
                Multiline = true
            };

            btnLamMoi = new Button
            {
                Text = "Làm mới",
                Top = 540,
                Left = 20,
                Width = 130,
                Height = 38
            };

            btnCapNhat = new Button
            {
                Text = "Cập nhật kết quả",
                Top = 540,
                Left = 170,
                Width = 170,
                Height = 38
            };

            Controls.Add(btnCapNhat);
            Controls.Add(btnLamMoi);
            Controls.Add(txtKetQua);
            Controls.Add(lblKetQua);
            Controls.Add(dgv);
            Controls.Add(title);

            btnLamMoi.Click += (s, e) => LoadData();
            btnCapNhat.Click += (s, e) => CapNhatKetQua();

            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow != null && dgv.CurrentRow.Cells["KETQUA"] != null)
                {
                    txtKetQua.Text = Convert.ToString(dgv.CurrentRow.Cells["KETQUA"].Value);
                }
            };
        }

        private void LoadData()
        {
            try
            {
                string sql = @"
                    SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA
                    FROM VIEW_CONGVIEC_KTV
                    ORDER BY NGAYDV DESC";

                DataTable dt = OracleDb.ExecuteQuery(sql);
                dgv.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải công việc KTV:\n" + ex.Message);
            }
        }

        private void CapNhatKetQua()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng dịch vụ.");
                return;
            }

            try
            {
                string sql = @"
                    UPDATE VIEW_CONGVIEC_KTV
                    SET KETQUA = :KETQUA
                    WHERE MAHSBA = :MAHSBA
                      AND LOAIDV = :LOAIDV
                      AND NGAYDV = :NGAYDV";

                OracleParameter[] p =
                {
                    new OracleParameter("KETQUA", txtKetQua.Text),
                    new OracleParameter("MAHSBA", dgv.CurrentRow.Cells["MAHSBA"].Value),
                    new OracleParameter("LOAIDV", dgv.CurrentRow.Cells["LOAIDV"].Value),
                    new OracleParameter("NGAYDV", dgv.CurrentRow.Cells["NGAYDV"].Value)
                };

                int rows = OracleDb.ExecuteNonQuery(sql, p);

                MessageBox.Show(rows > 0 ? "Cập nhật kết quả thành công." : "Không có dòng nào được cập nhật.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật kết quả:\n" + ex.Message);
            }
        }
    }
}