using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Parquet;
using Parquet.Data;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            // Path to the Parquet file
            string filePath = "your-parquet-file.parquet";

            // Open the file stream
            using (Stream fileStream = File.OpenRead(filePath))
            {
                // Use CreateAsync to asynchronously create a ParquetReader
                using (var parquetReader = await ParquetReader.CreateAsync(fileStream))
                {
                    // Initialize a DataTable to store the data
                    DataTable dataTable = new DataTable();

                    // Read the schema (column definitions)
                    Schema schema = parquetReader.Schema;

                    // Add columns to the DataTable
                    foreach (DataField dataField in schema.GetDataFields())
                    {
                        dataTable.Columns.Add(dataField.Name, typeof(string)); // Simplified to string; adjust based on your data type
                    }

                    // Loop through row groups
                    for (int i = 0; i < parquetReader.RowGroupCount; i++)
                    {
                        using (var rowGroupReader = await parquetReader.OpenRowGroupReaderAsync(i))
                        {
                            foreach (DataField dataField in schema.GetDataFields())
                            {
                                // Read the column data asynchronously
                                Parquet.Data.DataColumn parquetColumnData = await rowGroupReader.ReadColumnAsync(dataField);

                                // Extract the column values
                                object[] columnValues = parquetColumnData.Data;

                                // Add rows to the DataTable (one per value)
                                for (int rowIndex = 0; rowIndex < columnValues.Length; rowIndex++)
                                {
                                    if (dataTable.Rows.Count <= rowIndex)
                                    {
                                        dataTable.Rows.Add();
                                    }
                                    dataTable.Rows[rowIndex][dataField.Name] = columnValues[rowIndex];
                                }
                            }
                        }
                    }

                    // Display the DataTable
                    Console.WriteLine("Data from Parquet file:");
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Console.Write($"{column.ColumnName}\t");
                    }
                    Console.WriteLine();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (var item in row.ItemArray)
                        {
                            Console.Write($"{item}\t");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
