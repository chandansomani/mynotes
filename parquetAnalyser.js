const zlib = require('zlib');
const fs = require('fs');
const path = require('path');
const parquet = require('parquetjs');

async function findDuplicatesFromGzip(gzFilePath, columnsToCheck) {
    const tempFilePath = path.join(__dirname, 'temp.parquet'); // Temporary decompressed file path

    try {
        // Step 1: Decompress the .gz file into a temporary file
        await new Promise((resolve, reject) => {
            const input = fs.createReadStream(gzFilePath);
            const output = fs.createWriteStream(tempFilePath);
            input.pipe(zlib.createGunzip()).pipe(output)
                .on('finish', resolve)
                .on('error', reject);
        });

        // Step 2: Read the decompressed Parquet file
        const reader = await parquet.ParquetReader.openFile(tempFilePath);
        const cursor = reader.getCursor();
        let record;
        const recordCounts = {};

        // Step 3: Process each record and find duplicates
        while ((record = await cursor.next())) {
            // Create a unique key based on the columns to check
            const key = columnsToCheck.map(col => record[col]).join('|');
            if (recordCounts[key]) {
                recordCounts[key]++;
            } else {
                recordCounts[key] = 1;
            }
        }

        await reader.close();

        // Step 4: Find and log duplicates
        const duplicates = Object.entries(recordCounts)
            .filter(([_, count]) => count > 1)
            .map(([key, count]) => ({ key: key.split('|'), count }));

        console.log('Duplicate Records:', duplicates);
        return duplicates;
    } finally {
        // Step 5: Clean up the temporary file
        if (fs.existsSync(tempFilePath)) {
            fs.unlinkSync(tempFilePath);
        }
    }
}

// Example Usage
const gzFilePath = 'path/to/your.parquet.gz';
const columnsToCheck = ['column1', 'column2']; // Replace with your columns
findDuplicatesFromGzip(gzFilePath, columnsToCheck)
    .then(duplicates => console.log('Duplicates found:', duplicates))
    .catch(err => console.error('Error:', err));
