drop table Product;;

GO


create table Products (
    Name varchar(500) not null,
    Category varchar(500) not null,
    Price float not null
);;

GO

insert into  Products (Name, Category, Price) values ('IPad', 'Mobile', 2500);
insert into  Products (Name, Category, Price) values ('Samsung Tab', 'Mobile', 1500);
insert into  Products (Name, Category, Price) values ('Led TV', 'TV', 500);