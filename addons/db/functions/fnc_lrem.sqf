#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Remove a value from a list.
 *
 * Arguments:
 * 0: Key <STRING> - Key to remove
 * 1: Count <NUMBER> - Count to remove
 * 2: Value <STRING> - Value to remove
 *
 * Return Value:
 * Number of removed values <NUMBER>
 *
 * Example:
 * ["myKey", 1, "myValue"] call ramdb_db_fnc_lrem; (Server or Singleplayer only)
 * ["myKey", 1, "myValue"] remoteExec ["ramdb_db_fnc_lrem", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_count", 0, [0]],
    ["_value", "", [[], "", 0, false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_lrem' Invalid Input for Key '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["lrem", [_key, _count, _value]];

GVAR(inuse) = false;

_result
