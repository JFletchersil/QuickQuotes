using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace AngularApp.API.Models.DBModels
{
    /// <summary>
    /// Provides a class to map the information schema of a table, this is used to generically gather
    /// the parameters within a table.
    /// </summary>
    [TableName("INFORMATION_SCHEMA.COLUMNS")]
    public class ColumnMapper
    {
        /// <summary>
        /// The column Name for a given column
        /// </summary>
        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }
    }
}