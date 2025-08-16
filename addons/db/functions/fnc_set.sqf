#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Set a key in the database.
 *
 * Arguments:
 * 0: Key <STRING> - Key to set
 * 1: Data <ARRAY> - Data to set
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey", ["myData"]] call ramdb_db_fnc_set; (Server or Singleplayer only)
 * ["myKey", ["myData"]] remoteExec ["ramdb_db_fnc_set", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_data", [], [[], "", 0, false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_set' Invalid Input for Key '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["set", [_key, _data]];

GVAR(inuse) = false;

_result
