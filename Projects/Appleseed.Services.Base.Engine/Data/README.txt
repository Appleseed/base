1. as_Cache_BaseItemCollection is required for any aggregators inserting into it, and any collectors collecting from it

2. MemberContacts is a work in progress and so is the initial data SQL 

3. CREATE_as_Cache_BaseItemCollection is always the latest schema 

4. All other files are to be run in the order that they were created. The first one is a create table, and the rest are alter statements. 
We do this so that if a search index is in an older schema, we can upgrade it. 
