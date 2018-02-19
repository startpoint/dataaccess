# StartPoint corporate Data Access -- English Presentation

Data access compoennt

### What is the StartPoint Corporate Dac?  

Is an entreprise grade data access component. 

It is easily, setupable, reliable, and with a very hight performance level.
It supports a very large panel of data source connection.
You can use File Json, entity framework, or NoSql database in background.  
It is based on Hexagonal architecture so it can be very quickly adapted by adding three level of abstractions.

You can use it with web, mobile, api or Azure and Amazon Functions 

### It is very easily setting

You can setting it by the configuration application tree or by code

You can set

- The provider to specify with database use in background
- The connection string 
- The number of retrying before that an acession is fallbacked
- The interval between each retrying
- The action to fallback a connection or an action

### It is very customisable

Like a Russian doll, you can define two level of abstractions

Firstly, you can specify the repository type like entity framework or nosql or files
Then, you can specify the repository source like sql server or sql lite for entity framework or documentdb, ravendb, mongodb for nosql

Morever, you can add your own implementation

By behavior

You can specify directly from your client, the actions to be used on fallbacking the connection failed or the action execution failed. You can manage the retrying parameters

By adaptation  

If the adaptors provided with the system is not enought, you can add quickly and easily your own adaptors

### Reliable

Is not a simple data access component with a very large support of database type. But StartPoint Corporate DAC integrates in it DNA, allow the mechanism to provide you a very hight level of abstractions and reliability. StartPoint Corporate DAC gives a newest approach to manage your database access.

### Installation  

The installation is based on a first package Ecommerce.Data.RepositoryStore which is the main core component

Then depends on your request, you can install the adaptors packages

- ECommerce.Data.EntityFramework to abstract the entity framework using to access sql server, mysql, firebird, or sqllite
- ECommerce.Data.NoSql to abstract the nosql databases like documentdb, mongodb, ravendb, redis
- Ecommerce.Data.Http to abstract the http sources like couchdb
- ECommerce.Data.FileStore to astract the json file or csv file

for EntityFramework or NoSql, you have to install the Providers packages to enable using it

### Dependencies 

The system depends on

- Castle.Core/4.2.1
- DnsClient/1.0.7
- EntityFrameworkCore.FirebirdSql/2.0.11.6
- Microsoft.Azure.DocumentDB.Core/1.8.1
- Microsoft.Data.Sqlite.Core/2.0.0
- Microsoft.EntityFrameworkCore/2.0.1
- Microsoft.EntityFrameworkCore.InMemory/2.0.1

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D
