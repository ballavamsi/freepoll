
Scaffold-DbContext "server=remotemysql.com;port=3306;user=3wAtUqE7dU;password=esHlHotlf9;database=3wAtUqE7dU" MySql.Data.EntityFrameworkCore -Context FreePollDBContext -OutputDir Models -f
-Tables and we can specify tables we want
We use this command everytime there is a change in the db.
Once this command is executed Remove override OnConfiguring method

Remember it doesnot automatically delete classes which are not used we have to delete them.