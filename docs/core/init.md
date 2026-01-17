---
title: ArmaRAMDb - Initialization
icon: mdi:file-text-outline
excerpt: Initial Extension settings.
---

# ramdb_db_fnc_init

## Description
Initializes the ArmaRAMDb extension and sets up the initial settings. This function loads the extension, checks its version, sets the buffer size, and logs the initialization status. It is automatically called during mission startup.

## Syntax
```sqf
// This function is automatically called during initialization and doesn't need to be called directly
```

## Parameters
None. This function does not require any parameters.

## Return Value
None. The function sets up internal variables and logs initialization information.

## Examples
This function is automatically executed during framework initialization and doesn't need to be called manually.

## Notes
- Sets the global buffer size to 20480 bytes (20KB)
- Logs the DLL version number for reference
- Confirms successful loading of all functions
- Outputs initialization status to the RPT logs
- This is one of the first functions called when the framework loads
- It does not initialize or load any database data - use `ramdb_db_fnc_load` for that

## Related Functions
- `ramdb_db_fnc_load`: Loads database data from disk
- `ramdb_db_fnc_save`: Saves database data to disk

## Links

[Init](init.md) |
[Test](test.md)