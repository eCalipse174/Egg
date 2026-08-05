using System;

[Serializable]
public class UserItems
{
    public int id;
    public int user_id;
    public int item_id;
    public int slot_index;
}

[Serializable]
public class UserItemListResponse
{
    public UserItems[] list;
}