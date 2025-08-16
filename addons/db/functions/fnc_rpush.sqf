#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Push a value to the right of a list.
 *
 * Arguments:
 * 0: Key <STRING> - Key to push
 * 1: Data <ARRAY> - Data to push
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey", ["myData"]] call ramdb_db_fnc_rpush; (Server or Singleplayer only)
 * ["myKey", ["myData"]] remoteExec ["ramdb_db_fnc_rpush", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_data", [], [[], "", 0, false]]
];

if (_key == "") exitWith {
    diag_log text format ["ArmaRAMDb: 'ramdb_db_fnc_rpush' Invalid Input for Key '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaRAMDb" callExtension ["rpush", [_key, _data]];

GVAR(inuse) = false;

_result
