using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace AngularApp.API.Models.DBModels
{
    /// <summary>
    /// A collection of models designed to help operate the database modelling of the system.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// Provides a class to map the information schema of a table, this is used to generically gather
    /// the parameters within a table.
    /// </summary>
    /// <seealso cref="Controllers.QuoteController" />
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