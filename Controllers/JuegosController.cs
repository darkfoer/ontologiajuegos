using MVCJuegos.Models;
using Microsoft.AspNetCore.Mvc;
using VDS.RDF.Query;

namespace MVCJuegos.Controllers
{
    public class JuegosController : Controller

    {
        private static SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://localhost:3030/Juegos/"));

        private readonly ILogger<JuegosController> _logger;

        public JuegosController(ILogger<JuegosController> logger)
        {
            _logger = logger;
        }

        // Define los prefijos
        string prefixes =
            "prefix juego: <http://www.semanticweb.org/alexa/ontologies/2024/3/untitled-ontology-21#> " +
            "prefix owl: <http://www.w3.org/2002/07/owl#> " +
            "prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
            "prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
            "prefix xsd: <http://www.w3.org/2001/XMLSchema#> ";

        public IActionResult Index(int? pagina, string searchQuery)
        {
            int cantidadPorPagina = 20; // Número de juegos por página
            int paginaNumero = pagina ?? 1; // Página predeterminada si no se especifica
            try
            {
                string query = prefixes +
                    "select " +
                    "?NombreJuego " +
                    "?Clasificacion " +
                    "?ValorNota " +
                    "(group_concat(distinct ?Plataforma; separator=', ') as ?Plataformas) " +
                    "(group_concat(distinct ?Genero; separator=', ') as ?ListaGeneros) " +
                    "where{" +
                    "?x rdf:type juego:juego. " +
                    "?x juego:nombreJuego ?NombreJuego. " +
                    "?x juego:clasificacionJuego ?Clasificacion. " +
                    "?x juego:valorNota ?ValorNota. " +
                    "?x juego:esta ?cr. " +
                    "?cr juego:nombrePlaforma ?Plataforma. " +
                    "?x juego:pertenece ?cr2. " +
                    "?cr2 juego:nombreGenero ?Genero. ";

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query += $"FILTER(CONTAINS(LCASE(STR(?NombreJuego)), \"{searchQuery.ToLower()}\")) ";
                }

                query += "} group by ?NombreJuego ?Clasificacion ?ValorNota";

                // Ejecuta la consulta
                SparqlResultSet resultado = endpoint.QueryWithResultSet(query);
                List<Juego> list = new List<Juego>();
                foreach (var result in resultado.Results)
                {
                    Juego juegos = new Juego();
                    var dato = result.ToList();
                    juegos.nombreJuego = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.clasificacionJuego = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");
                    juegos.valorNotaJuego = dato[2].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");

                    juegos.plataformas = new List<Plataforma>();
                    Plataforma plataforma = new Plataforma();
                    plataforma.nombrePlataforma = dato[3].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.plataformas.Add(plataforma);

                    juegos.generos = new List<Genero>();
                    Genero genero = new Genero();
                    genero.nombreGenero = dato[4].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.generos.Add(genero);
                    list.Add(juegos);
                }

                // Calcular el total de páginas
                int totalJuegos = list.Count;
                int totalPaginas = (int)Math.Ceiling((double)totalJuegos / cantidadPorPagina);

                // Obtener los juegos para la página actual
                var juegosPaginados = list.Skip((paginaNumero - 1) * cantidadPorPagina)
                                           .Take(cantidadPorPagina);

                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.PaginaActual = paginaNumero;
                ViewBag.SearchQuery = searchQuery;
                return View(juegosPaginados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los juegos");
                // Redirigir a una vista de error
                return RedirectToAction("Error", "Home", new { errorMessage = "Ha ocurrido un error al obtener los juegos." });
            }
        }

        [HttpGet]
        public PartialViewResult Search(string query, int? pagina)
        {
            int cantidadPorPagina = 20; // Número de juegos por página
            int paginaNumero = pagina ?? 1; // Página predeterminada si no se especifica
            try
            {
                string sparqlQuery = prefixes +
                    "select " +
                    "?NombreJuego " +
                    "?Clasificacion " +
                    "?ValorNota " +
                    "(group_concat(distinct ?Plataforma; separator=', ') as ?Plataformas) " +
                    "(group_concat(distinct ?Genero; separator=', ') as ?ListaGeneros) " +
                    "where{" +
                    "?x rdf:type juego:juego. " +
                    "?x juego:nombreJuego ?NombreJuego. " +
                    "?x juego:clasificacionJuego ?Clasificacion. " +
                    "?x juego:valorNota ?ValorNota. " +
                    "?x juego:esta ?cr. " +
                    "?cr juego:nombrePlaforma ?Plataforma. " +
                    "?x juego:pertenece ?cr2. " +
                    "?cr2 juego:nombreGenero ?Genero. ";

                if (!string.IsNullOrEmpty(query))
                {
                    sparqlQuery += $"FILTER(CONTAINS(LCASE(STR(?NombreJuego)), \"{query.ToLower()}\")) ";
                }

                sparqlQuery += "} group by ?NombreJuego ?Clasificacion ?ValorNota";

                // Ejecuta la consulta
                SparqlResultSet resultado = endpoint.QueryWithResultSet(sparqlQuery);
                List<Juego> list = new List<Juego>();
                foreach (var result in resultado.Results)
                {
                    Juego juegos = new Juego();
                    var dato = result.ToList();
                    juegos.nombreJuego = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.clasificacionJuego = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");
                    juegos.valorNotaJuego = dato[2].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");

                    juegos.plataformas = new List<Plataforma>();
                    Plataforma plataforma = new Plataforma();
                    plataforma.nombrePlataforma = dato[3].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.plataformas.Add(plataforma);

                    juegos.generos = new List<Genero>();
                    Genero genero = new Genero();
                    genero.nombreGenero = dato[4].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.generos.Add(genero);
                    list.Add(juegos);
                }

                // Calcular el total de páginas
                int totalJuegos = list.Count;
                int totalPaginas = (int)Math.Ceiling((double)totalJuegos / cantidadPorPagina);

                // Obtener los juegos para la página actual
                var juegosPaginados = list.Skip((paginaNumero - 1) * cantidadPorPagina)
                                           .Take(cantidadPorPagina);

                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.PaginaActual = paginaNumero;
                return PartialView("_JuegosPartial", juegosPaginados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los juegos");
                return PartialView("_ErrorPartial", new { errorMessage = "Ha ocurrido un error al obtener los juegos." });
            }
        }


        // Escapado manual para controlar la '' comillas simples en el filtro de laconsulta de informacion
        public static string EscapeSparqlString(string input)
        {
            if (input == null)
                return null;

            return input.Replace("'", @"\'");
        }


        public ActionResult InformacionJuego(String nombreJuego)
        {
            try
            {
                ViewBag.NombreJuego = nombreJuego;
                string escapedNombreJuego = EscapeSparqlString(nombreJuego);
                List<Juego> list = new List<Juego>();

                string query =
                 prefixes +
                "SELECT" +
                "  ?NombreJuego " +
                "  (COALESCE(?ValorNota, 'no registra') AS ?ValorNotaNuevo)" +
                "  (COALESCE(?Clasificacion, 'no registra') AS ?ClasificacionNuevo)" +
                "  (COALESCE(?Dia, 'no registra') AS ?DiaNuevo)" +
                "  (COALESCE(?Mes, 'no registra') AS ?MesNuevo)" +
                "  (COALESCE(?Año, 'no registra') AS ?AñoNuevo)" +
                "  (group_concat(distinct COALESCE(?Plataforma, 'no registra'); separator=', ') as ?Plataformas)" +
                "  (group_concat(distinct COALESCE(?Genero, 'no registra'); separator=', ') as ?ListaGeneros)" +
                "  (group_concat(distinct COALESCE(?Desarrollador, 'no registra'); separator =', ') as ?Desarrolladoras)" +
                "  (group_concat(distinct COALESCE(?Distribuidor, 'no registra'); separator = ', ') as ?Distribuidoers)" +
                "  (group_concat(distinct COALESCE(?Formato, 'no registra'); separator =', ') as ?Formatos)" +
                "  (group_concat(distinct COALESCE(?ModoJuego, 'no registra'); separator = ', ') as ?ModoJuegos)" +
                "  (group_concat(distinct COALESCE(?ModoDisponible, 'no registra'); separator = ', ') as ?ModoDisponibles)" +
                "  (group_concat(distinct COALESCE(?Voz, 'no registra'); separator =', ') as ?Voces)" +
                "  (group_concat(distinct COALESCE(?Interfaz, 'no registra'); separator =', ') as ?Interfazes)" +

                "WHERE {" +
                "  ?x rdf:type juego:juego." +
                "  ?x juego:nombreJuego ?NombreJuego." +
                "  OPTIONAL { ?x juego:valorNota ?ValorNota }" +
                "  OPTIONAL { ?x juego:clasificacionJuego ?Clasificacion }" +
                "  OPTIONAL { ?x juego:diaLanzamiento ?Dia }" +
                "  OPTIONAL { ?x juego:mesLanzamiento ?Mes }" +
                "  OPTIONAL { ?x juego:annioLanzamiento ?Año }" +
                "  OPTIONAL { ?x juego:esta ?cr." +
                "             ?cr juego:nombrePlaforma ?Plataforma }" +
                "  OPTIONAL { ?x juego:pertenece ?cr2." +
                "             ?cr2 juego:nombreGenero ?Genero }" +
                "  OPTIONAL { ?x juego:esCreado ?cr3." +
                "             ?cr3 juego:nombreDesarrollador ?Desarrollador }" +
                "  OPTIONAL { ?x juego:esDistribuido ?cr4." +
                "             ?cr4 juego:nombreDistribuidor ?Distribuidor }" +
                "  OPTIONAL { ?x juego:cuenta ?cr5." +
                "             ?cr5 juego:nombreFormato ?Formato }" +
                "  OPTIONAL { ?x juego:cuentaCon ?cr6." +
                "             ?cr6 juego:nombreModoJuego ?ModoJuego }" +
                "  OPTIONAL { ?x juego:sePuedeJugar ?cr7." +
                "             ?cr7 juego:nombreModoDisponible ?ModoDisponible }" +
                "  OPTIONAL { ?x juego:tiene ?cr8." +
                "             ?cr8 juego:nombreVoz ?Voz }" +
                "  OPTIONAL { ?x juego:tiene ?cr9." +
                "             ?cr9 juego:nombreInterfaz ?Interfaz }" +
                $"FILTER(contains(lcase(?NombreJuego), lcase('{escapedNombreJuego}')))" +
                "}" +
                "group by ?NombreJuego ?Clasificacion ?ValorNota ?Dia ?Mes ?Año " +
                "limit 1";

                SparqlResultSet resultado = endpoint.QueryWithResultSet(query);
                foreach (var result in resultado.Results)
                {
                    Juego juegos = new Juego();
                    var dato = result.ToList();
                    juegos.nombreJuego = dato[0].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.valorNotaJuego = dato[1].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");
                    juegos.clasificacionJuego = dato[2].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");
                    juegos.diaLanzamientoJuego = dato[3].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");
                    juegos.mesLanzamientoJuego = dato[4].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");
                    juegos.annioLanzamientoJuego = dato[5].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#integer", "");


                    juegos.plataformas = new List<Plataforma>();
                    Plataforma plataforma = new Plataforma();
                    plataforma.nombrePlataforma = dato[6].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.plataformas.Add(plataforma);


                    juegos.generos = new List<Genero>();
                    Genero genero = new Genero();
                    genero.nombreGenero = dato[7].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.generos.Add(genero);


                    juegos.estudiosDesarrollo = new List<EstudioDesarrollo>();
                    EstudioDesarrollo estudioDesarrollo = new EstudioDesarrollo();
                    estudioDesarrollo.nombreDesarrolladora = dato[8].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.estudiosDesarrollo.Add(estudioDesarrollo);

                    juegos.distribuidores = new List<Distribuidor>();
                    Distribuidor distribuidor = new Distribuidor();
                    distribuidor.nombreDistribuidor = dato[9].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.distribuidores.Add(distribuidor);

                    juegos.formatos = new List<Formato>();
                    Formato formato = new Formato();
                    formato.nombreFormato = dato[10].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.formatos.Add(formato);

                    juegos.modoJuegos = new List<ModoJuego>();
                    ModoJuego modoJuego = new ModoJuego();
                    modoJuego.nombreModoJuego = dato[11].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.modoJuegos.Add(modoJuego);


                    juegos.modoDisponibles = new List<ModoDisponible>();
                    ModoDisponible modoDisponible = new ModoDisponible();
                    modoDisponible.nombreModoDisponible = dato[12].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    juegos.modoDisponibles.Add(modoDisponible);


                    juegos.idiomas = new List<Idioma>();
                    Idioma idioma = new Idioma();

                    // Agrega las voces al idioma
                    idioma.vocez = new List<Voz>();
                    Voz voz = new Voz();
                    voz.nombreVoz = dato[13].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    idioma.vocez.Add(voz);

                    // Agrega las interfaces al mismo idioma
                    idioma.interfazes = new List<Interfaz>();
                    Interfaz interfaz = new Interfaz();
                    interfaz.nombreInterfaz = dato[14].Value.ToString().Replace("^^http://www.w3.org/2001/XMLSchema#string", "");
                    idioma.interfazes.Add(interfaz);

                    // Agrega el idioma con voces e interfaces a la lista de idiomas del juego
                    juegos.idiomas.Add(idioma);


                    list.Add(juegos);

                }
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la información del juego {nombreJuego}");
                // Redirigir a una vista de error con un mensaje adecuado
                return RedirectToAction("Error", "Home", new { errorMessage = $"Ha ocurrido un error al obtener la información del juego {nombreJuego}." });
            }
        }

        public IActionResult Biblioteca()
        {
            try
            {
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la información");
                // Redirigir a una vista de error con un mensaje adecuado
                return RedirectToAction("Error", "Home", new { errorMessage = $"Ha ocurrido un error al obtener la información." });
            }
        }
    }
}