---
title: ArmaRAMDb - Test
icon: mdi:file-text-outline
excerpt: Test Function.
---

# ramdb_db_fnc_test

## Description
A simple test function for verifying database operations. This function displays the received data in a hint message and logs it to the RPT file. It's primarily used for testing database retrieval operations and callback functionality.

## Syntax
```sqf
[_value] spawn ramdb_db_fnc_test
```

## Parameters
| Parameter | Type                             | Description                      | Default |
|-----------|----------------------------------|----------------------------------|---------|
| `_value`  | Array, String, Number, or Boolean| The value to display and log     | []      |

## Return Value
The same value that was passed to the function. Also sets the global variable `ramdb_db_test` to this value.

## Examples
### Test with a simple string:
```sqf
["Hello World!"] spawn ramdb_db_fnc_test;
```

### Test database retrieval by specifying this as callback function:
```sqf
["playerStats", "ramdb_db_fnc_test"] call ramdb_db_fnc_get;
```

### Test from client to server:
```sqf
["Database is working!"] remoteExec ["ramdb_db_fnc_test", 2, false];
```

## Notes
- Displays the received value using `hint`
- Logs the same value to the RPT file
- Stores the value in the global variable `ramdb_db_test` for later inspection
- Commonly used as a callback function for database operations
- Useful for debugging and verifying data flow
- Can be called directly or specified as a callback in other functions

## Related Functions
N/A

## Links

[Init](init.md) |
[Test](test.md)