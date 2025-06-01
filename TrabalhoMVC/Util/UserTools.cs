namespace TrabalhoMVC.Util
{
    public static class UserTools
    {
        public static string GerarCPF()
        {
            Random rnd = new Random();
            int[] n = Enumerable.Range(0, 9).Select(_ => rnd.Next(10)).ToArray();

            int d1 = 11 - n.Select((num, index) => num * (10 - index)).Sum() % 11;
            if (d1 >= 10) d1 = 0;

            int d2 = 11 - ((d1 * 2) + n.Select((num, index) => num * (11 - index)).Sum()) % 11;
            if (d2 >= 10) d2 = 0;

            string cpf = string.Concat(n) + d1 + d2;
            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }
    }
}
