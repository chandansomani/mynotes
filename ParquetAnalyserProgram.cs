using System;
using System.IO;
using System.Threading.Tasks;
using Parquet;

public class ParquetReaderExample
{
    public static async Task Main(string[] args)
    {
        string filePath = "/mnt/storage/data.parquet"; // Replace with your actual file path

        try
        {
            using (Stream fs = File.OpenRead(filePath))
            {
                using (ParquetReader reader = await ParquetReader.CreateAsync(fs))
                {
                    // Choose a specific column name (adjust based on your schema)
                    string columnName = "my_column_name";

                    if (reader.Schema.TryFindField(columnName, out DataField dataField))
                    {
                        for (int i = 0; i < reader.RowGroupCount; i++)
                        {
                            using (ParquetRowGroupReader rowGroupReader = reader.OpenRowGroupReader(i))
                            {
                                DataColumn columnData = await rowGroupReader.ReadColumnAsync(dataField);

                                // Assuming your column contains strings (modify as needed)
                                if (columnData.DataType == Thrift.Type.UTF8)
                                {
                                    string[] stringValues = columnData.GetStringArray();
                                    Console.WriteLine($"Row Group {i}:");
                                    for (int j = 0; j < stringValues.Length; j++)
                                    {
                                        Console.WriteLine($"- {stringValues[j]}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Column '{columnName}' has unsupported data type: {columnData.DataType}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Column '{columnName}' not found in the Parquet schema.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
