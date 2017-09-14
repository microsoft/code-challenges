## Using Azure Database for PostgreSQL to Run a Python Application
### Overview
Azure Database for PostgreSQL is a PostgreSQL database service built on Microsoft's scalable cloud infrastructure for application developers. 

Leverage your existing open-source PostgreSQL skills and tools and scale on-the-fly without downtime to efficiently deliver existing and new applications with reduced operational overhead. 

Built-in features maximize performance, availability, and security. Azure Database for PostgreSQL empowers developers to focus on application innovation instead of database management tasks.



### Scenario Overview
This hands-on lab will step you through the following:

1.	Create an Azure storage account and initialize Azure Cloud Shell for Azure CLI.
1.  Create an **Azure Database for PostgreSQL** instance.
1.  Create an **Ubuntu Azure VM**.
2.	Enable the firewall.
3.	Create a database for an app written in Python and Django in Azure PostgreSQL database instance.
4.	Login to ubuntu VM. 
5.	Download the app from GitHub: git clone https://github.com/vitorfs/bootcamp.git - cd bootcamp.
7.	Change your connection string - vim .env - Paste this in the .env file (change database credentials to yours)
    	
    <span style="color:blue">DEBUG=True<br>SECRET_KEY='mys3cr3tk3y'<br>DATABASE_URL='postgres://pgsqluser@postgresql<inject story-id="story://Content-Private/content/dfd/SP-OSS/postgresql/ossexperience1/story_a_postgresql" key="resourceGroupName" />:P@ssword1@<inject story-id="story://Content-Private/content/dfd/SP-OSS/postgresql/ossexperience1/story_a_postgresql" key="resourceGroupName" />.database.windows.net:5432/bootcamp'<br>ALLOWED_HOSTS = "*"
8.  Run migration and serve the app
        
          Python manage.py migrate
          Python manage.py runserver 0.0.0.0:8000

9.  Open browser and go to http://[externalIpAddress]:8000 to interact with the app.
10. Play around with the app, and demonstrate that it works.
    
