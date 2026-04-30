using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Windows.Forms;

namespace OracleDBAdmin.Data
{
    public static class OracleDb
    {
		public static string ConnectionString { get; private set; } = "";

		public static OracleConnection GetConnection()
		{
			if (string.IsNullOrEmpty(ConnectionString))
				throw new Exception("Chưa đăng nhập hệ thống!");
			return new OracleConnection(ConnectionString);
		}

		// Thêm tham số 'database' vào hàm
		public static bool TryConnect(string hostPort, string database, string username, string password, bool isSysdba)
		{
			string role = isSysdba ? "DBA Privilege=SYSDBA;" : "";

			// Thử 2 cách gọi phổ biến nhất của Oracle (Service Name và SID)
			string[] connectionsToTry = {
		$"{hostPort}/{database}", // Cú pháp chuẩn mới (Service Name)
        $"{hostPort}:{database}"  // Cú pháp chuẩn cũ (SID)
    };

			foreach (string db in connectionsToTry)
			{
				string tempConn = $"User Id={username};Password={password};{role}Data Source={db};";
				try
				{
					using (var conn = new OracleConnection(tempConn))
					{
						conn.Open();
						ConnectionString = tempConn;
						return true;
					}
				}
				catch (OracleException ex)
				{
					if (ex.Number == 1017) // Sai User/Pass
					{
						MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi Đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
					else if (ex.Number == 12514 || ex.Number == 12505 || ex.Number == 12154) // Không tìm thấy DB
					{
						continue;
					}
				}
				catch (Exception)
				{
					continue;
				}
			}

			MessageBox.Show($"Không tìm thấy CSDL '{database}' tại máy chủ '{hostPort}'.", "Lỗi Kết Nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		public static DataTable ExecuteQuery(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            using (var da = new OracleDataAdapter(cmd))
            {
                cmd.BindByName = true;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                var dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                return dt;
            }
        }

        public static int ExecuteNonQuery(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.BindByName = true;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();

                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SQL lỗi:\n" + sql + "\n\nChi tiết: " + ex.Message);
                }
            }
        }

        public static object ExecuteScalar(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.BindByName = true;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static string EscapeIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Tên đối tượng không hợp lệ.");

            return value.Trim().ToUpper();
        }

        public static string EscapeLiteral(string value)
        {
            if (value == null) return "";
            return value.Replace("\"", "\"\"");
        }
    }
}