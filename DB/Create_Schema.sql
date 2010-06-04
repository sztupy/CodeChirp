
    drop table if exists Souls;

    drop table if exists Posts;

    drop table if exists Posts_Tags;

    drop table if exists Tags;

    drop table if exists Badges;

    drop table if exists StaticDatas;

    drop table if exists Sessions;

    drop table if exists Roles;

    drop table if exists Roles_Users;

    drop table if exists Users;

    drop table if exists OpenIdAlternatives;

    drop table if exists ProfileDatas;

    drop table if exists Profiles;

    drop table if exists hibernate_unique_key;

    create table Souls (
        Id INTEGER not null,
       sitename INTEGER,
       siteid INTEGER,
       name TEXT,
       gravatar TEXT,
       point INTEGER,
       primary key (Id)
    );

    create table Posts (
        Id INTEGER not null,
       body text,
       sitename INTEGER,
       siteid INTEGER,
       type INTEGER,
       summary TEXT,
       community INTEGER,
       score INTEGER,
       lastedit DATETIME,
       lastactivity DATETIME,
       userFk INTEGER,
       parentFk INTEGER,
       primary key (Id)
    );

    create table Posts_Tags (
        PostFk INTEGER not null,
       TagFk INTEGER not null,
       primary key (PostFk, TagFk)
    );

    create table Tags (
        Id INTEGER not null,
       site INTEGER,
       name TEXT,
       primary key (Id)
    );

    create table Badges (
        Id INTEGER not null,
       description text,
       sitename INTEGER,
       siteid INTEGER,
       name TEXT,
       count INTEGER,
       rank INTEGER,
       primary key (Id)
    );

    create table StaticDatas (
        Id INTEGER not null,
       lastmodifieddate DATETIME,
       primary key (Id)
    );

    create table Sessions (
        Id INTEGER not null,
       Data text,
       SessionId TEXT,
       ApplicationName TEXT,
       Created DATETIME,
       Expires DATETIME,
       Timeout INTEGER,
       Locked INTEGER,
       LockId INTEGER,
       LockDate DATETIME,
       Flags INTEGER,
       primary key (Id),
      unique (SessionId)
    );

    create table Roles (
        Id INTEGER not null,
       Name TEXT,
       ApplicationName TEXT,
       primary key (Id),
      unique (Name, ApplicationName)
    );

    create table Roles_Users (
        RoleFk INTEGER not null,
       UserFk INTEGER not null,
       primary key (UserFk, RoleFk)
    );

    create table Users (
        Id INTEGER not null,
       Username TEXT,
       ApplicationName TEXT,
       Email TEXT,
       IsLockedOut INTEGER,
       Comment TEXT,
       Password TEXT,
       PasswordQuestion TEXT,
       PasswordAnswer TEXT,
       IsApproved INTEGER,
       LastActivityDate DATETIME,
       LastLoginDate DATETIME,
       LastPasswordChangedDate DATETIME,
       CreationDate DATETIME,
       IsOnline INTEGER,
       LastLockedOutDate DATETIME,
       FailedPasswordAttemptCount INTEGER,
       FailedPasswordAttemptWindowStart DATETIME,
       FailedPasswordAnswerAttemptCount INTEGER,
       FailedPasswordAnswerAttemptWindowStart DATETIME,
       UserProfileFk INTEGER,
       primary key (Id),
      unique (Username, ApplicationName)
    );

    create table OpenIdAlternatives (
        User_id INTEGER not null,
       Value TEXT
    );

    create table ProfileDatas (
        Id INTEGER not null,
       ValueString text,
       Name TEXT,
       ValueBinary BLOB,
       ProfileFk INTEGER,
       primary key (Id)
    );

    create table Profiles (
        Id INTEGER not null,
       ApplicationName TEXT,
       IsAnonymous INTEGER,
       LastActivityDate DATETIME,
       LastUpdatedDate DATETIME,
       UserFk INTEGER,
       primary key (Id)
    );

    create index soul_site_name_index on Souls (sitename);

    create index soul_site_id_index on Souls (siteid);

    create index post_site_name_index on Posts (sitename);

    create index post_site_id_index on Posts (siteid);

    create index tag_site_name_index on Tags (site);

    create index badge_site_name_index on Badges (sitename);

    create index badge_site_id_index on Badges (siteid);

    create index users_email_index on Users (Email);

    create index users_islockedout_index on Users (IsLockedOut);

    create table hibernate_unique_key (
         next_hi INTEGER 
    );

    insert into hibernate_unique_key values ( 1 );
