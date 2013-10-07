using System;
public class OAuth{
	public static string GenerateNonce() {
        // Just a simple implementation of a random number between 123400 and 9999999
        return UnityEngine.Random.Range(123400, 9999999).ToString();            
    }
	
    public static string GenerateTimeStamp() {
        // Default implementation of UNIX time of the current UTC time
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();            
    }
}

