
    drop table if exists Souls cascade;

    drop table if exists Souls_Badges cascade;

    drop table if exists Channels cascade;

    drop table if exists Channels_Souls cascade;

    drop table if exists Posts cascade;

    drop table if exists Posts_Tags cascade;

    drop table if exists Tags cascade;

    drop table if exists Badges cascade;

    drop table if exists StaticDatas cascade;

    drop table if exists Sessions cascade;

    drop table if exists Roles cascade;

    drop table if exists Roles_Users cascade;

    drop table if exists Users cascade;

    drop table if exists OpenIdAlternatives cascade;

    drop table if exists ProfileDatas cascade;

    drop table if exists Profiles cascade;

    drop sequence if exists Soul_sequence;

    drop sequence if exists Channel_sequence;

    drop sequence if exists Post_sequence;

    drop sequence if exists Tag_sequence;

    drop sequence if exists Badge_sequence;

    drop sequence if exists StaticData_sequence;

    drop sequence if exists Session_sequence;

    drop sequence if exists Role_sequence;

    drop sequence if exists User_sequence;

    drop sequence if exists ProfileData_sequence;

    drop sequence if exists Profile_sequence;

    create table Souls (
        Id int4 not null,
       name varchar(255),
       siteid int8,
       sitename int4,
       gravatar varchar(255),
       point int8,
       primary key (Id),
      unique (siteid, sitename)
    );

    create table Souls_Badges (
        SoulFk int4 not null,
       BadgeFk int4 not null,
       primary key (BadgeFk, SoulFk)
    );

    create table Channels (
        Id int4 not null,
       name varchar(255),
       ownerFk int4,
       primary key (Id)
    );

    create table Channels_Souls (
        ChannelFk int4 not null,
       SoulFk int4 not null,
       primary key (ChannelFk, SoulFk)
    );

    create table Posts (
        Id int4 not null,
       body text,
       summary text,
       lastedit timestamp,
       lastactivity timestamp,
       type int4,
       siteid int8,
       sitename int4,
       community boolean,
       score int8,
       userFk int4,
       parentFk int4,
       primary key (Id),
      unique (type, siteid, sitename)
    );

    create table Posts_Tags (
        PostFk int4 not null,
       TagFk int4 not null,
       primary key (PostFk, TagFk)
    );

    create table Tags (
        Id int4 not null,
       site int4,
       name varchar(255),
       primary key (Id),
      unique (site, name)
    );

    create table Badges (
        Id int4 not null,
       description text,
       rank int4,
       name varchar(255),
       siteid int8,
       sitename int4,
       count int8,
       primary key (Id),
      unique (siteid, sitename)
    );

    create table StaticDatas (
        Id int4 not null,
       lastmodifieddate timestamp,
       primary key (Id)
    );

    create table Sessions (
        Id int4 not null,
       Data text,
       SessionId varchar(255),
       ApplicationName varchar(255),
       Created timestamp,
       Expires timestamp,
       Timeout int4,
       Locked boolean,
       LockId int4,
       LockDate timestamp,
       Flags int4,
       primary key (Id),
      unique (SessionId)
    );

    create table Roles (
        Id int4 not null,
       Name varchar(255),
       ApplicationName varchar(255),
       primary key (Id),
      unique (Name, ApplicationName)
    );

    create table Roles_Users (
        RoleFk int4 not null,
       UserFk int4 not null,
       primary key (UserFk, RoleFk)
    );

    create table Users (
        Id int4 not null,
       Username varchar(255),
       ApplicationName varchar(255),
       Email varchar(255),
       IsLockedOut boolean,
       Comment varchar(255),
       Password varchar(255),
       PasswordQuestion varchar(255),
       PasswordAnswer varchar(255),
       IsApproved boolean,
       LastActivityDate timestamp,
       LastLoginDate timestamp,
       LastPasswordChangedDate timestamp,
       CreationDate timestamp,
       IsOnline boolean,
       LastLockedOutDate timestamp,
       FailedPasswordAttemptCount int4,
       FailedPasswordAttemptWindowStart timestamp,
       FailedPasswordAnswerAttemptCount int4,
       FailedPasswordAnswerAttemptWindowStart timestamp,
       UserProfileFk int4,
       primary key (Id),
      unique (Username, ApplicationName)
    );

    create table OpenIdAlternatives (
        User_id int4 not null,
       Value varchar(255)
    );

    create table ProfileDatas (
        Id int4 not null,
       ValueString text,
       Name varchar(255),
       ValueBinary bytea,
       ProfileFk int4,
       primary key (Id)
    );

    create table Profiles (
        Id int4 not null,
       ApplicationName varchar(255),
       IsAnonymous boolean,
       LastActivityDate timestamp,
       LastUpdatedDate timestamp,
       UserFk int4,
       primary key (Id)
    );

    create index soul_name_index on Souls (name);

    alter table Souls_Badges 
        add constraint FK1EE91AF7CA62FC0 
        foreign key (BadgeFk) 
        references Badges;

    alter table Souls_Badges 
        add constraint FK1EE91AFFCEB21D4 
        foreign key (SoulFk) 
        references Souls;

    create index channel_name_index on Channels (name);

    alter table Channels 
        add constraint FK593E87F0AC313938 
        foreign key (ownerFk) 
        references Users;

    alter table Channels_Souls 
        add constraint FK9AB70FB1FCEB21D4 
        foreign key (SoulFk) 
        references Souls;

    alter table Channels_Souls 
        add constraint FK9AB70FB19C059600 
        foreign key (ChannelFk) 
        references Channels;

    create index post_lastedit_index on Posts (lastedit);

    create index post_lastactivity_index on Posts (lastactivity);

    alter table Posts 
        add constraint FK49B8BB3F85041FC 
        foreign key (userFk) 
        references Souls;

    alter table Posts 
        add constraint FK49B8BB37071807C 
        foreign key (parentFk) 
        references Posts;

    alter table Posts_Tags 
        add constraint FKCB92D45E629E73E 
        foreign key (TagFk) 
        references Tags;

    alter table Posts_Tags 
        add constraint FKCB92D45F30AEFF6 
        foreign key (PostFk) 
        references Posts;

    create index badge_rank_index on Badges (rank);

    create index badge_name_index on Badges (name);

    alter table Roles_Users 
        add constraint FK903B75C6A3381650 
        foreign key (UserFk) 
        references Users;

    alter table Roles_Users 
        add constraint FK903B75C6A5C6FBCE 
        foreign key (RoleFk) 
        references Roles;

    create index users_email_index on Users (Email);

    create index users_islockedout_index on Users (IsLockedOut);

    alter table Users 
        add constraint FK4E39DE8DFD77F69 
        foreign key (UserProfileFk) 
        references Profiles;

    alter table OpenIdAlternatives 
        add constraint FKBFC2442B9A29C98F 
        foreign key (User_id) 
        references Users;

    alter table ProfileDatas 
        add constraint FK51398200DA755784 
        foreign key (ProfileFk) 
        references Profiles;

    alter table Profiles 
        add constraint FKC81D100AA3381650 
        foreign key (UserFk) 
        references Users;

    create sequence Soul_sequence;

    create sequence Channel_sequence;

    create sequence Post_sequence;

    create sequence Tag_sequence;

    create sequence Badge_sequence;

    create sequence StaticData_sequence;

    create sequence Session_sequence;

    create sequence Role_sequence;

    create sequence User_sequence;

    create sequence ProfileData_sequence;

    create sequence Profile_sequence;
