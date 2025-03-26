Sqlserver :  

Launch docker instance : docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=tJWBx3ccNJ6dyp1wxoA99qqQ" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:latest

Configuration :

"StorageConfiguration": {
  "Type": "SQLSERVER",
  "ConnectionString": "Server=localhost,1433;Database=IdServer;User Id=sa;Password=tJWBx3ccNJ6dyp1wxoA99qqQ;TrustServerCertificate=True;"
}

Postgre :

Launch docker instance : docker run -e "POSTGRES_USER=admin" -e "POSTGRES_PASSWORD=tJWBx3ccNJ6dyp1wxoA99qqQ" -e "POSTGRES_DB=mydatabase" -p 5432:5432 --name postgres -d postgres:latest

Configuration :

"StorageConfiguration": {
  "Type": "POSTGRE",
  "ConnectionString": "Host=localhost;Port=5432;Database=mydatabase;Username=admin;Password=tJWBx3ccNJ6dyp1wxoA99qqQ"
}


Mysql :

Launch docker instance : docker run -e "MYSQL_ROOT_PASSWORD=tJWBx3ccNJ6dyp1wxoA99qqQ" -e "MYSQL_DATABASE=mydatabase" -e "MYSQL_USER=admin" -e "MYSQL_PASSWORD=tJWBx3ccNJ6dyp1wxoA99qqQ" -p 3306:3306 --name mysql -d mysql:latest

Configuration : 

"StorageConfiguration": {
  "Type": "MYSQL",
  "ConnectionString": "server=localhost;port=3306;database=mydatabase;user=admin;password=tJWBx3ccNJ6dyp1wxoA99qqQ"
}

Sqlite :

"StorageConfiguration": {
  "Type": "SQLITE",
  "ConnectionString": "Data Source=Sid.db"
}

Connect to docker instance (SQLSERVER): 

/opt/mssql-tools18/bin
./sqlcmd -S localhost -U sa -P tJWBx3ccNJ6dyp1wxoA99qqQ -C

Install redis :

docker run -d --name redis-server -p 6379:6379 redis
docker exec -it redis-server redis-cli