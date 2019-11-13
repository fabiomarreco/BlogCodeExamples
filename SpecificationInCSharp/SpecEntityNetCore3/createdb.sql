-- SQLite
create table tbUser (
    userid integer primary key,
    name text not null,
    gender int not null,
    birthday date not null
)

GO

insert into tbUser (name, gender, birthday) values ('joe', 0, '2001-03-04');

