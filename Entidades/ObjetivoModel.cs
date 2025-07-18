﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Entidades
{
    public class ObjetivoModel
    {
        public int IdObjetivo { get; set; }
        public string Objetivo { get; set; }
        public decimal Porcentaje { get; set; }
        public string Tipo { get; set; }
        public int? IdTipoObjetivo { get; set; }


        public List<SelectListItem> TiposObjetivos { get; set; }

    }
}
