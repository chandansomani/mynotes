using System.Data;
using Parquet;

public class ParquetToDataTable
{
    public static DataTable LoadParquet(string filePath)
    {
        DataTable dataTable = new DataTable();

        using (Stream fileStream = File.OpenRead(filePath))
        {
            using (var parquetReader = new ParquetReader(fileStream))
            {
                // Get the schema information from the Parquet file
                var schema = parquetReader.Schema;

                // Create columns in the DataTable based on the schema
                foreach (var column in schema.RootGroup.Fields)
                {
                    var dataTypeName = GetDataTypeFromParquetType(column.DataType);
                    dataTable.Columns.Add(column.Name, dataTypeName);
                }

                // Read the data rows from the Parquet file
                var dataRowCollection = parquetReader.ReadRowGroup(0); // Read the first row group
                foreach (var dataRow in dataRowCollection)
                {
                    DataRow row = dataTable.NewRow();

                    // Add each data cell value to the corresponding column in the row
                    for (int i = 0; i < dataRow.Count; i++)
                    {
                        row[i] = dataRow[i]; // Assuming data types directly match between parquet and DataTable
                    }

                    dataTable.Rows.Add(row);
                }
            }
        }

        return dataTable;
    }

    private static Type GetDataTypeFromParquetType(Parquet.Thrift.Type type)
    {
        switch (type)
        {
            case Parquet.Thrift.Type.INT32:
                return typeof(int);
            case Parquet.Thrift.Type.INT64:
                return typeof(long);
            case Parquet.Thrift.Type.DOUBLE:
                return typeof(double);
            case Parquet.Thrift.Type.FLOAT:
                return typeof(float);
            case Parquet.Thrift.Type.BOOL:
                return typeof(bool);
            case Parquet.Thrift.Type.BYTE_ARRAY:
                return typeof(byte[]);
            case Parquet.Thrift.Type.UTF8:
                return typeof(string);
            default:
                // Handle unsupported data types appropriately (e.g., throw an exception)
                throw new NotImplementedException($"Data type '{type}' is not currently supported");
        }
    }
}
