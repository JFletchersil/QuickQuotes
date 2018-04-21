using System;
using System.Collections.Generic;
using System.Linq;
using AngularApp.API.Models.Helpers;

namespace AngularApp.API.Helpers
{

    /// <summary>
    /// A collection of classes designed to make it easier to carry out some functions
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// Provides a collection of helper functions, mostly to avoid repeating code within the API
    /// </summary>
    public class CommonFunctionsHelper
    {
        /// <summary>
        /// Getting the type, then the property allows us to set the property by specifying a 
        /// string parameter rather than specifying a specific parameter to order by.
        /// This allows the code to be used in a generic fashion.
        /// </summary>
        /// <typeparam name="T">This is needed to ascertain the object type being worked on, must be present</typeparam>
        /// <param name="items">The list to be sorted</param>
        /// <param name="orderBy">The parameter to be sorted by</param>
        /// <returns>
        /// A sorted list of type T
        /// </returns>
        /// <remarks>
        /// The orderBy is expected to be a valid parameter type for a module, with potentially a - on it.
        /// If - is on it, descending order is presumed, else the order is presumed to be ascending.
        /// </remarks>
        public List<T> ReturnSortedList<T>(List<T> items, string orderBy)
        {
            try
            {
                // Determines if the sorting should be in descending order
                // Also determines the true strue to be used to find the property
                // Finds the property
                if (string.IsNullOrWhiteSpace(orderBy)) return items;
                var isDescendingOrder = orderBy.Contains("-");
                var orderByString = isDescendingOrder ? orderBy.Split('-')[1] : orderBy;
                var prop = typeof(T).GetProperty(orderByString);

                // If the prop is null, something has gone horribly wrong, throw an exception
                if (prop == null)
                {
                    throw new NullReferenceException("Unable to find property, please check the property name and try again");
                }
                // Else return the result of the ordering
                return isDescendingOrder ? items.OrderByDescending(x => prop.GetValue(x, null)).ToList()
                    : items.OrderBy(x => prop.GetValue(x, null)).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Paginates the give items in a generic fashion
        /// </summary>
        /// <typeparam name="T">This is needed to ascertain the object type being worked on, must be present</typeparam>
        /// <param name="items">The list to be sorted</param>
        /// <param name="pageNumber">The page from which the user wishes to look at results</param>
        /// <param name="pageSize">The indiviudal sizes of each page</param>
        /// <returns>A model containing the total number of pages and the paginated items</returns>
        public PagedResultsPageSizeModel<T> PaginateDbTables<T>(List<T> items, int pageNumber, int pageSize)
        {
            try
            {
                var totalPages = (int)Math.Ceiling(items.Count / (double)pageSize);
                // Gathers the items for the given page location and size for return.
                return new PagedResultsPageSizeModel<T>
                {
                    Items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                    TotalPages = totalPages
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}