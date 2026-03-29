--=============================================================================
--CAU 1
        -- USERS
-- 1. Tạo mới User
CREATE OR REPLACE PROCEDURE sp_ManageUser_Create(
    p_username IN VARCHAR2,
    p_password IN VARCHAR2
) AS
BEGIN
    EXECUTE IMMEDIATE 'CREATE USER ' || p_username || ' IDENTIFIED BY "' || p_password || '"';
    EXECUTE IMMEDIATE 'GRANT CONNECT, RESOURCE TO ' || p_username;
END;
/

-- 2. Xóa User
CREATE OR REPLACE PROCEDURE sp_ManageUser_Drop(
    p_username IN VARCHAR2
) AS
BEGIN
    EXECUTE IMMEDIATE 'DROP USER ' || p_username || ' CASCADE';
END;
/

-- 3. Đổi mật khẩu User (Sửa)
CREATE OR REPLACE PROCEDURE sp_ManageUser_UpdatePassword(
    p_username IN VARCHAR2,
    p_newpassword IN VARCHAR2
) AS
BEGIN
    EXECUTE IMMEDIATE 'ALTER USER ' || p_username || ' IDENTIFIED BY "' || p_newpassword ||'"';
END;
/

        -- ROLES
-- 1. Tạo mới Role
CREATE OR REPLACE PROCEDURE sp_ManageRole_Create(
    p_rolename IN VARCHAR2
) AS
BEGIN
    EXECUTE IMMEDIATE 'CREATE ROLE ' || p_rolename;
END;
/

-- 2. Xóa Role
CREATE OR REPLACE PROCEDURE sp_ManageRole_Drop(
    p_rolename IN VARCHAR2
) AS
BEGIN
    EXECUTE IMMEDIATE 'DROP ROLE ' || p_rolename;
END;
/
--=============================================================================
-- CAU 2
-- Xem tất cả user và trạng thái khóa/mở của họ
CREATE OR REPLACE PROCEDURE sp_Users_Show
    (p_cursor OUT SYS_REFCURSOR)
AS 
BEGIN
OPEN p_cursor FOR
SELECT 
    username, 
    account_status, 
    created, 
    default_tablespace, 
    last_login 
FROM DBA_USERS 
ORDER BY username;
END;
/
-- Xem danh sách tất cả các Role
CREATE OR REPLACE PROCEDURE sp_Roles_Show
    (p_cursor OUT SYS_REFCURSOR)
AS
BEGIN
OPEN p_cursor FOR
SELECT 
    role, 
    password_required, 
    authentication_type 
FROM DBA_ROLES 
ORDER BY role;
END;
/
-- Xem User và Role tương ứng được gán
CREATE OR REPLACE PROCEDURE sp_UsersAndRoles_Show
    (p_cursor OUT SYS_REFCURSOR)
AS
BEGIN
OPEN p_cursor FOR
SELECT 
    grantee AS "User/Role", 
    granted_role AS "Role Name", 
    admin_option 
FROM DBA_ROLE_PRIVS 
WHERE grantee NOT IN ('SYS', 'SYSTEM') -- Lọc bớt user hệ thống nếu cần
ORDER BY grantee;
END;

--=============================================================================
-- CAU 3
-- a, b. Cấp quyền cho doi tuong (ROLE hoac USER)
CREATE OR REPLACE PROCEDURE sp_Grant_ObjectPriv (
    p_grantee IN VARCHAR2,    -- User nhận quyền
    p_privilege IN VARCHAR2,  -- SELECT, INSERT, UPDATE, DELETE...
    p_object_name IN VARCHAR2,-- Tên Table/View
    p_with_grant IN NUMBER    -- 1: Có GRANT OPTION, 0: Không
) 
AS
    v_sql VARCHAR2(500);
BEGIN
    v_sql := 'GRANT ' || p_privilege || ' ON ' || p_object_name || ' TO ' || p_grantee;
    
    IF p_with_grant = 1 THEN
        v_sql := v_sql || ' WITH GRANT OPTION';
    END IF;
    
    EXECUTE IMMEDIATE v_sql;
END;
/
-- Cap Role cho USER
CREATE OR REPLACE PROCEDURE sp_GrantRoleToUser (
    p_rolename IN VARCHAR2,
    p_username IN VARCHAR2,
    p_with_admin IN NUMBER DEFAULT 0 -- 1: Có ADMIN OPTION, 0: Không
) 
AS
    v_sql VARCHAR2(500);
BEGIN
    -- Câu lệnh cơ bản
    v_sql := 'GRANT ' || p_rolename || ' TO ' || p_username;
    
    -- Nếu tham số truyền vào là 1, nối thêm chuỗi WITH ADMIN OPTION
    IF p_with_admin = 1 THEN
        v_sql := v_sql || ' WITH ADMIN OPTION';
    END IF;
    
    -- Thực thi câu lệnh động
    EXECUTE IMMEDIATE v_sql;
END;
/

--c. can suy nghi them:))
--===========================================================================
--CAU 4
-- Thu hoi Role
CREATE OR REPLACE PROCEDURE sp_Revoke_Role (
    p_grantee IN VARCHAR2,      -- Tên User hoặc Role bị thu hồi
    p_priv_or_role IN VARCHAR2  -- Tên quyền hệ thống hoặc tên Role
) 
AS
    v_sql VARCHAR2(500);
BEGIN
    -- Cú pháp: REVOKE <privilege/role> FROM <user/role>
    v_sql := 'REVOKE ' || p_priv_or_role || ' FROM ' || p_grantee;
    
    EXECUTE IMMEDIATE v_sql;
END;
/

-- Thu hoi Quyen tren object
CREATE OR REPLACE PROCEDURE sp_Revoke_ObjectPriv (
    p_grantee IN VARCHAR2,    -- User bị thu hồi quyền
    p_privilege IN VARCHAR2,  -- SELECT, INSERT, UPDATE...
    p_object_name IN VARCHAR2 -- Tên Table/View
) 
AS
    v_sql VARCHAR2(500);
BEGIN
    -- Cú pháp: REVOKE <privilege> ON <object> FROM <user>
    v_sql := 'REVOKE ' || p_privilege || ' ON ' || p_object_name || ' FROM ' || p_grantee;
    
    EXECUTE IMMEDIATE v_sql;
END;
/
---================================================================
--CAU 5
-- Kiem tra quyen
CREATE OR REPLACE PROCEDURE sp_Get_ObjectPrivileges (
    p_grantee IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
        SELECT 
            owner AS "Chủ sở hữu", 
            table_name AS "Tên đối tượng", 
            privilege AS "Quyền", 
            grantable AS "Có quyền cấp tiếp?"
        FROM DBA_TAB_PRIVS
        WHERE grantee = UPPER(p_grantee)
        ORDER BY table_name;
END;
/