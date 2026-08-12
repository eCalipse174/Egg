using System;

[Serializable]
public class Users
{
    public int id;
    public int device_id;
    public string nickname;
    public long gold;
    public int gacha_count;
    public int enhance_level;
    public int inventory_capacity;
    public int play_time_seconds;
    public string created_at;
    public int equipped_egg_id;
}