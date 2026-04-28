using System;
using System.Windows.Forms;
using OracleDBAdmin.UI;

namespace OracleDBAdmin
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // 1. Khởi tạo LoginForm
            using (LoginForm login = new LoginForm())
            {
                // Mở Login dưới dạng Dialog (chặn code ở Main cho đến khi Login đóng)
                DialogResult result = login.ShowDialog();

                // 2. Kiểm tra kết quả đăng nhập
                if (result == DialogResult.OK)
                {
                    // Nếu đăng nhập thành công, mới bắt đầu chạy MainForm
                    // Lúc này MainForm trở thành "Main Message Loop" của App
                    Application.Run(new MainForm());
                }
                // Nếu User bấm X hoặc Hủy ở Login, chương trình sẽ kết thúc tự nhiên tại đây
            }
        }
    }
}