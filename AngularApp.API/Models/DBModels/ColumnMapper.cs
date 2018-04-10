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
    /// <see cref="Controllers.QuoteController" />
    [TableName("INFORMATION_SCHEMA.COLUMNS")]
    public class ColumnMapper
    {
        /// <summary>
        /// The column Name for a given column
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }
    }
}