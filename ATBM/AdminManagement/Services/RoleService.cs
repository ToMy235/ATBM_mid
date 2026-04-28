using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System.Data;

namespace OracleDBAdmin.Services
{
    public class RoleService
    {
		public void CreateRole(string roleName, string auth, string password)
		{
			roleName = OracleDb.EscapeIdentifier(roleName);
			OracleParameter[] pms = new OracleParameter[] {
				new OracleParameter("p_rolename", roleName)
			};
			OracleDb.ExecuteNonQuery("BEGIN sp_ManageRole_Create(:p_rolename); END;", pms);

			if (auth == "PASSWORD" && !string.IsNullOrWhiteSpace(password))
			{
				OracleDb.ExecuteNonQuery($"ALTER ROLE {roleName} IDENTIFIED BY \"{OracleDb.EscapeLiteral(password)}\"");
			}
		}

		public void DropRole(string roleName)
		{
			OracleParameter[] pms = new OracleParameter[] {
				new OracleParameter("p_rolename", OracleDb.EscapeIdentifier(roleName))
			};
			OracleDb.ExecuteNonQuery("BEGIN sp_ManageRole_Drop(:p_rolename); END;", pms);
		}
		public DataTable GetRoles()
		{
			OracleParameter p_cursor = new OracleParameter("p_cursor", OracleDbType.RefCursor);
			p_cursor.Direction = ParameterDirection.Output;
			return OracleDb.ExecuteQuery("BEGIN sp_Roles_Show(:p_cursor); END;", p_cursor);
		}

        public DataTable SearchRoles(string keyword)
        {
            string sql = @"
                SELECT ROLE,
                       PASSWORD_REQUIRED,
                       COMMON
                FROM DBA_ROLES
                WHERE UPPER(ROLE) LIKE :kw
                ORDER BY ROLE";

            return OracleDb.ExecuteQuery(sql,
                new OracleParameter("kw", "%" + keyword.Trim().ToUpper() + "%"));
        }

        public void AlterRole(string roleName, string authenticationType, string password)
        {
            roleName = OracleDb.EscapeIdentifier(roleName);
            authenticationType = authenticationType.Trim().ToUpper();

            string sql;
            if (authenticationType == "PASSWORD")
            {
                password = OracleDb.EscapeLiteral(password);
                sql = $@"ALTER ROLE {roleName} IDENTIFIED BY ""{password}""";
            }
            else
            {
                sql = $@"ALTER ROLE {roleName} NOT IDENTIFIED";
            }

            OracleDb.ExecuteNonQuery(sql);
        }

        public DataRow GetRoleByName(string roleName)
        {
            string sql = @"
                SELECT ROLE,
                       PASSWORD_REQUIRED,
                       COMMON
                FROM DBA_ROLES
                WHERE ROLE = :roleName";

            var dt = OracleDb.ExecuteQuery(sql,
                new OracleParameter("roleName", roleName.Trim().ToUpper()));

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataTable GetAllRolesForCombo()
        {
            string sql = "SELECT ROLE FROM DBA_ROLES ORDER BY ROLE";
            return OracleDb.ExecuteQuery(sql);
        }
    }
}