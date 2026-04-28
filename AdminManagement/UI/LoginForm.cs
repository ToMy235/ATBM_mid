using OracleDBAdmin.Data;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OracleDBAdmin.UI
{
	public class LoginForm : Form
	{
		private TextBox txtHost;
		private TextBox txtDatabase;
		private TextBox txtUsername;
		private TextBox txtPassword;
		private CheckBox chkSysdba;
		private Button btnLogin;

		public LoginForm()
		{
			InitializeUi();
		}

		private void InitializeUi()
		{
			Text = "Đăng nhập Hệ thống Oracle";
			Size = new Size(420, 560);
			StartPosition = FormStartPosition.CenterScreen;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			BackColor = Color.FromArgb(244, 242, 236);

			Panel card = new Panel
			{
				Size = new Size(340, 450),
				Location = new Point(32, 35),
				BackColor = Color.White,
				BorderStyle = BorderStyle.FixedSingle
			};

			Label lblTitle = new Label
			{
				Text = "ORACLE ADMIN",
				Font = new Font("Segoe UI", 16, FontStyle.Bold),
				ForeColor = Color.FromArgb(37, 99, 235),
				TextAlign = ContentAlignment.MiddleCenter,
				Size = new Size(300, 40),
				Location = new Point(20, 20)
			};

			AddLabel(card, "Máy chủ (Host:Port)", 75);
			txtHost = AddTextBox(card, "localhost:1521", 100);

			AddLabel(card, "Tên CSDL (SID/Service)", 145);
            txtDatabase = AddTextBox(card, "PDBBV", 170);

            AddLabel(card, "Tài khoản", 215);
            txtUsername = AddTextBox(card, "BN00000001", 240);

            AddLabel(card, "Mật khẩu", 285);
			txtPassword = AddTextBox(card, "", 310, true);

			chkSysdba = new CheckBox
			{
				Text = "Đăng nhập với quyền SYSDBA",
				Location = new Point(20, 350),
				AutoSize = true,
				Checked = false
			};

			btnLogin = new Button
			{
				Text = "ĐĂNG NHẬP",
				Location = new Point(20, 385),
				Size = new Size(300, 45),
				BackColor = Color.FromArgb(37, 99, 235),
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 10, FontStyle.Bold),
				FlatStyle = FlatStyle.Flat
			};
			btnLogin.Click += BtnLogin_Click;

			card.Controls.Add(lblTitle);
			card.Controls.Add(chkSysdba);
			card.Controls.Add(btnLogin);
			Controls.Add(card);
			AcceptButton = btnLogin;
		}

		private void AddLabel(Panel p, string text, int y)
		{
			p.Controls.Add(new Label { Text = text, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.Gray });
		}

		private TextBox AddTextBox(Panel p, string defaultText, int y, bool isPass = false)
		{
			TextBox t = new TextBox { Text = defaultText, Location = new Point(20, y), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), UseSystemPasswordChar = isPass };
			p.Controls.Add(t);
			return t;
		}

		private void BtnLogin_Click(object sender, EventArgs e)
		{
            // 1. Đổi con trỏ chuột sang trạng thái chờ
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // 2. Thử kết nối với Oracle
                bool isConnected = OracleDb.TryConnect(
                    txtHost.Text.Trim(),
                    txtDatabase.Text.Trim(),
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    chkSysdba.Checked
                );

                if (isConnected)
                {
                    // THÀNH CÔNG: 
                    // Đóng LoginForm và trả về OK để Program.cs mở MainForm độc lập
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // THẤT BẠI: Thông báo cho người dùng biết
                    MessageBox.Show("Tài khoản hoặc mật khẩu Oracle không chính xác!",
                                    "Đăng nhập thất bại",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Bắt các lỗi hệ thống (Network, Oracle Client chưa cài,...)
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Luôn trả lại con trỏ chuột bình thường dù thành công hay thất bại
                Cursor.Current = Cursors.Default;
            }
        }
	}
}