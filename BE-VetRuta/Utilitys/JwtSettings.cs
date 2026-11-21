namespace BE_VetRuta.Utilitys
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessTokenMinutos { get; set; } = 60;
        public int RefreshTokenDias { get; set; } = 7;
    }
}
