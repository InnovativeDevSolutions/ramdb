# List Operations

This section contains documentation for the list operations of ArmaRAMDb that allow for working with ordered collections of items.

## Available Functions

- [lpush | rpush](listAdd.md) - Add an item to a list
- [lindex](listGet.md) - Get items from a list
- [lrange](listLoad.md) - Load a list from the database
- [lrem](listRemove.md) - Remove an item from a list
- [lset](listSet.md) - Set an item in a list

## Example Usage

```sqf
// Add an item to a list
["myList", ["myItem"]] call ramdb_db_fnc_lpush;
["myList", ["myItem"]] call ramdb_db_fnc_rpush;

// Set an item at a specific index
["myList", 0, [myNewValue]] call ramdb_db_fnc_lset;

// Get an item at a specific index
["myList", 0, "myFunction"] call ramdb_db_fnc_lindex;

// Load all items from a list
["myList", "myFunction"] call ramdb_db_fnc_lrange;

// Remove an item at a specific index
["myList", 0] call ramdb_db_fnc_lrem;
```

## Related Categories

- [Core Functions](../core/README.md)
- [Basic Data Operations](../basic/README.md)
- [Hash Operations](../hash/README.md)