namespace BE_VetRuta.Utilitys
{
    public static class Roles
    {
        public const string Root = "ROOT";
        public const string Director = "DIRECTOR";
        public const string Medico = "MEDICO";
        public const string Tecnico = "TECNICO";

        public const string Negocio = $"{Director},{Medico},{Tecnico}";
    }
}
