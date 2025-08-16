#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Set multiple hash fields at once.
 *
 * Arguments:
 * 0: Key <STRING> - Key to set
 * 1: Data <ARRAY> - Array of field-value pairs
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey", ["field1", "value1", "field2", "value2"]] call ramdb_db_fnc_hmset; (Server or Singleplayer only)
 * ["myKey", ["field1", "value1", "field2", "value2"]] remoteExec ["ramdb_db_fnc_hmset", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_data", [], [[]]]
];

if (_data isEqualTo []) exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_hmset' Invalid Input for Data '%1'", _data];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

if ((count _data) mod 2 != 0) exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_hmset' Invalid number of elements in data array '%1' - must be key-value pairs", _data];
    GVAR(inuse) = false;
};

private _args = [_key];
_args append _data;

private _result = "ArmaRAMDb" callExtension ["hmset", _args];

GVAR(inuse) = false;

_result
