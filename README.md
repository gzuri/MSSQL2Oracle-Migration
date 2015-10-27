# MSSQL2Oracle ![AppVayour status](https://ci.appveyor.com/api/projects/status/60wftquacksfgng6/branch/master?svg=true)


MSSQL2Oracle is a small migration tool to help you migrate to Oracle database. It extracts basic database structure and all of the data from source SQL Server database.  All the data is written to files, one file for table structure and one file per table. 

## Features
 - Translate all camel case table and column names to upper and underscore delimiter
 - Create a file with SQL create statements (FK, indexes not supported for now)
 - Create files with SQL INSERT statements from the SQL Server source database

## Usage
The easiest way is to download compiled zip version from the url, or download the latest version of the source and compile it yourself.

1. If obtained the zip, extract it
2. Navigate to application through command promt (you may need to run it in admin mode)
3. Run the app with params
 1. c - Source SQL Server connection string inside quotes
 2. p - full path to directory where you want the data to be stored