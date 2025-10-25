using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocios.Services
{

    ///<summary>
    /// Servicio agrupador que centraliza las clases de negocio relacionadas con mantenimientos.
    /// Permite pasar todos los mantenimientos en 1 sola clase de los controladores
    /// Facilita la inyección de dependencias y el acceso organizado a Funcionario, Puestos,
    /// Conglomerados y otros desde los controladores.
    /// </summary>
    public class MantenimientosService : IMantenimientosService
    {
        public FuncionarioNegocios Funcionario { get; }
        public PuestosNegocios Puestos { get; }
        public ConglomeradosNegocios Conglomerados { get; }
        public DependenciasNegocios Dependencias { get; }
        public RolesNegocios Roles { get; }
        public CompetenciasNegocios Competencias { get; }
        public EvaluacionesNegocio Evaluaciones { get; }
        public EstadoEvaluacionNegocios EstadoEvaluacion { get; }
        public EvaluacionXcompetenciaNegocios EvaluacionXcompetencia { get; }
        public EvaluacionXObjetivosNegocios EvaluacionXobjetivos { get; }
        public FuncionarioXConglomeradoNegocios FuncionarioXConglomerado { get; }
        public MetaXObjetivoNegocios MetaXObjetivo { get; }
        public ObjetivoNegocios Objetivo { get; }
        public PesosConglomeradoNegocios PesosConglomerado { get; }
        public TiposCompetenciasNegocios TiposCompetencias { get; }
        public TiposObjetivosNegocios TiposObjetivos { get; }
        public EstadoFuncionariosNegocios EstadoFuncionarios { get; }
        public AreasNegocios Areas { get; }
        public FuncionarioPorAreaNegocios FuncionarioPorArea { get; }
        public CarrerasNegocios Carreras { get; }
        public PeriodosNegocios Periodos {  get; }
        public ComportamientosNegocios Comportamientos { get; }
        public NivelesComportamientosNegocios NivelesComportamientos { get; }
        public CompetenciaPorComportamientoNegocios CompetenciaPorComportamiento { get; }
        public ComportamientoPorNivelNegocios ComportamientoPorNivel { get; }
        public ObtenerComportamientosYDescripcionesNegocios ObtenerComportamientosYDescripciones {  get; }
        public ReportesNegocios ReportesNegocios { get; }


        public MantenimientosService(
            FuncionarioNegocios funcionario,
            PuestosNegocios puestos,
            ConglomeradosNegocios conglomerados,
            DependenciasNegocios dependencias,
            RolesNegocios roles,
            CompetenciasNegocios competencias,
            EstadoEvaluacionNegocios estadoEvaluacion,
            EvaluacionXcompetenciaNegocios evaluacionXcompetencia,
            EvaluacionXObjetivosNegocios evaluacionXobjetivos,
            FuncionarioXConglomeradoNegocios funcionarioXConglomerado,
            MetaXObjetivoNegocios metaXObjetivo,
            ObjetivoNegocios objetivo,
            PesosConglomeradoNegocios pesosConglomerado,
            TiposCompetenciasNegocios tiposCompetencias,
            TiposObjetivosNegocios tiposObjetivos,
            EstadoFuncionariosNegocios estadoFuncionarios,
            AreasNegocios areas,
            FuncionarioPorAreaNegocios funcionarioPorArea,
            CarrerasNegocios carreras,
            PeriodosNegocios periodos,
            EvaluacionesNegocio evaluaciones,
            ComportamientosNegocios comportamientos,
            NivelesComportamientosNegocios nivelesComportamientos,
            CompetenciaPorComportamientoNegocios competenciaPorComportamiento,
            ComportamientoPorNivelNegocios comportamientoPorNivel,
            ObtenerComportamientosYDescripcionesNegocios obtenerComportamientosYDescripciones,
            ReportesNegocios reportesNegocios

            )
        {
            Funcionario = funcionario;
            Puestos = puestos;
            Conglomerados = conglomerados;
            Dependencias = dependencias;
            Roles = roles;
            Competencias = competencias;
            EstadoEvaluacion = estadoEvaluacion;
            EvaluacionXcompetencia = evaluacionXcompetencia;
            EvaluacionXobjetivos = evaluacionXobjetivos;
            FuncionarioXConglomerado = funcionarioXConglomerado;
            MetaXObjetivo = metaXObjetivo;
            Objetivo = objetivo;
            PesosConglomerado = pesosConglomerado;
            TiposCompetencias = tiposCompetencias;
            TiposObjetivos = tiposObjetivos;
            EstadoFuncionarios = estadoFuncionarios;
            Areas = areas;
            FuncionarioPorArea = funcionarioPorArea;
            Carreras = carreras;
            Periodos = periodos;
            Evaluaciones = evaluaciones;
            Comportamientos = comportamientos;
            NivelesComportamientos = nivelesComportamientos;
            CompetenciaPorComportamiento = competenciaPorComportamiento;
            ComportamientoPorNivel = comportamientoPorNivel;
            ObtenerComportamientosYDescripciones = obtenerComportamientosYDescripciones;
            ReportesNegocios = reportesNegocios;
        }
    }
}
