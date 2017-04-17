BUILD Hands-on-labs 2017 (Code challenge lab)
=============================================

Client kick-off meeting notes
-----------------------------

Liam Cavanagh - Azure Search

15 minute lab
Stick to Visual Studio (dev audience)


Azure Search
------------

Same as 2016 lab (which has been copied into code challenges repo)

Extra features / Changes

- change auto complete to term suggestion rather than document suggestion
- demonstrate synomns functionality by creating a dictionary of words we can use to suggest other terms and highlight these in our results.

Document DB
-----------

Same as 2016 lab (which has been copied into code challenges repo)

Extra features / Changes

- Add Latency record - display how long query took.
- Send some warm up queries when the application loads to reduce the latency time recorder
- Add region selector
- Add webjob to use twitter streaming api to gather tweets about build *need to agree on hashtag* and save them to DocumentDb