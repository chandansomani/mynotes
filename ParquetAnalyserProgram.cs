using System;
using System.IO;
using Parquet;
using Parquet.Data;

namespace ParquetReaderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Specify the Parquet file path
            string filePath = "path/to/your/parquet-file.parquet";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            try
            {
                // Open the Parquet file
                using (Stream fileStream = File.OpenRead(filePath))
                {
                    // Use ParquetReader to load the file
                    using (var parquetReader = new ParquetReader(fileStream))
                    {
                        // Get the schema of the file
                        Schema schema = parquetReader.Schema;
                        Console.WriteLine("File Schema:");
                        foreach (var field in schema.Fields)
                        {
                            Console.WriteLine($"  {field.Name} ({field.ClrType.Name})");
                        }

                        // Read each row group
                        for (int i = 0; i < parquetReader.RowGroupCount; i++)
                        {
                            // Read the rows from the row group
                            DataColumn[] dataColumns = parquetReader.ReadEntireRowGroup(i);

                            // Display rows (up to 10 rows per column)
                            for (int col = 0; col < dataColumns.Length; col++)
                            {
                                Console.WriteLine($"Column: {dataColumns[col].Field.Name}");
                                object[] data = dataColumns[col].Data;

                                for (int row = 0; row < Math.Min(10, data.Length); row++)
                                {
                                    Console.WriteLine($"  Row {row + 1}: {data[row]}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while reading the Parquet file:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
