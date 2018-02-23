using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace AngularApp.API.Models.DBModels
{
    [TableName("INFORMATION_SCHEMA.COLUMNS")]
    public class ColumnMapper
    {
        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }
    }
}