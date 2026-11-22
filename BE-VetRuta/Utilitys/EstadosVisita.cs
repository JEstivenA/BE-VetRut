namespace BE_VetRuta.Utilitys
{
    public static class EstadosVisita
    {
        public const string Pendiente = "PENDIENTE";
        public const string Realizada = "REALIZADA";
        public const string Cancelada = "CANCELADA";
        public const string Atrasada = "ATRASADA";

        public static string Calcular(string estado, DateTime fechaProgramada) =>
       estado == Pendiente && fechaProgramada.Date < DateTime.Today ? Atrasada : estado;
    }
}
