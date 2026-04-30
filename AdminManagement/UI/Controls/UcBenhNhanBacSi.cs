using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI.Controls
{
    public class UcBenhNhanBacSi : UserControl
    {
        private DataGridView dgvBenhNhan;

        private TextBox txtMaBN;
        private TextBox txtTenBN;
        private TextBox txtTienSuBenh;
        private TextBox txtTienSuBenhGD;
        private TextBox txtDiUngThuoc;

        private Button btnLamMoi;
        private Button btnCapNhat;

        public UcBenhNhanBacSi()
        {
            InitializeUi();
            LoadData();
        }

        private void InitializeUi()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            Padding = new Padding(15);

            Label lblTitle = new Label
            {
                Text = "BỆNH NHÂN LIÊN QUAN ĐẾN HỒ SƠ BỆNH ÁN PHỤ TRÁCH",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40)
            };

            dgvBenhNhan = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White
            };

            dgvBenhNhan.CellClick += DgvBenhNhan_CellClick;

            Panel pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 420,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            Label lblFormTitle = new Label
            {
                Text = "Cập nhật thông tin y tế",
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            txtMaBN = CreateTextBox(true);
            txtTenBN = CreateTextBox(true);
            txtTienSuBenh = CreateTextBox(false);
            txtTienSuBenhGD = CreateTextBox(false);
            txtDiUngThuoc = CreateTextBox(false);

            btnLamMoi = new Button
            {
                Text = "Làm mới",
                Width = 120,
                Height = 38,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnCapNhat = new Button
            {
                Text = "Cập nhật",
                Width = 120,
                Height = 38,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnLamMoi.Click += (s, e) => LoadData();
            btnCapNhat.Click += BtnCapNhat_Click;

            FlowLayoutPanel pnlButton = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight
            };

            pnlButton.Controls.Add(btnCapNhat);
            pnlButton.Controls.Add(btnLamMoi);

            pnlRight.Controls.Add(pnlButton);
            pnlRight.Controls.Add(CreateField("Dị ứng thuốc", txtDiUngThuoc, 75));
            pnlRight.Controls.Add(CreateField("Tiền sử bệnh gia đình", txtTienSuBenhGD, 75));
            pnlRight.Controls.Add(CreateField("Tiền sử bệnh", txtTienSuBenh, 75));
            pnlRight.Controls.Add(CreateField("Tên bệnh nhân", txtTenBN, 55));
            pnlRight.Controls.Add(CreateField("Mã bệnh nhân", txtMaBN, 55));
            pnlRight.Controls.Add(lblFormTitle);

            Controls.Add(dgvBenhNhan);
            Controls.Add(pnlRight);
            Controls.Add(lblTitle);
        }

        private TextBox CreateTextBox(bool readOnly)
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = readOnly,
                Font = new Font("Segoe UI", 10),
                Multiline = !readOnly,
                ScrollBars = readOnly ? ScrollBars.None : ScrollBars.Vertical
            };
        }

        private Panel CreateField(string label, TextBox textBox, int height)
        {
            Panel pnl = new Panel
            {
                Dock = DockStyle.Top,
                Height = height,
                Padding = new Padding(0, 5, 0, 5)
            };

            Label lbl = new Label
            {
                Text = label,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            pnl.Controls.Add(textBox);
            pnl.Controls.Add(lbl);

            return pnl;
        }

        private void LoadData()
        {
            try
            {
                string sql = @"
                    SELECT DISTINCT
                           MABN,
                           TENBN,
                           PHAI,
                           NGAYSINH,
                           CCCD,
                           SONHA,
                           TENDUONG,
                           QUANHUYEN,
                           TINHTP,
                           TIENSUBENH,
                           TIENSUBENHGD,
                           DIUNGTHUOC
                    FROM BENHNHAN
                    ORDER BY MABN";

                DataTable dt = OracleDb.ExecuteQuery(sql);
                dgvBenhNhan.DataSource = dt;

                ClearInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải danh sách bệnh nhân: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void DgvBenhNhan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvBenhNhan.Rows[e.RowIndex];

            txtMaBN.Text = Convert.ToString(row.Cells["MABN"].Value);
            txtTenBN.Text = Convert.ToString(row.Cells["TENBN"].Value);
            txtTienSuBenh.Text = Convert.ToString(row.Cells["TIENSUBENH"].Value);
            txtTienSuBenhGD.Text = Convert.ToString(row.Cells["TIENSUBENHGD"].Value);
            txtDiUngThuoc.Text = Convert.ToString(row.Cells["DIUNGTHUOC"].Value);
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaBN.Text))
            {
                MessageBox.Show("Vui lòng chọn một bệnh nhân cần cập nhật.");
                return;
            }

            try
            {
                string sql = @"
                    UPDATE BENHNHAN
                    SET TIENSUBENH = :TIENSUBENH,
                        TIENSUBENHGD = :TIENSUBENHGD,
                        DIUNGTHUOC = :DIUNGTHUOC
                    WHERE MABN = :MABN";

                int rows = OracleDb.ExecuteNonQuery(
                    sql,
                    new OracleParameter("TIENSUBENH", txtTienSuBenh.Text),
                    new OracleParameter("TIENSUBENHGD", txtTienSuBenhGD.Text),
                    new OracleParameter("DIUNGTHUOC", txtDiUngThuoc.Text),
                    new OracleParameter("MABN", txtMaBN.Text)
                );

                if (rows > 0)
                {
                    MessageBox.Show(
                        "Cập nhật thông tin bệnh nhân thành công.",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadData();
                }
                else
                {
                    MessageBox.Show(
                        "Không có dòng nào được cập nhật. Có thể bệnh nhân này không thuộc quyền truy cập của bác sĩ hiện tại.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi cập nhật bệnh nhân: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ClearInput()
        {
            txtMaBN.Clear();
            txtTenBN.Clear();
            txtTienSuBenh.Clear();
            txtTienSuBenhGD.Clear();
            txtDiUngThuoc.Clear();
        }
    }
}