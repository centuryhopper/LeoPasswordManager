CREATE TABLE PasswordManager_Users (
    ID SERIAL PRIMARY KEY,
    EMAIL VARCHAR(256) NOT NULL,
    FirstName VARCHAR(256) NOT NULL,
    LastName VARCHAR(256) NOT NULL,
    DateLastLogin TIMESTAMP,
    DateLastLogout TIMESTAMP,
    DateCreated TIMESTAMP,
    DateRetired TIMESTAMP
);


-- CREATE TABLE Roles(
--     ID VARCHAR(256) PRIMARY KEY NOT NULL,
--     Name VARCHAR(256) NOT NULL
-- );

-- INSERT INTO ROLES (ID, Name) VALUES ('6cc5f093-510e-43a7-a4f5-7fd24ba4b712', 'Admin');
-- INSERT INTO ROLES (ID, Name) VALUES ('769db090-6341-413d-9168-b3e126fc4e05', 'User');

-- CREATE TABLE UserRoles(
--     ID VARCHAR(256) PRIMARY KEY NOT NULL,
--     USERID VARCHAR(256),
--     RoleId VARCHAR(256),
--     FOREIGN KEY (USERID) REFERENCES passwordmanager_users (ID) ON DELETE CASCADE,
--     FOREIGN KEY (RoleId) REFERENCES Roles (ID) ON DELETE CASCADE
-- );

CREATE TABLE PasswordManager_Accounts (
    ID SERIAL PRIMARY KEY,
    USERID INT NOT NULL,
    TITLE VARCHAR(256) NOT NULL,
    USERNAME VARCHAR(256) NOT NULL,
    PASSWORD VARCHAR(256) NOT NULL,
    CREATED_AT TIMESTAMP,
    LAST_UPDATED_AT TIMESTAMP,
    FOREIGN KEY (USERID) REFERENCES PasswordManager_Users(ID)
);


-- CREATE TABLE UserTokens(
--     Id VARCHAR(256) PRIMARY KEY NOT NULL,
--     LoginProvider VARCHAR(256) NOT NULL,
--     PROVIDERKEY VARCHAR(256) NOT NULL,
--     USERID VARCHAR(256) REFERENCES passwordmanager_users (ID) ON DELETE CASCADE
-- );


drop table "passwordmanager_accounts";
drop table "usertokens";
drop table "userroles";
drop table "roles";
drop table "passwordmanager_users";
