---
title: ArmaRAMDb - Fetch
icon: mdi:file-text-outline
excerpt: Handles data chunks received from the database extension when data is too large to be returned in a single callback.
---

# ramdb_db_fnc_fetch

## Description
Handles data chunks received from the database extension when data is too large to be returned in a single callback. This function collects all chunks of data, reassembles them in the correct order, and then passes the complete data to the specified function.

## Syntax
```sqf
[_uniqueID, _function, _index, _total, _datachunk, _call, _object] call ramdb_db_fnc_fetch
```

## Parameters
| Parameter   | Type      | Description                                                 |
|-------------|-----------|-------------------------------------------------------------|
| `_uniqueID` | String    | Unique identifier for this data fetch operation             |
| `_function` | String    | Name of the function to call after data is assembled        |
| `_index`    | Number    | Current chunk index (0-based)                               |
| `_total`    | Number    | Total number of chunks expected                             |
| `_datachunk`| String    | The chunk of data being received                            |
| `_call`     | Boolean   | Whether to call the function directly (true) or spawn (false) |
| `_object`   | Object    | (Optional) Object to whom the data belongs     |

## Return Value
None. When all chunks are received, the function will:
1. Assemble the complete data string
2. Parse it as a simple array
3. Call the specified function with the parsed data

## Examples
This function is typically not called directly but is triggered by the extension's callback mechanism when large datasets are retrieved.

## Notes
- The function stores received chunks in the global array `ramdb_db_fetch_array`
- Chunks are sorted by their index to ensure correct assembly regardless of arrival order
- After successful processing, the chunks for this uniqueID are removed from the array
- This function is essential for handling large datasets that exceed the callback buffer limit

## Related Functions
N/A