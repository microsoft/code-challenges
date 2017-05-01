SQL DB Lab Deployment Steps
===========================

About the Files in this directory
---------------------------------

### ShardMapPrep.sln

This solution contains some utility programs:

1.  Extract - Extract the `Orders` and `Order Details` tables from a pre-prepared database and dumps the results
    into two JSON files.
    
2.  Prepare - Builds the demo shard map.

    Requires the Head database and Shard Database Pool to be pre-created.
    
    1.  Connects to the Head database.
    2.  Creates an initializes a shard map.
    3.  Iterates between `FirstShardId` and `LastShardId` and programatically
        creates and prepares each order shard.
        
    The Shard Id is used as a customer Id to filter the Orders and Order Details read from the JSON data files before inserting
    them into each shard.

    The tool can be safely re-run - on subsequent runs existing shards will be cleaned and re-populated.


### JSON Files

These data files contain all demo orders in JSON format. They are used by the ShardPrepMap Prepare tool to prepare each customer shard.

1.  `orders.json`
2.  `order details.json`

You can regenerate these files from a pre-prepared database (e.g. built using `4_insert_customer_orders.sql`) using the ShardMapPrep Extract tool.


### SQL Files

1.  `0_master_prepare.sql` - Prepare the master database (creates demo user login)
2.  `1_head_prepare.sql` - Prepare the Head database
3.  `2_products_prepare.sql` - Prepare the Product database
4.  `3_order_shard_prepare.sql` - Prepare a single order shard. Used by the ShardPrepMap Prepare tool
5.  `4_insert_customer_orders.sql` - Inserts all demo orders into a single order shard. Contains the data from `orders.json` and `order details.json` in SQL format.  


Create databases
----------------

1.  In the Azure portal create:

    -  SQL Database Server v12
    -  SQL Database named "Head", size S1
    -  SQL Database named "Products", size S0
    -  SQL Database Pool named "ShardDatabasePool", size B0

2.  Configure the ShardPrepMap Prepare tool with the SQL Database Server name, admin username and password.

    - Be sure to set the correct number of shards to create

3.  Run the Prepare tool.

4.  Configure the 1_head_prepare.sql script with the new SQL Database Server name, admin username and password. 

5.  Connect to the "Head" database and run the 1_head_prepare.sql script.

6.  Connect to the "Products" database and run the 2_products_prepare.sql script.