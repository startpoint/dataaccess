# StartPoint corporate Data Access -- French Presentation

Composant  d’accès aux données 

### Introduction qu’est-ce que le composant Startpoint Dac?  

Composant d’accès et de mise en cache des données de l’entreprise. 

Il est, facilement personnalisable, configurable, fiable, très performant et supporte de très nombreuses sources comme les fichiers json, csv et xml, les bases de relationnelles de type sql-server, SqlLite que des bases non sql comme documentdb, mongodb rave en ou encore redis,  

Il supporte aussi bien des clients Web, mobile, que des api Web et des fonctions Azure et Amazon.,  

### Configurable 

La  configuration se fait depuis l’arbre de configuration applicatif ou directement depuis un poco passé en paramétre. 

Il est possible de configurer 

- Le provider pour définir le fournisseur à utiliser
- La chaîne de connexion 
- Le nombre d'essaye avant qu'une opération soit considérée en échec
- L'interval entre chaque essaye
- Les actions à exécuter en cas d'échec de la connexion ou de l'action

### Personnalisable  

Le système est personnalisable à deux niveaux  

Par comportement  

Par code, vous pouvez fournir les méthodes de fallback et de fallback du disjoncteur et les règles de retrying

Par adaptation  

Vous pouvez définir vos propres adapteurs et ainsi utiliser le mécanisme de résolution automatique mis en œuvre  

### Fiable 

Plus qu’un simple composant d’accès aux données basique comme retrouver dans la plupart des composants nuget ou qu’un simple service d’abstraction pour entity framework core et quelques dizaines de sources de données, le système intègre un mécanisme de resilience et fournit un niveau de fiabilité très avancé.  

### Installation  

L’installation se fait par l’installation d’un package core et de plusieurs packages adapteurs 

### Dépendances 

Le système dépend des packages suivant 

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
