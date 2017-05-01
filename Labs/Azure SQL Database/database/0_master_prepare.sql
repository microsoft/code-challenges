
/* 
    Create a demo user login.
    This login will be given access to each database with specific permissions:
        1. Head - READ
        2. Products - READ
        3. Order shards - READ, WRITE
*/

create login [demouser] with password = 'P@ssw0rd1';