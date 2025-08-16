#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Set a value at a specific index in a list.
 *
 * Arguments:
 * 0: Key <STRING> - Key to set
 * 1: Index <NUMBER> - Index to set
 * 2: Data <ARRAY> - Data to set
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey", 0, ["myData"]] call ramdb_db_fnc_lset; (Server or Singleplayer only)
 * ["myKey", 0, ["myData"]] remoteExec ["ramdb_db_fnc_lset", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_index", 0, [0]],
    ["_value", "", [[], "", 0, false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_lset' Invalid Input for Key '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["lset", [_key, _index, _value]];

GVAR(inuse) = false;

_result
