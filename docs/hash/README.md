# Hash Operations

This section contains documentation for the hash operations of ArmaRAMDb that allow for working with hash tables (key-value pairs within a namespace).

## Available Functions

- [hget](hashGet.md) - Get a field from a hash
- [hgetall](hashGetAll.md) - Get all fields from a hash
- [hrem](hashRemove.md) - Remove a field from a hash
- [hset](hashSet.md) - Set a field in a hash
- [hmset](hashSetBulk.md) - Set multiple fields in a hash in one operation

## Example Usage

```sqf
// Context mode examples
["", "myField", [myValue]] call ramdb_db_fnc_hset;
["", "myField"] call ramdb_db_fnc_hget;
[] call ramdb_db_fnc_hgetall;
["", "myField"] call ramdb_db_fnc_hrem;

// Set multiple hash fields in one operation
["", ["loadout", [getUnitLoadout player], "position", [getPosASL player], "direction", [getDir player]]] call ramdb_db_fnc_hmset;

// ID-specific examples
["myKey", "myField", [myValue]] call ramdb_db_fnc_hset;
["myKey", "myField"] call ramdb_db_fnc_hget;
["myKey"] call ramdb_db_fnc_hgetall;
["myKey", "myField"] call ramdb_db_fnc_hrem;

// Set multiple hash fields in on operation
[getPlayerUID player, ["loadout", [getUnitLoadout player], "position", [getPosASL player], "direction", [getDir player]]] call ramdb_db_fnc_hmset;
```

## Related Categories

- [Core Functions](../core/README.md)
- [Basic Data Operations](../basic/README.md)
- [List Operations](../list/README.md)