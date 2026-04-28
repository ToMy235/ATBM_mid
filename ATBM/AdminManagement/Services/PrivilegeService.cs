using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Data;
using System;
using System.Data;
using System.Linq;

namespace OracleDBAdmin.Services
{
    public class PrivilegeService
    {
		public void GrantObjectPrivilege(string priv, string owner, string obj, string grantee, string[] cols, bool withGrant)
		{
			string finalPriv = priv;
			if (cols != null && cols.Length > 0 && (priv == "UPDATE" || priv == "SELECT"))
			{
				finalPriv += "(" + string.Join(",", cols) + ")";
			}

			OracleParameter[] pms = new OracleParameter[] {
				new OracleParameter("p_grantee", OracleDb.EscapeIdentifier(grantee)),
				new OracleParameter("p_privilege", finalPriv),
				new OracleParameter("p_object_name", $"{OracleDb.EscapeIdentifier(owner)}.{OracleDb.EscapeIdentifier(obj)}"),
				new OracleParameter("p_with_grant", withGrant ? 1 : 0)
			};
			OracleDb.ExecuteNonQuery("BEGIN sp_Grant_ObjectPriv(:p_grantee, :p_privilege, :p_object_name, :p_with_grant); END;", pms);
		}

		public void GrantRole(string role, string grantee, bool withAdmin)
		{
			OracleParameter[] pms = new OracleParameter[] {
				new OracleParameter("p_rolename", OracleDb.EscapeIdentifier(role)),
				new OracleParameter("p_username", OracleDb.EscapeIdentifier(grantee)),
				new OracleParameter("p_with_admin", withAdmin ? 1 : 0)
			};
			OracleDb.ExecuteNonQuery("BEGIN sp_GrantRoleToUser(:p_rolename, :p_username, :p_with_admin); END;", pms);
		}

		public void RevokeObjectPrivilege(string priv, string owner, string obj, string grantee)
		{
			OracleParameter[] pms = new OracleParameter[] {
				new OracleParameter("p_grantee", OracleDb.EscapeIdentifier(grantee)),
				new OracleParameter("p_privilege", priv),
				new OracleParameter("p_object_name", $"{OracleDb.EscapeIdentifier(owner)}.{OracleDb.EscapeIdentifier(obj)}")
			};
			OracleDb.ExecuteNonQuery("BEGIN sp_Revoke_ObjectPriv(:p_grantee, :p_privilege, :p_object_name); END;", pms);
		}

		public void RevokeRole(string role, string grantee)
		{
			OracleParameter[] pms = new OracleParameter[] {
				new OracleParameter("p_grantee", OracleDb.EscapeIdentifier(grantee)),
				new OracleParameter("p_priv_or_role", OracleDb.EscapeIdentifier(role))
			};
			OracleDb.ExecuteNonQuery("BEGIN sp_Revoke_Role(:p_grantee, :p_priv_or_role); END;", pms);
		}

		public DataTable GetObjectPrivileges(string grantee)
		{
			OracleParameter pmsGrantee = new OracleParameter("p_grantee", OracleDb.EscapeIdentifier(grantee));
			OracleParameter p_cursor = new OracleParameter("p_cursor", OracleDbType.RefCursor);
			p_cursor.Direction = ParameterDirection.Output;

			return OracleDb.ExecuteQuery("BEGIN sp_Get_ObjectPrivileges(:p_grantee, :p_cursor); END;", pmsGrantee, p_cursor);
		}
		public DataTable GetSchemas()
        {
            string sql = "SELECT USERNAME FROM DBA_USERS ORDER BY USERNAME";
            return OracleDb.ExecuteQuery(sql);
        }

        public DataTable GetObjectsByType(string owner, string objectType)
        {
            string sql = @"
                SELECT OBJECT_NAME
                FROM DBA_OBJECTS
                WHERE OWNER = :owner
                  AND OBJECT_TYPE = :type
                ORDER BY OBJECT_NAME";

            return OracleDb.ExecuteQuery(sql,
                new OracleParameter("owner", owner.Trim().ToUpper()),
                new OracleParameter("type", objectType.Trim().ToUpper()));
        }

        public DataTable GetColumns(string owner, string objectName)
        {
            string sql = @"
                SELECT COLUMN_NAME
                FROM DBA_TAB_COLUMNS
                WHERE OWNER = :owner
                  AND TABLE_NAME = :obj
                ORDER BY COLUMN_ID";

            return OracleDb.ExecuteQuery(sql,
                new OracleParameter("owner", owner.Trim().ToUpper()),
                new OracleParameter("obj", objectName.Trim().ToUpper()));
        }

        public void GrantSystemPrivilege(string privilege, string grantee, bool withAdminOption)
        {
            privilege = privilege.Trim().ToUpper();
            grantee = OracleDb.EscapeIdentifier(grantee);

            string sql = $"GRANT {privilege} TO {grantee}";
            if (withAdminOption)
                sql += " WITH ADMIN OPTION";

            OracleDb.ExecuteNonQuery(sql);
        }


        public void RevokeSystemPrivilege(string privilege, string grantee)
        {
            privilege = privilege.Trim().ToUpper();
            grantee = OracleDb.EscapeIdentifier(grantee);
            string sql = $"REVOKE {privilege} FROM {grantee}";
            OracleDb.ExecuteNonQuery(sql);
        }

        public DataTable GetSystemPrivileges(string grantee)
        {
            string sql = @"
                SELECT PRIVILEGE,
                       ADMIN_OPTION
                FROM DBA_SYS_PRIVS
                WHERE GRANTEE = :grantee
                ORDER BY PRIVILEGE";

            return OracleDb.ExecuteQuery(sql,
                new OracleParameter("grantee", grantee.Trim().ToUpper()));
        }

        public DataTable GetRolePrivileges(string grantee)
        {
            string sql = @"
                SELECT GRANTED_ROLE,
                       ADMIN_OPTION,
                       DEFAULT_ROLE
                FROM DBA_ROLE_PRIVS
                WHERE GRANTEE = :grantee
                ORDER BY GRANTED_ROLE";

            return OracleDb.ExecuteQuery(sql,
                new OracleParameter("grantee", grantee.Trim().ToUpper()));
        }

        public DataTable GetPrivilegeListForRevoke(string grantee)
        {
            string sql = @"
        SELECT OWNER,
               TABLE_NAME AS OBJECT_NAME,
               'COLUMN' AS PRIV_TYPE,
               PRIVILEGE,
               COLUMN_NAME,
               GRANTABLE,
               'OBJECT' AS CATEGORY
        FROM DBA_COL_PRIVS
        WHERE GRANTEE = :grantee

        UNION ALL

        SELECT OWNER,
               TABLE_NAME AS OBJECT_NAME,
               'TABLE/VIEW' AS PRIV_TYPE,
               PRIVILEGE,
               NULL AS COLUMN_NAME,
               GRANTABLE,
               'OBJECT' AS CATEGORY
        FROM DBA_TAB_PRIVS
        WHERE GRANTEE = :grantee

        UNION ALL

        SELECT NULL AS OWNER,
               NULL AS OBJECT_NAME,
               'SYSTEM' AS PRIV_TYPE,
               PRIVILEGE,
               NULL AS COLUMN_NAME,
               ADMIN_OPTION AS GRANTABLE,
               'SYSTEM' AS CATEGORY
        FROM DBA_SYS_PRIVS
        WHERE GRANTEE = :grantee

        UNION ALL

        SELECT NULL AS OWNER,
               NULL AS OBJECT_NAME,
               'ROLE' AS PRIV_TYPE,
               GRANTED_ROLE AS PRIVILEGE,
               NULL AS COLUMN_NAME,
               ADMIN_OPTION AS GRANTABLE,
               'ROLE' AS CATEGORY
        FROM DBA_ROLE_PRIVS
        WHERE GRANTEE = :grantee

        ORDER BY CATEGORY, PRIV_TYPE, PRIVILEGE";

            return OracleDb.ExecuteQuery(sql,
                new Oracle.ManagedDataAccess.Client.OracleParameter("grantee", grantee.Trim().ToUpper()));
        }

        public string[] GetCommonSystemPrivileges()
        {
            return new[]
            {
                "CREATE SESSION",
                "CREATE TABLE",
                "CREATE VIEW",
                "CREATE PROCEDURE",
                "CREATE SEQUENCE",
                "CREATE SYNONYM",
                "UNLIMITED TABLESPACE"
            };
        }
    }
}