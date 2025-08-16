# Basic Data Operations

This section contains documentation for the basic data operations of ArmaRAMDb that allow for simple key-value storage and retrieval.

## Available Functions

- [delete](delete.md) - Delete a value from the database
- [fetch](fetch.md) - Internal function to process data chunks
- [get](get.md) - Get a value from the database
- [load](load.md) - Load the database from disk
- [save](save.md) - Save the database to disk
- [set](set.md) - Set a value in the database

## Example Usage

```sqf
// Set a value
["myKey", [myValue]] call ramdb_db_fnc_set;

// Get a value
["myKey", "myFunction"] call ramdb_db_fnc_get;

// Delete a key
["myKey"] call ramdb_db_fnc_delete;

// Save database to disk
[] call ramdb_db_fnc_save;

// Load the database from disk
[] call ramdb_db_fnc_load;
```

## Related Categories

- [Core Functions](../core/README.md)
- [Hash Operations](../hash/README.md)
- [List Operations](../list/README.md)