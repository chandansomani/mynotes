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
                Console.WriteLine("File not found: " + filePath);
                return;
            }

            try
            {
                using (Stream fileStream = File.OpenRead(filePath))
                {
                    // Open the Parquet file
                    using (ParquetReader parquetReader = new ParquetReader(fileStream))
                    {
                        // Iterate through row groups
                        for (int groupIndex = 0; groupIndex < parquetReader.RowGroupCount; groupIndex++)
                        {
                            using (ParquetRowGroupReader groupReader = parquetReader.OpenRowGroupReader(groupIndex))
                            {
                                // Read data fields
                                foreach (DataField field in parquetReader.Schema.GetDataFields())
                                {
                                    DataColumn column = groupReader.ReadColumn(field);
                                    Console.WriteLine($"Column: {field.Name}");

                                    // Print top 10 rows
                                    for (int i = 0; i < Math.Min(10, column.Data.Length); i++)
                                    {
                                        Console.WriteLine($"  Row {i + 1}: {column.Data.GetValue(i)}");
                                    }
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
