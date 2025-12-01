using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocios.Services
{

    public interface IMantenimientosService
    {
        FuncionarioNegocios Funcionario { get; }
        PuestosNegocios Puestos { get; }
        ConglomeradosNegocios Conglomerados { get; }
        DependenciasNegocios Dependencias { get; }
        RolesNegocios Roles { get; }
        CompetenciasNegocios Competencias { get; }
        EvaluacionesNegocio Evaluaciones { get; }
        EstadoEvaluacionNegocios EstadoEvaluacion { get; }
        EvaluacionXcompetenciaNegocios EvaluacionXcompetencia { get; }
        EvaluacionXObjetivosNegocios EvaluacionXobjetivos { get; }
        FuncionarioXConglomeradoNegocios FuncionarioXConglomerado { get; }
        MetaXObjetivoNegocios MetaXObjetivo { get; }
        ObjetivoNegocios Objetivo { get; }
        PesosConglomeradoNegocios PesosConglomerado { get; }
        TiposCompetenciasNegocios TiposCompetencias { get; }
        TiposObjetivosNegocios TiposObjetivos { get; }
        EstadoFuncionariosNegocios EstadoFuncionarios { get; }
        AreasNegocios Areas { get; }
        FuncionarioPorAreaNegocios FuncionarioPorArea { get; }
        CarrerasNegocios Carreras { get; }
        PeriodosNegocios Periodos { get;}
        ComportamientosNegocios Comportamientos { get; }
        NivelesComportamientosNegocios NivelesComportamientos { get; }
        CompetenciaPorComportamientoNegocios CompetenciaPorComportamiento { get; }
        ComportamientoPorNivelNegocios ComportamientoPorNivel { get; }
        ObtenerComportamientosYDescripcionesNegocios ObtenerComportamientosYDescripciones { get; }
        ReportesNegocios ReportesNegocios { get; }
        CorreoService CorreoService { get; }
    }
}
