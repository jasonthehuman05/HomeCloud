namespace HomeCloud_Server
{
    public static class Utils
    {
        public static string GenerateRandomString(int length)
        {
            List<char> characterList = new List<char>();
            // Adding '0' to '9'
            for (char c = '0'; c <= '9'; c++)
            {
                characterList.Add(c);
            }

            // Adding 'A' to 'Z'
            for (char c = 'A'; c <= 'Z'; c++)
            {
                characterList.Add(c);
            }

            // Adding 'a' to 'z'
            for (char c = 'a'; c <= 'z'; c++)
            {
                characterList.Add(c);
            }

            var r = new Random();
            return new String(Enumerable.Range(0, length).Select(n => characterList[r.Next(0,characterList.Count)]).ToArray());
        }
    }
}
