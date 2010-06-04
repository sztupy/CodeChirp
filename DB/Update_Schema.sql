
    create table Channels (
        Id INTEGER not null,
       name TEXT,
       ownerFk INTEGER,
       primary key (Id)
    );

    create table Channels_Souls (
        ChannelFk INTEGER not null,
       SoulFk INTEGER not null,
       primary key (ChannelFk, SoulFk)
    );

    create index channel_name_index on Channels (name);
