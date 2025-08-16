---
title: ArmaRAMDb - Hash Set Bulk
icon: mdi:file-text-outline
excerpt: Set multiple fields in the current/specific client's hash table in RAMDb.
---

# ramdb_db_fnc_hmset

## Description
Sets multiple field-value pairs in the hash table associated with the current/specific client/player in a single operation. This function allows efficiently storing multiple related fields at once, reducing the number of separate database calls required.

## Syntax
```sqf
[_key, _data] call ramdb_db_fnc_hmset
```

## Parameters
| Parameter | Type  | Description                                             | Default |
|-----------|-------|---------------------------------------------------------|---------|
| `_key`      | String  | Key to set the data in                              | ""      |
| `_data`   | Array | Array of alternating field names and values to store    | []      |

## Return Value
None. The operation runs synchronously to store all the data.

## Examples
### Store player loadout and position:
```sqf
["", ["loadout", [getUnitLoadout player], "position", [getPosASLVisual player]]] call ramdb_db_fnc_hmset;
[getPlayerUID player, ["loadout", [getUnitLoadout player], "position", [getPosASLVisual player]]] call ramdb_db_fnc_hmset;
```

### Store multiple player settings:
```sqf
["mykey", ["difficulty", ["regular"], "respawn", [true], "tickets", [500]]] call ramdb_db_fnc_hmset;
```

### Store player data from a client:
```sqf
["", ["name", [name player], "uid", [getPlayerUID player], "score", [score player]]] remoteExecCall ["ramdb_db_fnc_hmset", 2, false];
```

## Notes
- The data array must be structured as alternating field names and values: `[field1, value1, field2, value2, ...]`
- Each field name must be a string
- Values can be arrays, strings, numbers, or booleans
- All field-value pairs are stored in a single database operation
- If any of the fields already exist, their values will be overwritten
- More efficient than multiple individual `hashSet` calls when setting several fields
- The operation is executed immediately and synchronously
- All operations are logged for debugging purposes

## Related Functions
- `ramdb_db_fnc_hget`: Retrieves a field value from the current/specific client's hash table
- `ramdb_db_fnc_hgetall`: Retrieves all fields from the current/specific client's hash table
- `ramdb_db_fnc_hrem`: Removes a field from the current/specific client's hash table
- `ramdb_db_fnc_hset`: Sets a field value pair in the current/specific client's hash table

## Links

[Hash Delete](hashDelete.md) |
[Hash Get](hashGet.md) |
[Hash Get All](hashGetAll.md) |
[Hash Remove](hashRemove.md) |
[Hash Set](hashSet.md) |
[Hash Set Bulk](hashSetBulk.md)