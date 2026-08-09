using System;
using System.Collections.Generic;

[Serializable]
public class UserCollections
{
    public int id;
    public int item_id;
    public int user_id;
    public string unlocked_at;
}

[Serializable]
public class UserCollectionListResponse
{
    public List<UserCollections> list;
}