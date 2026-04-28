using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System;
using System.Data;

namespace OracleDBAdmin.Services
{
    public class UserService
    {
		public DataTable GetUsers()
		{
			OracleParameter p_cursor = new OracleParameter("p_cursor", OracleDbType.RefCursor);
			p_cursor.Direction = ParameterDirection.Output;
			return OracleDb.ExecuteQuery("BEGIN sp_Users_Show(:p_cursor); END;", p_cursor);
		}

		public DataTable SearchUsers(string keyword)
        {
            string sql = @"
                SELECT USERNAME,
                       ACCOUNT_STATUS,
                       EXPIRY_DATE,
                       DEFAULT_TABLESPACE,
                       TEMPORARY_TABLESPACE
                FROM DBA_USERS
                WHERE UPPER(USERNAME) LIKE :kw
                ORDER BY USERNAME";

            return OracleDb.ExecuteQuery(sql,
                new OracleParameter("kw", "%" + keyword.Trim().ToUpper() + "%"));
        }

        public DataTable GetProfiles()
        {
            string sql = "SELECT DISTINCT PROFILE FROM DBA_PROFILES ORDER BY PROFILE";
            return OracleDb.ExecuteQuery(sql);
        }

        public DataTable GetTablespaces()
        {
            string sql = "SELECT TABLESPACE_NAME FROM DBA_TABLESPACES ORDER BY TABLESPACE_NAME";
            return OracleDb.ExecuteQuery(sql);
        }

		public void CreateUser(string username, string password, string defaultTs, string tempTs, string profile, bool unlock, DateTime? expiry)
		{
			username = OracleDb.EscapeIdentifier(username);

			OracleParameter[] pms = new OracleParameter[] {
				new OracleParameter("p_username", username),
				new OracleParameter("p_password", password)
			};
			OracleDb.ExecuteNonQuery("BEGIN sp_ManageUser_Create(:p_username, :p_password); END;", pms);

			string alterSql = $@"
                ALTER USER {username} 
                DEFAULT TABLESPACE {OracleDb.EscapeIdentifier(defaultTs)} 
                TEMPORARY TABLESPACE {OracleDb.EscapeIdentifier(tempTs)} 
                PROFILE {OracleDb.EscapeIdentifier(profile)} 
                {(unlock ? "ACCOUNT UNLOCK" : "ACCOUNT LOCK")}";
			OracleDb.ExecuteNonQuery(alterSql);

			if (expiry.HasValue)
				OracleDb.ExecuteNonQuery($"ALTER USER {username} PASSWORD EXPIRE");
		}

		public void AlterUser(string username, string password, string defaultTs, string tempTs, string profile, bool unlock)
		{
			username = OracleDb.EscapeIdentifier(username);

			if (!string.IsNullOrWhiteSpace(password))
			{
				OracleParameter[] pms = new OracleParameter[] {
					new OracleParameter("p_username", username),
					new OracleParameter("p_newpassword", password)
				};
				OracleDb.ExecuteNonQuery("BEGIN sp_ManageUser_UpdatePassword(:p_username, :p_newpassword); END;", pms);
			}

			string alterSql = $@"
                ALTER USER {username} 
                DEFAULT TABLESPACE {OracleDb.EscapeIdentifier(defaultTs)} 
                TEMPORARY TABLESPACE {OracleDb.EscapeIdentifier(tempTs)} 
                PROFILE {OracleDb.EscapeIdentifier(profile)} 
                {(unlock ? "ACCOUNT UNLOCK" : "ACCOUNT LOCK")}";
			OracleDb.ExecuteNonQuery(alterSql);
		}

		public void DropUser(string username)
		{
			OracleParameter[] pms = new OracleParameter[] {
				new OracleParameter("p_username", OracleDb.EscapeIdentifier(username))
			};
			OracleDb.ExecuteNonQuery("BEGIN sp_ManageUser_Drop(:p_username); END;", pms);
		}

		public DataRow GetUserByName(string username)
        {
            string sql = @"
                SELECT USERNAME,
                       ACCOUNT_STATUS,
                       EXPIRY_DATE,
                       DEFAULT_TABLESPACE,
                       TEMPORARY_TABLESPACE,
                       PROFILE
                FROM DBA_USERS
                WHERE USERNAME = :username";

            var dt = OracleDb.ExecuteQuery(sql,
                new OracleParameter("username", username.Trim().ToUpper()));

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataTable GetAllUsersForCombo()
        {
            string sql = "SELECT USERNAME FROM DBA_USERS ORDER BY USERNAME";
            return OracleDb.ExecuteQuery(sql);
        }
    }
}