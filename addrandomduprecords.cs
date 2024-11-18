private void testDuplicatesButton_Click(object sender, EventArgs e)
{
    try
    {
        if (this.MainDataSource is null || this.MainDataSource.Rows.Count == 0)
        {
            MessageBox.Show("No data available to duplicate records.", "Test Duplicates", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Generate 5 duplicates at random positions
        CreateRandomDuplicates(this.MainDataSource, 5);

        MessageBox.Show("5 duplicate records have been added at random locations.", "Test Duplicates", MessageBoxButtons.OK, MessageBoxIcon.Information);

        this.toolStripStatusSearch.Text = "Test duplicates created.";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred while creating duplicates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.toolStripStatusSearch.Text = "Error occurred during duplicate creation.";
    }
}

private void CreateRandomDuplicates(DataTable dataTable, int duplicateCount)
{
    Random random = new Random();
    int rowCount = dataTable.Rows.Count;

    if (rowCount == 0)
        throw new InvalidOperationException("Dataset is empty. Cannot create duplicates.");

    for (int i = 0; i < duplicateCount; i++)
    {
        // Select a random row to duplicate
        int randomRowIndex = random.Next(0, rowCount);
        DataRow originalRow = dataTable.Rows[randomRowIndex];

        // Create a duplicate row
        DataRow duplicateRow = dataTable.NewRow();
        duplicateRow.ItemArray = originalRow.ItemArray.Clone() as object[];

        // Insert the duplicate at a random position
        int randomInsertIndex = random.Next(0, rowCount + 1);
        dataTable.Rows.InsertAt(duplicateRow, randomInsertIndex);
        rowCount++; // Update rowCount as we're adding new rows
    }
}
