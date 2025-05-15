/* As referenced in the READ.ME file This is where editing and add table schemas will need to be updated.
 * After table creation. See Microsoft Documentation https://learn.microsoft.com/en-us/sql/relational-databases/tables/create-tables-database-engine?view=sql-server-ver16
 */
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SqlFlatFileImporter.Import
{
    public static class TableSchemas
    {
        // This method returns the schema (DataTable structure) for a specific table
        public static DataTable GetSchema(string tableName)
        {
            // Create a new DataTable to hold the schema
            var table = new DataTable(tableName);

            switch (tableName.ToLower())
            {
                case "product":
                    return GetProductSchema();
                // Add more tables here with their respective schemas
                default:
                    throw new ArgumentException($"Schema for table {tableName} not defined.");
            }
        }

        // Example: Product Table Schema
        private static DataTable GetProductSchema()
        {
            var table = new DataTable("product");

            // Define columns for the 'product' table
            table.Columns.Add("product_number", typeof(string));
            table.Columns.Add("product_description", typeof(string));
            table.Columns.Add("pline_num", typeof(string));  
            table.Columns.Add("sell_pkg", typeof(string));
            table.Columns.Add("trn_pkg_qty", typeof(int));
            table.Columns.Add("obsolete", typeof(bool));
            table.Columns.Add("obs_date", typeof(DateTime));  
            table.Columns.Add("sugg_ret", typeof(decimal));  
            table.Columns.Add("per_sugg_ret", typeof(decimal));  
            table.Columns.Add("bas_sugg_ret", typeof(decimal));  
            table.Columns.Add("rep_cost", typeof(decimal));  
            table.Columns.Add("ppg_num", typeof(string));  
            table.Columns.Add("un_meas_buy", typeof(string));  
            table.Columns.Add("un_meas_stk", typeof(string));  
            table.Columns.Add("comm_ov_flg", typeof(string));  
            table.Columns.Add("reg_typ_ins", typeof(string));  
            table.Columns.Add("reg_pct_ins", typeof(decimal));  
            table.Columns.Add("dir_typ_ins", typeof(string));  
            table.Columns.Add("dir_pct_ins", typeof(decimal));  
            table.Columns.Add("reg_typ_out", typeof(string));  
            table.Columns.Add("reg_pct_out", typeof(decimal));  
            table.Columns.Add("dir_typ_out", typeof(string));  
            table.Columns.Add("dir_pct_out", typeof(decimal));  
            table.Columns.Add("reg_typ_wb", typeof(string));
            table.Columns.Add("reg_pct_wb", typeof(decimal));  
            table.Columns.Add("dir_typ_wb", typeof(string)); 
            table.Columns.Add("dir_pct_wb", typeof(decimal)); 
            table.Columns.Add("pc_factor", typeof(decimal));  
            table.Columns.Add("sq_foot_fact", typeof(decimal));  
            table.Columns.Add("prod_type", typeof(string)); 
            table.Columns.Add("est_date", typeof(DateTime)); 
            table.Columns.Add("substitutes", typeof(string)); 
            table.Columns.Add("weight", typeof(decimal)); 
            table.Columns.Add("superceded_flag", typeof(bool));
            table.Columns.Add("superceding", typeof(string)); 
            table.Columns.Add("price5", typeof(decimal)); 

            return table;
        }
    }
}
