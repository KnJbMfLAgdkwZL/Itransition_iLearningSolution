namespace DataAccess;

public class DataAccessLayer
{
    public static string GetStr()
    {
        return "DataAccess.DataAccessLayer.GetStr()";
    }
    public static string GetStrAll()
    {
        return Database.DatabaseLayer.GetStrAll() + "2";
    }
}