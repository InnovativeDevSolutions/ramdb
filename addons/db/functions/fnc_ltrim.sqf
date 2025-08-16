#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Trim a list to retain only the elements within the specified range.
 *
 * Arguments:
 * 0: Key <STRING> - Key to trim
 * 1: Start <NUMBER> - Start index
 * 2: End <NUMBER> - End index
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey", 0, 10] call ramdb_db_fnc_ltrim; (Server or Singleplayer only)
 * ["myKey", 0, 10] remoteExec ["ramdb_db_fnc_ltrim", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_start", 0, [0]],
    ["_end", 0, [0]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_ltrim' Invalid Input for Key '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["ltrim", [_key, _start, _end]];

GVAR(inuse) = false;

_result
