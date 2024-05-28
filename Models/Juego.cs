namespace MVCJuegos.Models
{
    public class Juego
    {
        public String? nombreJuego { get; set; }
        public String? valorNotaJuego { get; set; }
        public String? clasificacionJuego { get; set; }
        public String? diaLanzamientoJuego { get; set; }
        public String? mesLanzamientoJuego { get; set; }
        public String? annioLanzamientoJuego { get; set; }
        public List<Distribuidor> distribuidores { get; set; }
        public List<EstudioDesarrollo> estudiosDesarrollo { get; set; }
        public List<Plataforma> plataformas { get; set; }
        public List<Genero> generos { get; set; }
        public List<Formato> formatos { get; set; }
        public List<ModoJuego> modoJuegos { get; set; }
        public List<ModoDisponible> modoDisponibles { get; set; }
        public List<Idioma> idiomas { get; set; }


    }

}
