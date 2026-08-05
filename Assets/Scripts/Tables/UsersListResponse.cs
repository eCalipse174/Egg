using System;

[Serializable]
public class UsersListResponse
{
    public Users[] list;
}

[Serializable]
public class IdOnlyResponse
{
    public int id;
}