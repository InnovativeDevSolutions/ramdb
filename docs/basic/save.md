---
title: ArmaRAMDb - Save DB
icon: mdi:file-text-outline
excerpt: Save DB to disc.
---

# ramdb_db_fnc_save

## Description
Saves the entire database to disk storage. This function persists all data (key-value pairs, hash tables, and lists) to a file, allowing it to be retrieved later even after server restart.

## Syntax
```sqf
[_createBackup] call ramdb_db_fnc_save
```

## Parameters
| Parameter       | Type    | Description                                     | Default |
|-----------------|---------|------------------------------------------------|---------|
| `_createBackup` | Boolean | Whether to create a backup of the current state | false   |

## Return Value
None. The operation runs synchronously and saves the database immediately.

## Examples
### Save the database without creating a backup:
```sqf
[] call ramdb_db_fnc_save;
```

### Save the database and create a backup:
```sqf
[true] call ramdb_db_fnc_save;
```

### Call the save function remotely from a client:
```sqf
[] remoteExecCall ["ramdb_db_fnc_save", 2, false];
```

## Notes
- This function should be called periodically to ensure data persistence
- The backup feature creates a timestamped copy of the database
- Automatic backups can be configured in the extension's config file
- Saving is a resource-intensive operation, so it shouldn't be called too frequently
- Consider saving before mission end or during low-activity periods

## Related Functions
- `ramdb_db_fnc_load`: Loads the database from disk

## Links

[Save DB](save.md) |
[Load DB](load.md)