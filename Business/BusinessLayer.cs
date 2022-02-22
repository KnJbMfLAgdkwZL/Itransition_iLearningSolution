namespace Business;

public class BusinessLayer
{
    public static string GetStr()
    {
        return "Business.BusinessLayer.GetStr()";
    }

    public static string GetStrAll()
    {
        return DataAccess.DataAccessLayer.GetStrAll() + "1";
    }
    
}